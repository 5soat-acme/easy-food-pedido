using AutoFixture.AutoMoq;
using AutoFixture;
using Moq;
using EF.Identidade.Application.UseCases;
using EF.WebApi.Commons.Identity;
using Microsoft.Extensions.Options;
using FluentAssertions;

namespace EF.Identidade.Application.Test.UseCases;

public class IdentidadeUseCaseTest
{
    private readonly IFixture _fixture;
    private readonly IdentitySettings _identitySettings;
    private readonly Mock<IOptions<IdentitySettings>> _identitySettingsMock;
    private readonly IdentidadeUseCase _identidadeUseCase;

    public IdentidadeUseCaseTest()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _identitySettings = _fixture.Create<IdentitySettings>();
        _identitySettings.Secret = "lMoIuyBcYkvPpczZlJB7ipeHkJt5IYZeAIjlkj9y";
        _identitySettingsMock = _fixture.Freeze<Mock<IOptions<IdentitySettings>>>();
        _identitySettingsMock.Setup(x => x.Value).Returns(_identitySettings);
        _identidadeUseCase = _fixture.Create<IdentidadeUseCase>();
    }

    [Fact]
    public async Task DeveGerarTokenDeAcesso()
    {
        // Act
        var result = await _identidadeUseCase.AcessarSistema();

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().NotBeNullOrEmpty();
    }
}
