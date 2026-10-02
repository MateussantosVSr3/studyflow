# Publicar e entregar

O projeto, README, migration e capturas estão prontos. A entrega só é concluída quando o link do repositório é enviado na atividade.

## Pelo site do GitHub

1. Crie um repositório público chamado `studyflow-cp5` (ou outro nome).
2. Extraia o ZIP `artifacts/StudyFlow-CP5.zip` em uma pasta.
3. No repositório, use **Add file → Upload files** e envie os arquivos e pastas extraídos, preservando a estrutura. Não envie somente o ZIP.
4. Confirme que o README e as imagens de evidência aparecem no repositório.
5. Envie o link do repositório na atividade antes das 23:59.

## Pelo terminal

Na raiz deste projeto, após instalar/autenticar o GitHub CLI:

```bash
gh auth login
git add .
git commit -m "Implementa StudyFlow para CP5 com EF Core, versionamento e evidencias"
gh repo create studyflow-cp5 --public --source=. --remote=origin --push
```

Se o Git solicitar identidade, configure seu próprio nome e e-mail com `git config user.name` e `git config user.email` antes do commit.

Somente um integrante deve criar o repositório do grupo. Se escolher repositório privado, conceda acesso ao professor. Confira o link e a acessibilidade antes de enviar.
