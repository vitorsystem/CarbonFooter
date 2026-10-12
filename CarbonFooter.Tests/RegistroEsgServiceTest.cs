using CarbonFooter.Api.Models;
using CarbonFooter.Api.Repositories;
using CarbonFooter.Api.Services;
using Moq;

namespace CarbonFooter.Tests;

public class RegistroEsgServiceTest
{
    private readonly Mock<IRegistroEsgRepository> _repositoryMock;
    private readonly RegistroEsgService _service;

    public RegistroEsgServiceTest()
    {
        _repositoryMock = new Mock<IRegistroEsgRepository>();
        _service = new RegistroEsgService(_repositoryMock.Object);
    }

    [Fact]
    public async Task DeveCalcularEmissaoAoSalvarQuandoNaoInformada()
    {
        // Arrange
        var entrada = new RegistroEsg("TI", 100.0, null);

        _repositoryMock.Setup(r => r.SalvarAsync(It.IsAny<RegistroEsg>()))
            .ReturnsAsync((RegistroEsg r) => r);

        // Act
        var resultado = await _service.SalvarAsync(entrada);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(8.5, resultado.EmissoesCo2Kg);
    }
}