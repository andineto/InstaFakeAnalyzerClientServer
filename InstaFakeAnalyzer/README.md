# 🤖 FakeNewsAnalyzer - Chatbot de Verificação de Fake News via Instagram

Este projeto é um chatbot que analisa mensagens recebidas via Instagram Direct Message (DM) para verificar se são fake news. O sistema utiliza inteligência artificial para analisar o conteúdo e responder automaticamente com uma justificativa e um veredito (verdadeiro ou falso).

## 🔍 Funcionalidades

- Recebimento de mensagens via Instagram DM (API Oficial da Meta).
- Análise do conteúdo com IA (DeepSeek R1 ou outro modelo).
- Consulta a banco de dados para verificar se a notícia já foi avaliada.
- Resposta automática com justificativa e flag de fake news.
- API RESTful com endpoints para recebimento e resposta.
- Interface de teste local com ngrok.

## 🛠️ Tecnologias Utilizadas

- C# (.NET)
- ASP.NET Core Web API
- Entity Framework Core + Migrations
- PostgreSQL
- DeepSeek R1 / OpenRouter
- Facebook Graph API + Instagram Graph API
- ngrok (para testes locais de Webhook)

## 🚀 Como rodar localmente

1. Clone este repositório
2. Crie o banco de dados PostgreSQL e configure `appsettings.json`
3. Execute as migrations:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. Inicie o projeto:

```bash
dotnet run
```

4.5. Para testar de forma local, com postman, envie uma requisição do tipo post para http://localhost:5170/api/noticias com corpo:
{
    "Conteudo": "Sua noticia para teste"
}

5. Rode o ngrok para expor seu localhost:

```bash
ngrok http https://localhost:porta
```

## 📲 Requisitos para integração com Instagram

### Permissões do Facebook Business

| Permissão | Finalidade |
|----------|------------|
| `instagram_basic` | Acesso ao perfil da conta |
| `instagram_manage_messages` | Leitura/envio de mensagens do Instagram |
| `pages_show_list` | Listagem de páginas conectadas |
| `pages_manage_metadata` | Configuração de webhooks |
| `pages_messaging` | Envio de mensagens pela página |
| `pages_read_engagement` | Leitura de mensagens recebidas |

> ⚠️ Você precisa conectar uma Página do Facebook a um perfil Instagram Business e gerar um token com essas permissões.

## 🧪 Testes

- Durante o modo de desenvolvimento, apenas administradores, desenvolvedores ou testadores definidos no Facebook App poderão interagir com o bot.
- Use o [Graph API Explorer](https://developers.facebook.com/tools/explorer/) para gerar tokens com permissões temporárias.

## 💡 Créditos

Este projeto foi desenvolvido como trabalho escolar com fins educativos.
