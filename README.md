# 🎮 FiapCloudGames - API de Plataforma de Jogos Digitais

Tech Challenge - Fase 1 | FIAP

## 📋 Sobre o Projeto

API REST desenvolvida para a FIAP Cloud Games, uma plataforma de venda de jogos digitais e gestão de biblioteca de jogos adquiridos. Implementa arquitetura em camadas com autenticação JWT, autorização baseada em roles e testes abrangentes.

## 🚀 Tecnologias

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQLite** - Banco de dados
- **JWT Authentication** - Autenticação baseada em tokens
- **BCrypt.Net-Next** - Hashing de senhas
- **Swagger/OpenAPI** - Documentação da API
- **xUnit** - Framework de testes unitários
- **SpecFlow** - Framework BDD para testes de comportamento
- **Microsoft.AspNetCore.Mvc.Versioning** - Versionamento de API

## ✨ Funcionalidades Implementadas

### Autenticação e Autorização
- ✅ Cadastro de usuários com validação de dados
- ✅ Login com geração de token JWT
- ✅ Dois níveis de acesso: User e Admin
- ✅ Controle de acesso baseado em roles
- ✅ Seed automático de usuário administrador

### Gestão de Jogos
- ✅ Listagem de jogos disponíveis (público)
- ✅ CRUD completo de jogos (Admin)
- ✅ Compra de jogos por usuários
- ✅ Biblioteca pessoal do usuário
- ✅ Validação de saldo e duplicatas

### Gestão de Usuários (Admin)
- ✅ Listagem de todos os usuários
- ✅ Visualização de detalhes de usuário
- ✅ Alteração de roles (User ↔ Admin)
- ✅ Exclusão de usuários

### Testes e Qualidade
- ✅ 9 testes unitários passando
- ✅ Testes BDD com SpecFlow
- ✅ Cobertura de cenários críticos
- ✅ Middleware de tratamento de erros
- ✅ Middleware de logging de requisições

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação em camadas:

```
FiapCloudGames/
├── API/                    # Camada de Apresentação
│   ├── Controllers/        # Endpoints REST
│   └── Middleware/         # Middlewares customizados
├── Application/            # Camada de Aplicação
│   ├── DTOs/              # Objetos de Transferência de Dados
│   └── Services/          # Serviços de Aplicação
├── Domain/                 # Camada de Domínio
│   ├── Entities/          # Entidades de negócio
│   └── Services/          # Serviços de Domínio
├── Infrastructure/         # Camada de Infraestrutura
│   ├── Data/              # Contexto do banco e seed
│   └── Repositories/      # Implementações de repositório
└── FiapCloudGames.Tests/   # Testes
    ├── Controllers/       # Testes unitários
    ├── Features/          # Cenários BDD
    └── Steps/             # Implementação dos steps BDD
```

## 🔧 Como Executar

### Pré-requisitos
- .NET 9 SDK instalado
- SQLite (incluído no projeto)

### Instalação e Execução

1. **Clone o repositório:**
   ```bash
   git clone <url-do-repositorio>
   cd FiapCloudGames-Correto
   ```

2. **Restaure as dependências:**
   ```bash
   dotnet restore
   ```

3. **Execute as migrações do banco:**
   ```bash
   dotnet ef database update
   ```

4. **Execute a aplicação:**
   ```bash
   dotnet run --urls=http://localhost:5169
   ```

5. **Acesse a documentação:**
   - Swagger UI: http://localhost:5169/swagger
   - API Base: http://localhost:5169/api

### Credenciais de Admin

Após a primeira execução, um usuário administrador é criado automaticamente:
- **Email:** admin@fiapcloudgames.com
- **Senha:** Admin@123

## 📚 API Endpoints

### Autenticação
```
POST /api/auth/register  - Registrar novo usuário
POST /api/auth/login     - Fazer login (retorna JWT)
```

### Jogos
```
GET    /api/games           - Listar jogos disponíveis (público)
GET    /api/games/{id}      - Detalhes de um jogo (público)
POST   /api/games           - Criar jogo (Admin)
PUT    /api/games/{id}      - Atualizar jogo (Admin)
DELETE /api/games/{id}      - Remover jogo (Admin)
POST   /api/games/{id}/purchase - Comprar jogo (User)
```

### Usuários (Admin)
```
GET    /api/users           - Listar todos os usuários (Admin)
GET    /api/users/{id}      - Detalhes de usuário (Admin)
PATCH  /api/users/{id}/role - Alterar role do usuário (Admin)
DELETE /api/users/{id}      - Remover usuário (Admin)
```

### Minha Biblioteca
```
GET    /api/games/library   - Ver minha biblioteca (User)
```

## 🧪 Executar Testes

### Testes Unitários
```bash
dotnet test FiapCloudGames.Tests/FiapCloudGames.Tests.csproj
```

### Testes BDD (SpecFlow)
```bash
dotnet test FiapCloudGames.Tests/FiapCloudGames.Tests.csproj --filter "Category=BDD"
```

## 🔒 Segurança

- **JWT Tokens:** Autenticação stateless com expiração
- **BCrypt:** Hashing seguro de senhas
- **Role-based Access Control:** Controle granular de permissões
- **Input Validation:** Validação de dados em DTOs
- **SQL Injection Protection:** Uso de EF Core com parâmetros

## 📊 Status dos Testes

- ✅ **9/9 testes unitários** passando
- ✅ **Testes BDD** implementados
- ✅ **Cobertura de cenários críticos** (login, registro, autorização)
- ✅ **Testes de integração** via HTTP requests

## 🎯 Requisitos do Tech Challenge - Fase 1

| Requisito | Status | Descrição |
|-----------|--------|-----------|
| API REST | ✅ | Endpoints RESTful implementados |
| Autenticação | ✅ | JWT com login/registro |
| Autorização | ✅ | Roles User/Admin |
| CRUD Jogos | ✅ | Operações completas |
| Compra de Jogos | ✅ | Sistema de biblioteca |
| Gestão Admin | ✅ | CRUD de usuários |
| Testes | ✅ | Unitários + BDD |
| Documentação | ✅ | Swagger + README |
| Arquitetura | ✅ | Clean Architecture |
| Segurança | ✅ | Hashing + RBAC |

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto é parte do Tech Challenge da FIAP.

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