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
using dotnetexample.Logging;

namespace UnitTests
{
    public class PaymentServiceTests
    {
        
        [Fact]
        public async Task We_Are_Able_To_Go_Through_The_Whole_Payment_Process() {

            var createPaymentDto = new CreatePaymentDto {
                Issuer = "amazon-issuer-id",
                CardHolder = "John Cage",
                Value = 199.99m,
                Currency = "Euro",
                CardNumber = "0000-0000-0000-0000",
                ExpiryMonth = 13,
                ExpiryYear = 2099,
                CCV = "12e2",
            };

            var bankResponse = new BankResponse {
                id = "fake-id",
                successful = true,
                message = "very nice",
                statusCode = 1
            };

            var paymentModelDocument = new PaymentModel {
                Id = "mocked-id",
                Issuer = "amazon-issuer-id",
                CardHolder = "John Cage",
                Value = 199.99m,
                Currency = "Euro",
                CardNumber = "0000-0000-0000-0000",
                ExpiryMonth = 13,
                ExpiryYear = 2099,
                CCV = "12e2",
                response = bankResponse,
            };
            
            var bankRequest = new BankRequest {
                Issuer = "amazon-issuer-id",
                CardHolder = "John Cage",
                Value = 199.99m,
                Currency = "Euro",
                CardNumber = "0000-0000-0000-0000",
                ExpiryMonth = 13,
                ExpiryYear = 2099,
                CCV = "12e2",
            };

            var loggerMock = new Mock<ILoggerService>();
            loggerMock.Setup(logger => logger.Log<ExceptionMetric>(LogLevel.Error, new EventId {}, new ExceptionMetric {}, null, null));
 
            var mongoCollectionMock = new Mock<IMongoCollection<PaymentModel>>();
            mongoCollectionMock.Setup(col => col.InsertOne( paymentModelDocument, new InsertOneOptions {}, default)); 
            mongoCollectionMock.Setup(col => col.ReplaceOne( "mocked-id", paymentModelDocument, new ReplaceOptions {}, default));            

            var mockedBank = new Mock<IAcquiringBank>();
            mockedBank.Setup( bank => bank.processPayment(bankRequest)).Returns( bankResponse );
            
            var paymentService = new PaymentService( mongoCollectionMock.Object, mockedBank.Object , loggerMock.Object );
            var result = paymentService.Create(createPaymentDto);
            
            Assert.Equal(2 , mongoCollectionMock.Invocations.Count );
            Assert.Equal(1 , mockedBank.Invocations.Count );
        }
    }
}