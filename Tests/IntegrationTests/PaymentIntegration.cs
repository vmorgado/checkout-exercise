using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using dotnetexample;
using dotnetexample.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace IntegrationTests
{
    public class PaymentIntegrationTest {

        private IHostBuilder GetHostBuilder() {
                        // Arrange            
            return new HostBuilder()
                .ConfigureAppConfiguration( (context, config) => {
                    System.Environment.SetEnvironmentVariable("PaymentDatabaseSettings__CollectionName","payments");
                    System.Environment.SetEnvironmentVariable("PaymentDatabaseSettings__ConnectionString","mongodb://localhost:27017");
                    System.Environment.SetEnvironmentVariable("PaymentDatabaseSettings__DatabaseName","paymenthub");
                    System.Environment.SetEnvironmentVariable("LoggingDatabaseSettings__CollectionName","logs");
                    System.Environment.SetEnvironmentVariable("LoggingDatabaseSettings__ConnectionString","mongodb://localhost:27017");
                    System.Environment.SetEnvironmentVariable("LoggingDatabaseSettings__DatabaseName","paymenthub");

                    config.AddEnvironmentVariables();
                    
                })
                .ConfigureWebHost(webHost =>
                {
                    // Add TestServer
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });
        }

        [Fact]
        public async Task Should_Get_UnAuthenticated()
        {   
            var hostBuilder = this.GetHostBuilder();
            // Create and start up the host
            var host = await hostBuilder.StartAsync();

            // Create an HttpClient which is setup for the test host
            var client = host.GetTestClient();

            // Act
            var response = await client.GetAsync("/payment/5f6b703e5e1e7b715ad7c03b");

            // Assert

            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Fact]
        public async Task Should_Get_A_Valid_Payment()
        {   
            var hostBuilder = this.GetHostBuilder();
            // Create and start up the host
            var host = await hostBuilder.StartAsync();

            // Create an HttpClient which is setup for the test host
            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", "C5BFF7F0-B4DF-475E-A331-F737424F013C");

            // Act
            var response = await client.GetAsync("/payment/5f6b703e5e1e7b715ad7c03b");

            // Assert

            Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        }
        [Fact]
        public async Task Should_Be_Able_To_Perform_A_Payment()
        {   
            var hostBuilder = this.GetHostBuilder();
            // Create and start up the host
            var host = await hostBuilder.StartAsync();

            var createPaymentDto = new {
                issuer = "market-app",
                cardHolder = "John Cage",
                value = 199.99m,
                currency = "Euro",
                cardNumber = "0000-0000-0000-0000",
                expiryMonth = 13,
                expiryYear = 2099,
                ccv = "121",
            };

            // Create an HttpClient which is setup for the test host
            var client = host.GetTestClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", "C5BFF7F0-B4DF-475E-A331-F737424F013C");
            client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
            client.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br");

            var body = new StringContent(JsonSerializer.Serialize(createPaymentDto), Encoding.UTF8, "application/json");
            
            var responseMessage = await client.PostAsync("/payment", body );
            Assert.Equal(System.Net.HttpStatusCode.OK, responseMessage.StatusCode);
        }

    }
}
