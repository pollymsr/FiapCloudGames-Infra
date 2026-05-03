Feature: Autenticação de usuário

  Scenario: Login com credenciais válidas
    Given que existe um usuário com email "teste@email.com" e senha "Senha@123"
    When eu solicitar autenticação com email "teste@email.com" e senha "Senha@123"
    Then o sistema deve retornar um token JWT

  Scenario: Login com senha inválida
    Given que existe um usuário com email "teste@email.com" e senha "Senha@123"
    When eu solicitar autenticação com email "teste@email.com" e senha "SenhaErrada"
    Then o sistema não deve autenticar o usuário