using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using product_crud_api.Infra;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using FluentAssertions;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Core;
using System.Net.Http;

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
                            options.UseSqlServer(_config["DefaultConnection"]);
                        });
                    });
                    builder.Configure(app =>
                    {
                        app.UseHttpsRedirection();
                        app.UseAuthorization();
                        //app.MapControllers();
                    });
                });

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
            
            // This didn't work, try later with help
            //HttpTestServer = await new HostBuilder().ConfigureWebHost(webBuilder =>
            //{
            //    webBuilder.UseTestServer();
            //    webBuilder.ConfigureServices(services =>
            //        {
            //            services.AddControllers();
            //            services.AddDbContext<ApiDbContext>(options =>
            //            {
            //                options.UseSqlServer(ConfigurationFile["DefaultConnection"]);
            //            });
            //        });
            //    webBuilder.Configure(app =>
            //        {
            //            app.UseAuthorization();
            //            //app.MapControllers();
            //        });
            //}).StartAsync();
        }
        public void Dispose()
        {
            _factory.Dispose();
        }
        [Fact]
        public async Task Get_EmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            const string requestUri = "Product?pageNumber=1&pageSize=5";

            // Act
            var response = await _client.GetAsync(requestUri);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().BeNull();
        }
    }
}