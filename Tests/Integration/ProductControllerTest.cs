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
using System.Linq;
using System.Text;

namespace Tests.Integration
{
    public class ProductControllerTest : IDisposable
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client; 
        private IConfiguration _config;
        public ProductControllerTest()
        {
            Setup();
        }
        private async void Setup()
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

            using (var scope = _factory.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<ApiDbContext>().Database.ExecuteSqlRaw("TRUNCATE TABLE Products");
            }

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
            var requestUri = "/product?pageNumber=1&pageSize=5";

            // Act
            var response = await _client.GetStringAsync(requestUri);
            var content = JsonConvert.DeserializeObject<PagedResponse<IEnumerable<Product>>>(response);

            // Assert
            content.PageNumber.Should().Be(1);
            content.PageSize.Should().Be(5);
            content.Data.Count().Should().Be(0);
        }
        [Fact]
        public async Task Post_ValidProduct_ReturnsValidGetRoute()
        {
            //Arrange
            var requestUri = "/product";
            var productToAdd = new Product()
            {
                Name = "Test Product",
                Description =  "Description Test",
                Lot = "666",
                Price = 525.45m
            };

            //Act
            var createProductResponse = await _client.PostAsync(requestUri, 
                new StringContent(JsonConvert.SerializeObject(productToAdd), UnicodeEncoding.UTF8, "application/json"));
            var getCreatedProductResponse = await _client.GetStringAsync(createProductResponse.Headers.Location);
            var getResponseContent = JsonConvert.DeserializeObject<Response<Product>>(getCreatedProductResponse);

            // Assert
            createProductResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            getResponseContent.Data.Name.Should().Be(productToAdd.Name);
            getResponseContent.Data.Description.Should().Be(productToAdd.Description);
            getResponseContent.Data.Lot.Should().Be(productToAdd.Lot);
            getResponseContent.Data.Price.Should().Be(productToAdd.Price);
        }
        [Fact]
        public async Task Put_ValidProduct_ReturnsValidGetRouteAndEditedValues()
        {
            //Arrange
            var requestUri = "/product";
            var productToAdd = new Product()
            {
                Name = "Test Product",
                Description = "Description Test",
                Lot = "666",
                Price = 525.45m
            };
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
            var createProductResponse = await _client.PostAsync(requestUri,
                new StringContent(JsonConvert.SerializeObject(productToAdd), UnicodeEncoding.UTF8, "application/json"));
            var createdProductContent = JsonConvert.DeserializeObject<Response<Product>>(await createProductResponse.Content.ReadAsStringAsync());
            editedProduct.Id = createdProductContent.Data.Id;
            var editProductResponse = await _client.PutAsync(requestUri,
                new StringContent(JsonConvert.SerializeObject(editedProduct), UnicodeEncoding.UTF8, "application/json"));
            var getProductResponse = await _client.GetAsync(editProductResponse.Headers.Location);
            var getProductContent = JsonConvert.DeserializeObject<Response<Product>>(await getProductResponse.Content.ReadAsStringAsync());

            // Assert
            editProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getProductContent.Data.Name.Should().Be(editedProduct.Name);
            getProductContent.Data.Description.Should().Be(editedProduct.Description);
            getProductContent.Data.Lot.Should().Be(editedProduct.Lot);
            getProductContent.Data.Price.Should().Be(editedProduct.Price);
        }
    }
}