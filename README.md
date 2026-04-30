<h1>🎮 Fiap Cloud Games API</h1>

<p>API REST desenvolvida em <strong>.NET 8</strong> para gerenciamento de usuários, autenticação, biblioteca de jogos digitais e promoções, como parte do Tech Challenge.</p>

<hr>

<h2>📌 Sobre o projeto</h2>

<p>A <strong>Fiap Cloud Games (FCG)</strong> é uma plataforma de venda de jogos digitais com foco educacional.</p>

<p>Nesta fase (MVP), o objetivo foi construir uma API capaz de:</p>

<ul>
<li>Gerenciar usuários</li>
<li>Autenticar via JWT</li>
<li>Controlar níveis de acesso (User e Admin)</li>
<li>Gerenciar jogos e biblioteca</li>
<li>Permitir administração da plataforma</li>
<li>Gerenciar promoções</li>
</ul>

<hr>

<h2>🚀 Tecnologias utilizadas</h2>

<ul>
<li>.NET 8</li>
<li>ASP.NET Core</li>
<li>Entity Framework Core</li>
<li>SQLite</li>
<li>JWT Authentication</li>
<li>Swagger / OpenAPI</li>
<li>BCrypt</li>
<li>xUnit</li>
<li>Moq</li>
<li>SpecFlow</li>
</ul>

<hr>

<h2>📂 Arquitetura</h2>

<p>O projeto segue uma arquitetura monolítica organizada em camadas:</p>

<ul>
<li><strong>Domain</strong> → entidades e regras de negócio</li>
<li><strong>Application</strong> → serviços e casos de uso</li>
<li><strong>Infrastructure</strong> → persistência e repositórios</li>
<li><strong>API</strong> → controllers, middlewares e configuração</li>
</ul>

<hr>

<h2>🔐 Autenticação e Autorização</h2>

<p>A autenticação é feita via <strong>JWT (JSON Web Token)</strong>.</p>

<h3>Perfis de acesso</h3>

<table>
<thead>
<tr>
<th>Perfil</th>
<th>Permissões</th>
</tr>
</thead>
<tbody>
<tr>
<td>User</td>
<td>Acessar jogos e biblioteca</td>
</tr>
<tr>
<td>Admin</td>
<td>Gerenciar jogos, usuários e promoções</td>
</tr>
</tbody>
</table>

<hr>

<h2>👤 Usuários padrão (Seed)</h2>

<p>Ao iniciar a aplicação, são criados usuários automaticamente:</p>

<h3>Admin</h3>
<ul>
<li>Email: <code>admin@fiapcloudgames.com</code></li>
<li>Senha: <code>Adm1n@SecurePass2026</code></li>
</ul>

<h3>Usuário comum</h3>
<ul>
<li>Email: <code>user@fiapcloudgames.com</code></li>
<li>Senha: <code>User@SecurePass2026</code></li>
</ul>

<hr>

<h2>▶️ Como rodar o projeto</h2>

<h3>1. Clonar o repositório</h3>

<pre><code>git clone &lt;URL_DO_REPOSITORIO&gt;
cd Tech-Challenge-Fiap-Cloud-Games</code></pre>

<h3>2. Restaurar dependências</h3>

<pre><code>dotnet restore .\FiapCloudGames\FiapCloudGames.sln</code></pre>

<h3>3. Compilar a solução</h3>

<pre><code>dotnet build .\FiapCloudGames\FiapCloudGames.sln</code></pre>

<h3>4. Executar a API</h3>

<pre><code>dotnet run --project .\FiapCloudGames</code></pre>

<h3>5. Acessar o Swagger</h3>

<pre><code>http://localhost:5169/swagger</code></pre>

<hr>

<h2>🔑 Como autenticar no Swagger</h2>

<ol>
<li>Execute o login no endpoint de autenticação</li>
<li>Copie o token JWT retornado</li>
<li>Clique em <strong>Authorize</strong></li>
<li>Informe no formato: <code>Bearer SEU_TOKEN</code></li>
</ol>

<hr>

<h2>📡 Principais endpoints</h2>

<h3>Autenticação</h3>
<table>
<thead>
<tr>
<th>Método</th>
<th>Endpoint</th>
<th>Descrição</th>
</tr>
</thead>
<tbody>
<tr>
<td>POST</td>
<td><code>/api/auth/register</code></td>
<td>Cadastrar usuário</td>
</tr>
<tr>
<td>POST</td>
<td><code>/api/auth/login</code></td>
<td>Autenticar usuário</td>
</tr>
</tbody>
</table>

<h3>Jogos</h3>
<table>
<thead>
<tr>
<th>Método</th>
<th>Endpoint</th>
<th>Descrição</th>
</tr>
</thead>
<tbody>
<tr>
<td>GET</td>
<td><code>/api/games</code></td>
<td>Listar jogos</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/games/{id}</code></td>
<td>Buscar jogo por id</td>
</tr>
<tr>
<td>POST</td>
<td><code>/api/games</code></td>
<td>Criar jogo</td>
</tr>
<tr>
<td>PUT</td>
<td><code>/api/games/{id}</code></td>
<td>Atualizar jogo</td>
</tr>
<tr>
<td>DELETE</td>
<td><code>/api/games/{id}</code></td>
<td>Remover jogo</td>
</tr>
<tr>
<td>POST</td>
<td><code>/api/games/{id}/purchase</code></td>
<td>Comprar jogo</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/games/library</code></td>
<td>Listar biblioteca do usuário autenticado</td>
</tr>
</tbody>
</table>

