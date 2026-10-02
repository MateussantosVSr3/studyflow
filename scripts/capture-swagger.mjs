import { chromium } from 'playwright';
import fs from 'node:fs/promises';

const browser = await chromium.launch({ channel: 'chrome', headless: true });
const page = await browser.newPage({ viewport: { width: 1440, height: 1080 } });
await page.goto('http://localhost:5080/swagger');
await page.locator('.opblock').first().waitFor();
await fs.mkdir('docs/evidencias', { recursive: true });
await page.screenshot({ path: 'docs/evidencias/00-swagger.png', fullPage: true });
async function execute(method, path, file, expected, body, id) {
  const block = page.locator(`.opblock-${method}`).filter({ has: page.locator(`.opblock-summary-path[data-path="${path}"]`) }).first();
  if (!(await block.locator('.opblock-body').count())) await block.locator('.opblock-summary').click();
  const tryButton = block.locator('.try-out__btn').first();
  await tryButton.waitFor();
  if ((await tryButton.innerText()) === 'Try it out') await tryButton.click();
  if (body) await block.locator('textarea').fill(JSON.stringify(body, null, 2));
  if (id !== undefined) await block.locator('input[placeholder="id"]').fill(String(id));
  await block.getByRole('button', { name: 'Execute', exact: true }).click();
  await block.locator('.live-responses-table .response-col_status').filter({ hasText: String(expected) }).waitFor();
  await block.screenshot({ path: `docs/evidencias/${file}.png` });
  const response = await block.locator('.live-responses-table pre').allTextContents();
  return response;
}
const body = { titulo: 'Preparar entrega CP5', disciplina: 'C# Software Development',
  descricao: 'Testar a API pelo Swagger', prazo: '2026-10-10', concluida: false };
const output = await execute('post', '/api/v1/tarefas', '01-post-201', 201, body);
const parsed = output.map(t => { try { return JSON.parse(t); } catch { return null; } }).find(t => t?.id);
if (!parsed) throw new Error('Não foi possível obter o ID criado no Swagger.');
const id = parsed.id;
await execute('get', '/api/v1/tarefas', '02-get-lista-200', 200);
await execute('get', '/api/v1/tarefas/{id}', '03-get-id-200', 200, null, id);
await execute('put', '/api/v1/tarefas/{id}', '04-put-204', 204, { ...body, concluida: true }, id);
await execute('post', '/api/v1/tarefas', '05-post-invalido-400', 400, { titulo: '', disciplina: '', prazo: '0001-01-01' });
await execute('delete', '/api/v1/tarefas/{id}', '06-delete-204', 204, null, id);
await execute('get', '/api/v1/tarefas/{id}', '07-get-inexistente-404', 404, null, id);
await browser.close();
console.log('Capturas de requisições reais no Swagger concluídas.');
