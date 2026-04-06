# 🎮 FiapCloudGames - API de Plataforma de Jogos Digitais

Tech Challenge - Fase 1 | FIAP

## 📋 Sobre o Projeto

API REST desenvolvida para a FIAP Cloud Games, uma plataforma de venda de jogos digitais e gestão de biblioteca de jogos adquiridos.

## 🚀 Tecnologias

- .NET 9
- Entity Framework Core
- SQLite
- JWT Authentication
- Swagger
- xUnit (Testes)
- BDD (SpecFlow)

## ✨ Funcionalidades

- ✅ Cadastro de usuários com validação
- ✅ Autenticação JWT
- ✅ Dois níveis de acesso (User/Admin)
- ✅ CRUD completo de jogos
- ✅ Compra de jogos
- ✅ Biblioteca pessoal do usuário
- ✅ Testes unitários (4 testes)
- ✅ Testes BDD (3 cenários)
- ✅ Documentação Swagger

## 🏗️ Estrutura do Projeto


FiapCloudGames/
├── API/Controllers/ # Endpoints
├── Application/DTOs/ # Data Transfer Objects
├── Domain/Entities/ # Entidades de domínio
├── Infrastructure/Data/ # Context e Migrations
└── Tests/ # Testes unitários e BDD

text

## 🔧 Como Executar

### Pré-requisitos
- .NET 9 SDK
- SQLite

### Passos

```bash
# Clonar o repositório
git clone https://github.com/SEU_USUARIO/FiapCloudGames.git

# Entrar na pasta
cd FiapCloudGames

# Restaurar pacotes
dotnet restore

# Criar banco de dados
dotnet ef database update

# Executar a API
dotnet run
Acesse: http://localhost:5169/swagger

📝 Endpoints Principais
Método	Endpoint	Descrição	Role
POST	/auth/register	Cadastrar usuário	-
POST	/auth/login	Fazer login	-
GET	/api/games/list	Listar jogos	User
POST	/api/games/create	Criar jogo	Admin
POST	/api/games/buy/{id}	Comprar jogo	User
GET	/api/games/my-games	Minha biblioteca	User
🧪 Testes
bash
# Executar todos os testes
dotnet test

# Resultado: 7 testes, 0 falhas
👥 Autores
Pollyana Manuela Silva Rocha

📄 Licença
Este projeto foi desenvolvido para fins educacionais - FIAP

🔗 Links
Repositório

Vídeo de Apresentação

Documentação DDD - Miro