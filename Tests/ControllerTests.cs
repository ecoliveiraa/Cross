// /Tests/ControllerTests.cs (NOVO FICHEIRO)
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using WebApplication1.Models;

namespace WebApplication1.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location));
                    builder.UseEnvironment("Testing");
                });
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task CreateCompany_ValidPortugueseCompany_Returns201()
        {
            // Arrange
            var company = new
            {
                country = "Portugal",
                nif = "123456789",
                address = "Rua das Flores, Lisboa",
                stakeholder = "João Silva",
                contact = "joao@empresa.pt"
            };

            var json = JsonSerializer.Serialize(company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/company", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(responseContent);
        }

        [Test]
        public async Task CreateCompany_PortugalWithoutNIF_Returns400()
        {
            // Arrange
            var company = new
            {
                country = "Portugal",
                nif = "", // ← Vazio para Portugal - deve falhar
                address = "Rua das Flores, Lisboa",
                stakeholder = "João Silva",
                contact = "joao@empresa.pt"
            };

            var json = JsonSerializer.Serialize(company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/company", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.That(responseContent, Does.Contain("NIF is required"));
        }

        [Test]
        public async Task CreateCompany_SpainWithoutNIF_Returns201()
        {
            // Arrange
            var company = new
            {
                country = "Spain",
                nif = "", // ← Vazio mas OK porque não é Portugal
                address = "Calle Mayor, Madrid",
                stakeholder = "Carlos Lopez",
                contact = "carlos@empresa.es"
            };

            var json = JsonSerializer.Serialize(company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/company", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task GetAllCompanies_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/company");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseContent);
        }

        [Test]
        public async Task CreateLead_ActiveWithoutBusinessType_Returns400()
        {
            // Arrange - Primeiro criar uma company
            var company = new
            {
                country = "Portugal",
                nif = "123456789",
                address = "Lisboa",
                stakeholder = "João",
                contact = "joao@test.pt"
            };

            var companyJson = JsonSerializer.Serialize(company);
            var companyContent = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var companyResponse = await _client.PostAsync("/api/company", companyContent);
            
            // Extrair ID da resposta (simplificado)
            var companyResponseContent = await companyResponse.Content.ReadAsStringAsync();
            var companyResult = JsonSerializer.Deserialize<JsonElement>(companyResponseContent);
            var companyId = companyResult.GetProperty("id").GetString();

            // Criar lead inválido
            var lead = new
            {
                companyId = companyId,
                businessType = "", // ← Vazio com status Active - deve falhar
                status = "Active"
            };

            var leadJson = JsonSerializer.Serialize(lead);
            var leadContent = new StringContent(leadJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/lead", leadContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.That(responseContent, Does.Contain("BusinessType is required"));
        }

        [Test]
        public async Task CreateLead_DraftWithoutBusinessType_Returns201()
        {
            // Arrange - Primeiro criar uma company
            var company = new
            {
                country = "Spain",
                nif = "",
                address = "Madrid",
                stakeholder = "Carlos",
                contact = "carlos@test.es"
            };

            var companyJson = JsonSerializer.Serialize(company);
            var companyContent = new StringContent(companyJson, Encoding.UTF8, "application/json");
            var companyResponse = await _client.PostAsync("/api/company", companyContent);
            
            var companyResponseContent = await companyResponse.Content.ReadAsStringAsync();
            var companyResult = JsonSerializer.Deserialize<JsonElement>(companyResponseContent);
            var companyId = companyResult.GetProperty("id").GetString();

            // Criar lead válido
            var lead = new
            {
                companyId = companyId,
                businessType = "", // ← Vazio mas OK porque é Draft
                status = "Draft"
            };

            var leadJson = JsonSerializer.Serialize(lead);
            var leadContent = new StringContent(leadJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/lead", leadContent);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task CreateProduct_WithValidData_Returns201()
        {
            // Arrange
            var product = new
            {
                productType = "Electronics",
                dependentProductId = (string)null
            };

            var json = JsonSerializer.Serialize(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/product", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [Test]
        public async Task GetNonExistentCompany_Returns404()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/company/{nonExistentId}");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}