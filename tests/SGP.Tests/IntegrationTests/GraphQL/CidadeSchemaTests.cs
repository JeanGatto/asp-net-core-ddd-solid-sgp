using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using SGP.Application.Responses;
using SGP.PublicApi.GraphQL.Constants;
using SGP.Tests.Extensions;
using SGP.Tests.Fixtures;
using SGP.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace SGP.Tests.IntegrationTests.GraphQL
{
    public class CidadeSchemaTests : IntegrationTestBase, IClassFixture<WebTestApplicationFactory>
    {
        public CidadeSchemaTests(WebTestApplicationFactory factory, ITestOutputHelper outputHelper)
            : base(factory, outputHelper)
        {
        }

        [Fact]
        public async Task Devera_RetornarResultadoSucessoComCidades_AoObterPorUf()
        {
            // Arrange
            const int total = 645;
            const string uf = "SP";
            const string queryName = QueryNames.CidadesPorEstado;

            var request = new GraphQLQuery<CidadeResponse>(queryName)
                .AddArguments(new { uf })
                .AddField(c => c.Regiao)
                .AddField(c => c.Estado)
                .AddField(c => c.Uf)
                .AddField(c => c.Nome)
                .AddField(c => c.Ibge)
                .ToGraphQLRequest();

            // Act
            var result
                = await HttpClient.SendAndDeserializeAsync<IEnumerable<CidadeResponse>>(
                    OutputHelper, EndPoints.Api.Cidades, request, queryName);

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.HaveCount(total)
                .And.Subject.ForEach(c =>
                {
                    c.Regiao.Should().NotBeNullOrWhiteSpace();
                    c.Estado.Should().NotBeNullOrWhiteSpace();
                    c.Uf.Should().NotBeNullOrWhiteSpace().And.HaveLength(2).And.Be(uf);
                    c.Nome.Should().NotBeNullOrWhiteSpace();
                    c.Ibge.Should().BePositive();
                });
        }

        [Fact]
        public async Task Devera_RetornarErroNaoEncontrado_AoObterTodosPorUfInexistente()
        {
            // Arrange
            var request = new GraphQLQuery<CidadeResponse>(QueryNames.CidadesPorEstado)
                .AddArguments(new { uf = "XX" })
                .AddField(c => c.Nome)
                .ToGraphQLRequest();

            // Act
            var result = await HttpClient.SendAndGetErrorsAsync(OutputHelper, EndPoints.Api.Cidades, request);

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Subject.ForEach(error => error.Message.Should().NotBeNullOrWhiteSpace());
        }

        [Fact]
        public async Task Devera_RetornarErroValidacao_AoObterPorIbgeInexistente()
        {
            // Arrange
            var request = new GraphQLQuery<CidadeResponse>(QueryNames.CidadePorIbge)
                .AddArguments(new { ibge = int.MaxValue })
                .AddField(c => c.Ibge)
                .ToGraphQLRequest();

            // Act
            var result = await HttpClient.SendAndGetErrorsAsync(OutputHelper, EndPoints.Api.Cidades, request);

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Subject.ForEach(error => error.Message.Should().NotBeNullOrWhiteSpace());
        }

        [Fact]
        public async Task Devera_RetornarErroValidacao_AoObterPorIbgeInvalido()
        {
            // Arrange
            var request = new GraphQLQuery<CidadeResponse>(QueryNames.CidadePorIbge)
                .AddArguments(new { ibge = int.MinValue })
                .AddField(c => c.Ibge)
                .ToGraphQLRequest();

            // Act
            var result = await HttpClient.SendAndGetErrorsAsync(OutputHelper, EndPoints.Api.Cidades, request);

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Subject.ForEach(error => error.Message.Should().NotBeNullOrWhiteSpace());
        }

        [Fact]
        public async Task Devera_RetornarErroValidacao_AoObterTodosPorUfInvalido()
        {
            // Arrange
            var request = new GraphQLQuery<CidadeResponse>(QueryNames.CidadesPorEstado)
                .AddArguments(new { uf = "XXX.XX_X" })
                .AddField(c => c.Nome)
                .ToGraphQLRequest();

            // Act
            var result = await HttpClient.SendAndGetErrorsAsync(OutputHelper, EndPoints.Api.Cidades, request);

            // Assert
            result.Should().NotBeNullOrEmpty()
                .And.OnlyHaveUniqueItems()
                .And.Subject.ForEach(error => error.Message.Should().NotBeNullOrWhiteSpace());
        }

        [Fact]
        public async Task Devera_RetornarResultadoSucessoComCidade_AoObterPorIbge()
        {
            // Arrange
            const int ibge = 3557105;
            const string queryName = QueryNames.CidadePorIbge;

            var request = new GraphQLQuery<CidadeResponse>(queryName)
                .AddArguments(new { ibge })
                .AddField(c => c.Regiao)
                .AddField(c => c.Estado)
                .AddField(c => c.Uf)
                .AddField(c => c.Nome)
                .AddField(c => c.Ibge)
                .ToGraphQLRequest();

            // Act
            var result
                = await HttpClient.SendAndDeserializeAsync<CidadeResponse>(
                    OutputHelper, EndPoints.Api.Cidades, request, queryName);

            // Assert
            result.Should().NotBeNull();
            result.Regiao.Should().NotBeNullOrWhiteSpace();
            result.Estado.Should().NotBeNullOrWhiteSpace();
            result.Uf.Should().NotBeNullOrWhiteSpace().And.HaveLength(2);
            result.Nome.Should().NotBeNullOrWhiteSpace();
            result.Ibge.Should().BePositive().And.Be(ibge);
        }
    }
}