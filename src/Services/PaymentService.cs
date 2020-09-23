
using dotnetexample.Models;
using dotnetexample.Exception;
using dotnetexample.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace dotnetexample.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IMongoCollection<PaymentModel> _paymentModelCollection;
        private readonly IAcquiringBank _acquiringBank;
        private ILoggerService _logger;

        public PaymentService(IMongoCollection<PaymentModel> paymentModelCollection, IAcquiringBank acquiringBank, ILoggerService logger)
        {
            _acquiringBank = acquiringBank;
            _paymentModelCollection = paymentModelCollection;
            _logger = logger;
        }

        public PaymentModel Get(string id) { 
            
            var paymentModel = _paymentModelCollection.Find<PaymentModel>(PaymentModel => PaymentModel.Id == id).FirstOrDefault();
            paymentModel.CardNumber = this.HideCreditCardNumber(paymentModel.CardNumber);
            paymentModel.CCV = this.HideCCV();
            return paymentModel;
        }
        
        public PaymentResponse Create(CreatePaymentDto createPaymentDto)
        {   
            var bankResponse = new BankResponse {
                id = "",
                successful = false,
                statusCode = 0,
                message = "this request never made it to the bank",
            };

            var paymentModel = new PaymentModel {
                Issuer = createPaymentDto.Issuer,
                CardHolder = createPaymentDto.CardHolder,
                Value = createPaymentDto.Value,
                Currency = createPaymentDto.Currency,
                CardNumber = createPaymentDto.CardNumber,
                ExpiryMonth = createPaymentDto.ExpiryMonth,
                ExpiryYear = createPaymentDto.ExpiryYear,
                CCV = createPaymentDto.CCV,
                transactionDate = System.DateTimeOffset.Now.UtcDateTime,
                response = bankResponse
            };


            var bankRequest = new BankRequest {
                Issuer = createPaymentDto.Issuer,
                CardHolder = createPaymentDto.CardHolder,
                Value = createPaymentDto.Value,
                Currency = createPaymentDto.Currency,
                CardNumber = createPaymentDto.CardNumber,
                ExpiryMonth = createPaymentDto.ExpiryMonth,
                ExpiryYear = createPaymentDto.ExpiryYear,
                CCV = createPaymentDto.CCV,
            };

            _paymentModelCollection.InsertOne(paymentModel);
            
            try {

                bankResponse = _acquiringBank.processPayment(bankRequest);

                paymentModel.response = bankResponse;

                this.Update(paymentModel.Id, paymentModel);

            }  catch (System.Exception ex) {

                var exceptionMetric = new ExceptionMetric {
                    origin = nameof(PaymentService),
                    exception = new AcquiringBankNotAvailable(ex.Message),
                    time = new DateTimeOffset().Date,
                    stack = ex.StackTrace
                };
                
                _logger.Log<ExceptionMetric>(LogLevel.Error, new EventId {}, exceptionMetric, ex );
            }

            paymentModel.CardNumber = this.HideCreditCardNumber(paymentModel.CardNumber);
            paymentModel.CCV = this.HideCCV();

            return new PaymentResponse { 
                paymentRequest = paymentModel,
                paymentResponse = bankResponse,
            };
            
        }

        public void Update(string id, PaymentModel paymentModelIn) =>
            _paymentModelCollection.ReplaceOne(paymentModel => paymentModel.Id == id, paymentModelIn, new ReplaceOptions {}, default);

        // this can be done in the serialization layer, todo: find where
        private string HideCreditCardNumber( string creditCardNumber ) {
            try {

                var lastIndex = creditCardNumber.LastIndexOf("-");
                var lenght = creditCardNumber.Length;
                
                return string.Join("XXXX-XXXX-XXXX-", creditCardNumber.Substring(lastIndex, lenght - lastIndex));

            } catch (System.Exception e) {
                
                var exceptionMetric = new ExceptionMetric {
                    origin = nameof(PaymentService),
                    exception = new WrongCardNumberStoredException("The card number stored has not the correct format to be hidden, returning empty instead"),
                    time = new DateTimeOffset().Date,
                    stack = e.StackTrace
                };

                _logger.Log<ExceptionMetric>(LogLevel.Error, new EventId {}, exceptionMetric, e );
                return "";
            }
        }

        private string HideCCV() {

            return "***";
        }
    }
}
