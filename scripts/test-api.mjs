import assert from 'node:assert/strict';
import fs from 'node:fs/promises';

const base = process.env.API_URL ?? 'http://localhost:5080';
const results = [];
async function request(name, method, route, expected, body) {
  const response = await fetch(base + route, {
    method, headers: body ? { 'Content-Type': 'application/json' } : {},
    body: body ? JSON.stringify(body) : undefined
  });
  const text = await response.text();
  results.push({ name, method, url: base + route, request: body ?? null,
    status: response.status, headers: Object.fromEntries(response.headers),
    response: text ? JSON.parse(text) : null });
  assert.equal(response.status, expected, name);
  return results.at(-1);
}
const route = '/api/v1/tarefas';
const body = { titulo: 'Revisar EF Core', disciplina: 'C# Software Development',
  descricao: 'Estudar DbContext e migrations para o CP5', prazo: '2026-10-10', concluida: false };
const created = await request('01-post-201', 'POST', route, 201, body);
const id = created.response.id;
assert.ok(created.headers.location.endsWith(`${route}/${id}`));
const list = await request('02-get-lista-200', 'GET', route, 200);
assert.ok(list.response.some(t => t.id === id));
const found = await request('03-get-id-200', 'GET', `${route}/${id}`, 200);
assert.equal(found.response.titulo, body.titulo);
await request('04-put-204', 'PUT', `${route}/${id}`, 204, { ...body, concluida: true });
const updated = await request('05-get-filtro-200', 'GET', `${route}?concluida=true`, 200);
assert.ok(updated.response.some(t => t.id === id && t.concluida));
await request('06-post-invalido-400', 'POST', route, 400, { titulo: '', disciplina: '', prazo: '0001-01-01' });
await request('07-delete-204', 'DELETE', `${route}/${id}`, 204);
await request('08-get-inexistente-404', 'GET', `${route}/${id}`, 404);
await request('09-put-inexistente-404', 'PUT', `${route}/${id}`, 404, body);
await request('10-delete-inexistente-404', 'DELETE', `${route}/${id}`, 404);
await request('11-json-invalido-400', 'POST', route, 400, { ...body, prazo: 'data-invalida' });
await request('12-versao-inexistente-404', 'GET', '/api/v2/tarefas', 404);
await fs.mkdir('docs/evidencias', { recursive: true });
await fs.writeFile('docs/evidencias/resultados.json', JSON.stringify(results, null, 2));
console.log(`${results.length} testes aprovados. Respostas reais em docs/evidencias/resultados.json`);
