@ProdutoController
Feature: ProdutoController

Controller de Produtos

Scenario: Obter produtos existentes
    Given que eu tenho produtos com categoria "Lanche"
    When eu solicitar produtos com categoria "Lanche"
    Then a resposta deve ser 200

Scenario: Obter produtos inexistentes
    Given que eu não tenho produtos com categoria "Bebida"
    When eu solicitar produtos com categoria "Bebida"
    Then a resposta deve ser 404

Scenario: Criar produto com dados válidos
    Given que eu tenho um produto válido
    When eu enviar uma requisição para criar o produto
    Then a resposta deve ser 200

Scenario: Criar produto com dados inválidos
    Given que eu tenho um produto inválido
    When eu enviar uma requisição para criar o produto
    Then a resposta deve ser 400

Scenario: Atualizar produto com dados válidos
    Given que eu tenho um produto válido com id "23bf0c51-7230-4a5b-8031-10b3dde9a908"
    When eu enviar uma requisição para atualizar o produto
    Then a resposta deve ser 200

Scenario: Atualizar produto com dados inválidos
    Given que eu tenho um produto inválido com id "bbb311a7-08db-4248-97c4-52aafe0e62f7"
    When eu enviar uma requisição para atualizar o produto
    Then a resposta deve ser 500

Scenario: Remover um produto existente
    Given que eu tenho um produto válido com id "23bf0c51-7230-4a5b-8031-10b3dde9a908"
    When eu enviar uma requisição para remover o produto com id "23bf0c51-7230-4a5b-8031-10b3dde9a908"
    Then a resposta deve ser 200

Scenario: Remover um produto inexistente
    Given que eu tenho um produto inválido com id "bbb311a7-08db-4248-97c4-52aafe0e62f7"
    When eu enviar uma requisição para remover o produto com id "bbb311a7-08db-4248-97c4-52aafe0e62f7"
    Then a resposta deve ser 500