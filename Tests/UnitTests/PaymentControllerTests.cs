using System;
using Xunit;
using dotnetexample.Services;
using dotnetexample.Controllers;
using Microsoft.Extensions.Logging;
using dotnetexample.Models;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace dotnetexample.Tests.UnitTests
{
    public class PaymentControllerTests
    {
        [Fact]
        public async Task Should_Be_Able_To_Preform_A_Payment_And_Recieve_A_Successful_Transaction()
        {
            var createPaymentDto = new CreatePaymentDto {
                Issuer = "amazon-issuer-id",
                CardHolder = "John Cage",
                Value = 199.99m,
                Currency = "Euro",
                CardNumber = "1234-1234-1234-1235",
                ExpiryMonth = 13,
                ExpiryYear = 2099,
                CCV = "12e2",
            };

            var bankResponse = new BankResponse {
                id = "mocekd-transaction-id",
                successful = true,
            };

            var paymentResponse = new PaymentResponse {
                paymentRequest = new PaymentModel {
                    Id = "mocked-id",
                    Issuer = "amazon-issuer-id",
                    CardHolder = "John Cage",
                    Value = 199.99m,
                    Currency = "Euro",
                    CardNumber = "1234-1234-1234-1235",
                    ExpiryMonth = 13,
                    ExpiryYear = 2099,
                    CCV = "12e2",
                    response = bankResponse,
                },
                paymentResponse = bankResponse,
                
            };


            var mockPaymentService = new Mock<IPaymentService>();
            var mockLogger = new Mock<ILogger<PaymentController>>();

            mockPaymentService.Setup<PaymentResponse>(paymentService => paymentService.Create(createPaymentDto)).Returns(paymentResponse);

            var controller = new PaymentController(mockLogger.Object, mockPaymentService.Object);

            var result = controller.Create(createPaymentDto);
            
            

            Assert.IsType<PaymentResponse>(result);
            Assert.IsAssignableFrom<PaymentModel>(result.paymentRequest);
            Assert.IsAssignableFrom<BankResponse>(result.Value.paymentResponse);
            Assert.Equal( result.Value.paymentRequest.response.id, result.Value.paymentResponse.id);
        }
        
    }
}