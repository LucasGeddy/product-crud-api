using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using product_crud_api.Infra;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FluentAssertions;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Newtonsoft.Json;
using product_crud_api.API.DTO;
using System.Collections.Generic;
using product_crud_api.Infra.Models;
using System.Text;

namespace Tests.Integration
{
    public class ProductControllerTest : IDisposable
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client; 
        private IConfiguration _config;
        private Product _baseProduct;
        private string _requestUri;
        public ProductControllerTest()
        {
            _config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.UseTestServer();
                    builder.ConfigureServices(services =>
                    {
                        services.AddControllers();
                        services.AddDbContext<ApiDbContext>(options =>
                        {
                            options.UseSqlServer(_config["ConnectionStrings:DefaultConnection"]);
                        });
                    });
                });

            _client = _factory.CreateClient();

            _requestUri = "/product";

            _baseProduct = new Product()
            {
                Name = "Test Product",
                Description = "Description Test",
                Lot = "666",
                Price = 525.45m
            };
        }
        public void Dispose()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApiDbContext>().Database.ExecuteSqlRaw("TRUNCATE TABLE Products");
            }
            _factory.Dispose();
        }
        [Fact]
        public async Task Get_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var requestUri = $"{_requestUri}?pageNumber=1&pageSize=5";

            // Act
            var response = await _client.GetAsync(requestUri);
            var content = JsonConvert.DeserializeObject<PagedResponse<IEnumerable<Product>>>(await response.Content.ReadAsStringAsync());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        [Fact]
        public async Task Post_ValidProduct_ReturnsValidGetRoute()
        {
            //Arrange

            //Act
            var createProductResponse = await _client.PostAsync(_requestUri, 
                new StringContent(JsonConvert.SerializeObject(_baseProduct), UnicodeEncoding.UTF8, "application/json"));
            var getCreatedProductResponse = await _client.GetStringAsync(createProductResponse.Headers.Location);
            var getResponseContent = JsonConvert.DeserializeObject<Response<Product>>(getCreatedProductResponse);

            // Assert
            createProductResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            getResponseContent.Data.Name.Should().Be(_baseProduct.Name);
            getResponseContent.Data.Description.Should().Be(_baseProduct.Description);
            getResponseContent.Data.Lot.Should().Be(_baseProduct.Lot);
            getResponseContent.Data.Price.Should().Be(_baseProduct.Price);
        }
        [Fact]
        public async Task Put_ValidProduct_ReturnsValidGetRouteAndEditedValues()
        {
            //Arrange
            var editedProduct = new Product()
            {
                Id = 0,
                Name = "Test Name Edited",
                Description = "Test Description Edited",
                Lot = "Test Lot Edited",
                Price = 666.66m,
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };

            //Act
            var createProductResponse = await _client.PostAsync(_requestUri,
                new StringContent(JsonConvert.SerializeObject(_baseProduct), UnicodeEncoding.UTF8, "application/json"));
            var createdProductContent = JsonConvert.DeserializeObject<Response<Product>>(await createProductResponse.Content.ReadAsStringAsync());
            editedProduct.Id = createdProductContent.Data.Id;
            var editProductResponse = await _client.PutAsync(_requestUri,
                new StringContent(JsonConvert.SerializeObject(editedProduct), UnicodeEncoding.UTF8, "application/json"));
            var getProductResponse = await _client.GetAsync($"{_requestUri}/{createdProductContent.Data.Id}");
            var getProductContent = JsonConvert.DeserializeObject<Response<Product>>(await getProductResponse.Content.ReadAsStringAsync());

            // Assert
            editProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getProductContent.Data.Name.Should().Be(editedProduct.Name);
            getProductContent.Data.Description.Should().Be(editedProduct.Description);
            getProductContent.Data.Lot.Should().Be(editedProduct.Lot);
            getProductContent.Data.Price.Should().Be(editedProduct.Price);
        }
        [Fact]
        public async Task Delete_ValidProductId_ReturnsSuccessAndDatabaseIsEmpty()
        {
            //Arrange

            //Act
            var createProductResponse = await _client.PostAsync(_requestUri,
                new StringContent(JsonConvert.SerializeObject(_baseProduct), UnicodeEncoding.UTF8, "application/json"));
            var createProductContent = JsonConvert.DeserializeObject<Response<Product>>(await createProductResponse.Content.ReadAsStringAsync());
            var deleteResponse = await _client.DeleteAsync($"{_requestUri}/{createProductContent.Data.Id}");
            var getAllResponse = await _client.GetAsync(_requestUri);

            // Assert
            createProductResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getAllResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}