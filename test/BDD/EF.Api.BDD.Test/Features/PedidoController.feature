@PedidoController
Feature: PedidoController

Controller de Pedidos

Scenario: Obter um pedido existente
    Given o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea" existe
    When eu solicitar o pedido com id "f694f3a3-2622-45ea-b168-f573f16165ea"
    Then o resultado deve ser OK com os dados do pedido

Scenario: Obter um pedido inexistente
    Given o pedido com id "017465fc-dbec-4d1f-a766-6e8ab93cf8c4" não existe
    When eu solicitar o pedido com id "017465fc-dbec-4d1f-a766-6e8ab93cf8c4"
    Then o resultado deve ser NotFound