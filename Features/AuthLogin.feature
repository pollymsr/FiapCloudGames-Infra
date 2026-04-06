Feature: Autenticação de Usuário
  Como um usuário da plataforma FIAP Cloud Games
  Eu quero fazer login na minha conta
  Para acessar minha biblioteca de jogos

  Background:
    Given que o sistema está rodando
    And o banco de dados está configurado

  @login @sucesso
  Scenario: Login com credenciais válidas
    Given que existe um usuário cadastrado com email "joao@email.com" e senha "Senha@123"
    When eu faço login com email "joao@email.com" e senha "Senha@123"
    Then eu recebo um token JWT válido
    And o token contém o papel "User"

  @login @falha
  Scenario: Login com senha incorreta
    Given que existe um usuário cadastrado com email "maria@email.com" e senha "Maria@456"
    When eu faço login com email "maria@email.com" e senha "SenhaErrada"
    Then eu recebo uma resposta "Credenciais inválidas"
    And o status code é 401 Unauthorized

  @login @email-inexistente
  Scenario: Login com email não cadastrado
    Given que não existe um usuário com email "naoexiste@email.com"
    When eu faço login com email "naoexiste@email.com" e senha "Qualquer@123"
    Then eu recebo uma resposta "Credenciais inválidas"
    And o status code é 401 Unauthorized

  @registro @sucesso
  Scenario: Registrar novo usuário com dados válidos
    Given que não existe um usuário com email "novo@email.com"
    When eu registro um novo usuário com nome "Novo Usuario", email "novo@email.com" e senha "Nova@123"
    Then o usuário é criado com sucesso
    And o status code é 200 OK

  @registro @email-duplicado
  Scenario: Registrar usuário com email já existente
    Given que existe um usuário cadastrado com email "existente@email.com" e senha "Existente@123"
    When eu registro um novo usuário com nome "Outro", email "existente@email.com" e senha "Outra@456"
    Then recebo uma mensagem de erro "Email já cadastrado"
    And o status code é 400 Bad Request