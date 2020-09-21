using System;
using Xunit;
using dotnetexample.Services;
using dotnetexample.Controllers;
using Microsoft.Extensions.Logging;
using dotnetexample.Models;
using System.Threading.Tasks;
using Moq;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    public class PaymentServiceTests
    {
        
        [Fact]
        public async Task Should_Be_Able_Create_PaymentModel_Document_And_Request_Bank_To_Process() {

            
            var bankResponse = new BankResponse {
                id = "mocekd-transaction-id",
                successful = true,
            };

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

            var paymentModelDocument = new PaymentModel {
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
            };
 
            var mongoCollectionMock = new Mock<IMongoCollection<PaymentModel>>();
            mongoCollectionMock.Setup(col => col.InsertOne( paymentModelDocument, null, default));

            var acquiringBank = new MockedAcquiringBank {};

            var paymentService = new PaymentService( mongoCollectionMock.Object, acquiringBank );
            var result = paymentService.Create(createPaymentDto);
            Assert.IsType<PaymentResponse>(result);
            Assert.Equal(result.paymentResponse.successful, true);
        }
    }
}