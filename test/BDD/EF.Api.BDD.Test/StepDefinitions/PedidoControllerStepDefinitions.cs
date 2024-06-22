using EF.Api.BDD.Test.Support;
using EF.Pedidos.Domain.Models;
using FluentAssertions;
using System.Net;
using System.Text.Json;
using EF.Pedidos.Application.DTOs.Responses;

namespace EF.Api.BDD.Test.StepDefinitions;

[Binding]
[Scope(Tag = "PedidoController")]
public class PedidoControllerStepDefinitions : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpResponseMessage _result;

    public PedidoControllerStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Given(@"o pedido com id ""(.*)"" existe")]
    public async Task DadoQueOPedidoComIdExiste(Guid pedidoId)
    {       
    }

    [Given(@"o pedido com id ""(.*)"" não existe")]
    public void DadoQueOPedidoComIdNaoExiste(Guid pedidoId)
    {
    }

    [When(@"eu solicitar o pedido com id ""(.*)""")]
    public async Task QuandoEuSolicitarOPedidComId(Guid pedidoId)
    {
        _result = await _client.GetAsync($"/api/pedidos/{pedidoId}");
    }

    [Then(@"o resultado deve ser OK com os dados do pedido")]
    public async Task EntaoOResultadoDeveSerOKComOsDadosDoPedido()
    {
        var contentString = await _result.Content.ReadAsStringAsync();
        var pedidoDto = JsonSerializer.Deserialize<PedidoDto>(contentString);

        _result.StatusCode.Should().Be(HttpStatusCode.OK);
        pedidoDto!.Status.Should().Be(Status.AguardandoPagamento);
    }

    [Then(@"o resultado deve ser NotFound")]
    public void EntaoOResultadoDeveSerNotFound()
    {
        _result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}