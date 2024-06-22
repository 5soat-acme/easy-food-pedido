using AutoFixture.AutoMoq;
using AutoFixture;
using EF.Api.BDD.Test.Support;
using EF.Produtos.Domain.Models;
using EF.Produtos.Application.DTOs.Requests;
using System.Text;
using System.Net;
using FluentAssertions;
using System.Text.Json;

namespace EF.Api.BDD.Test.StepDefinitions;

[Binding]
[Scope(Tag = "ProdutoController")]
public class ProdutoControllerStepDefinitions
{
    private readonly IFixture _fixture;
    private readonly CustomWebApplicationFactory<Program> _factory;    
    private readonly HttpClient _client;    
    private HttpResponseMessage _result;
    private CriarProdutoDto _produtoCriar;
    private AtualizarProdutoDto _produtoAtualizar;
    private Guid _produtoId;

    public ProdutoControllerStepDefinitions(CustomWebApplicationFactory<Program> factory)
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Given(@"que eu tenho produtos com categoria ""(.*)""")]
    public async Task DadoQueEuTenhoProdutosComCategoria(string categoria)
    {
    }

    [Given(@"que eu não tenho produtos com categoria ""(.*)""")]
    public async Task DadoQueEuNaoTenhoProdutosComCategoria(string categoria)
    {
    }

    [Given(@"que eu tenho um produto válido")]
    public void DadoQueEuTenhoUmProdutoValido()
    {
        _produtoCriar = new CriarProdutoDto()
        {
            Nome = "Produto 1",
            Descricao = "Produto 1 Descrição",
            ValorUnitario = 10.5M,            
            Categoria = ProdutoCategoria.Lanche,
            TempoPreparoEstimado = 20
        };
    }

    [Given(@"que eu tenho um produto inválido")]
    public void DadoQueEuTenhoUmProdutoInvalido()
    {
        _produtoCriar = new CriarProdutoDto()
        {
            Nome = "Produto 1",
            Descricao = "Produto 1 Descrição",
            ValorUnitario = -1,
            Categoria = ProdutoCategoria.Lanche,
            TempoPreparoEstimado = 20
        };
    }

    [Given(@"que eu tenho um produto válido com id ""(.*)""")]
    public void DadoQueEuTenhoUmProdutoValidoComId(Guid produtoId)
    {
        _produtoId = produtoId;
        _produtoAtualizar = _fixture.Create<AtualizarProdutoDto>();
    }

    [Given(@"que eu tenho um produto inválido com id ""(.*)""")]
    public void DadoQueEuTenhoUmProdutoInvalidoComId(Guid produtoId)
    {
        _produtoId = produtoId;
        _produtoAtualizar = _fixture.Create<AtualizarProdutoDto>();
    }

    [When(@"eu solicitar produtos com categoria ""(.*)""")]
    public async Task QuandoEuSolicitarProdutosComCategoria(string categoria)
    {
        _result = await _client.GetAsync($"/api/produtos?categoria={categoria}");
    }

    [When(@"eu enviar uma requisição para criar o produto")]
    public async Task QuandoEuEnviarUmaRequisiçãoParaCriarOProduto()
    {
        var content = new StringContent(JsonSerializer.Serialize(_produtoCriar), Encoding.UTF8, "application/json");
        _result = await _client.PostAsync("/api/produtos", content);
    }

    [When(@"eu enviar uma requisição para atualizar o produto")]
    public async Task QuandoEuEnviarUmaRequisicaoParaAtualizarOProduto()
    {
        var content = new StringContent(JsonSerializer.Serialize(_produtoAtualizar), Encoding.UTF8, "application/json");
        _result = await _client.PutAsync($"/api/produtos/{_produtoId}", content);
    }

    [When(@"eu enviar uma requisição para remover o produto com id ""(.*)""")]
    public async Task QuandoEuEnviarUmaRequisiçãoParaRemoverOProdutoComId(Guid produtoId)
    {
        _result = await _client.DeleteAsync($"/api/produtos/{produtoId}");
    }

    [Then(@"a resposta deve ser (.*)")]
    public void EntaoARespostaDeveSer(int statusCode)
    {
        _result.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