<h3>Promoções</h3>
<table>
<thead>
<tr>
<th>Método</th>
<th>Endpoint</th>
<th>Descrição</th>
</tr>
</thead>
<tbody>
<tr>
<td>GET</td>
<td><code>/api/promotions</code></td>
<td>Listar promoções</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/promotions/{id}</code></td>
<td>Buscar promoção por id</td>
</tr>
<tr>
<td>POST</td>
<td><code>/api/promotions</code></td>
<td>Criar promoção</td>
</tr>
<tr>
<td>PUT</td>
<td><code>/api/promotions/{id}</code></td>
<td>Atualizar promoção</td>
</tr>
<tr>
<td>DELETE</td>
<td><code>/api/promotions/{id}</code></td>
<td>Remover promoção</td>
</tr>
</tbody>
</table>

<h3>Usuários</h3>
<table>
<thead>
<tr>
<th>Método</th>
<th>Endpoint</th>
<th>Descrição</th>
</tr>
</thead>
<tbody>
<tr>
<td>GET</td>
<td><code>/api/users</code></td>
<td>Listar usuários</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/users/{id}</code></td>
<td>Buscar usuário por id</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/users/me</code></td>
<td>Perfil do usuário autenticado</td>
</tr>
<tr>
<td>GET</td>
<td><code>/api/users/me/library</code></td>
<td>Biblioteca detalhada do usuário autenticado</td>
</tr>
<tr>
<td>PATCH</td>
<td><code>/api/users/me</code></td>
<td>Atualizar próprio perfil</td>
</tr>
<tr>
<td>PUT</td>
<td><code>/api/users/{id}</code></td>
<td>Atualizar usuário por id</td>
</tr>
<tr>
<td>DELETE</td>
<td><code>/api/users/{id}</code></td>
<td>Remover usuário</td>
</tr>
<tr>
<td>PATCH</td>
<td><code>/api/users/{id}/role</code></td>
<td>Alterar role do usuário</td>
</tr>
<tr>
<td>POST</td>
<td><code>/api/users/{userId}/games/{gameId}</code></td>
<td>Atribuir jogo a um usuário</td>
</tr>
</tbody>
</table>

<hr>

<h2>🧪 Testes</h2>

<p>Os testes estão no projeto: <code>FiapCloudGames.Tests</code></p>

<h3>Tipos de testes implementados</h3>
<ul>
<li>Testes unitários</li>
<li>Testes de controller com mocks</li>
<li>Testes BDD com SpecFlow</li>
</ul>

<h3>Executar testes</h3>

<pre><code>dotnet test .\FiapCloudGames\FiapCloudGames.sln</code></pre>

<hr>

<h2>📊 Persistência de dados</h2>

<ul>
<li><strong>Banco utilizado:</strong> SQLite</li>
<li><strong>ORM:</strong> Entity Framework Core</li>
<li>Migrations aplicadas automaticamente na inicialização</li>
</ul>

<hr>

<h2>🛡️ Middlewares</h2>

<p>O projeto inclui:</p>

<ul>
<li>Middleware de tratamento de exceções</li>
<li>Middleware de logging estruturado</li>
</ul>

<hr>

<h2>📐 Documentação DDD</h2>

<p>A documentação de domínio inclui:</p>

<ul>
<li>Event Storming</li>
<li>Fluxos de criação de usuários</li>
<li>Fluxos de criação de jogos</li>
<li>Fluxos de criação de promoções</li>
</ul>

<p>👉 <code>https://miro.com/welcomeonboard/eUlXQlJiNTdtay81ejFhbTh2TVBCS3BzdVZHa2ZFWG5FcmpPeWtqWTZDS2hWNTZtbTJrVjhFRU5aQkg4SHV1bnAwYmNacDE5TW5aaWwydnNnZ3JhZVA3MEZBYzZtc08yaGZ6eXpwTGdvSlY5bFMzOXQyaXF2R0dFdzFaVjlZc25BS2NFMDFkcUNFSnM0d3FEN050ekl3PT0hdjE=?share_link_id=127801181002</code></p>

<hr>

<h2>🎥 Demonstração</h2>

<p>Vídeo de apresentação do projeto:</p>

<p>👉 <code>https://www.youtube.com/watch?v=jK4k5t0l8l4</code></p>

<hr>

<h2>👥 Equipe</h2>

<h3>Integrantes:</h3>

<table>
<thead>
<tr>
<th>Pollyana Rocha</th>
<th>Pollyana Rocha - RM372429</th>
</tr>
</thead>
</table>

<hr>

<h2>📄 Licença</h2>

<p>Projeto acadêmico para fins educacionais.</p>

</div>
</body>
</html>