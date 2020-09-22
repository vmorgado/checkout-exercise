
using dotnetexample.Models;
using dotnetexample.Exception;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dotnetexample.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IMongoCollection<PaymentModel> _paymentModelCollection;
        private readonly IAcquiringBank _acquiringBank;

        public PaymentService(IMongoCollection<PaymentModel> paymentModelCollection, IAcquiringBank acquiringBank)
        {
            _acquiringBank = acquiringBank;
            _paymentModelCollection = paymentModelCollection;
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
                id = "not-processed-by-the-bank",
                successful = false,
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

            _paymentModelCollection.InsertOne(paymentModel);
            
            try {
                bankResponse = _acquiringBank.processPayment( new BankRequest {
                    CardHolder = createPaymentDto.CardHolder,
                    Value = createPaymentDto.Value,
                    Currency = createPaymentDto.Currency,
                    CardNumber = createPaymentDto.CardNumber,
                    ExpiryMonth = createPaymentDto.ExpiryMonth,
                    ExpiryYear = createPaymentDto.ExpiryYear,
                    CCV = createPaymentDto.CCV,
                });

            }  catch (System.Exception ex) {

                throw new AcquiringBankNotAvailable(ex.Message);
            }

            paymentModel.response = bankResponse;

            this.Update(paymentModel.Id, paymentModel);


            paymentModel.CardNumber = this.HideCreditCardNumber(paymentModel.CardNumber);
            paymentModel.CCV = this.HideCCV();

            return new PaymentResponse { 
                paymentRequest = paymentModel,
                paymentResponse = bankResponse,
            };
            
        }

        public void Update(string id, PaymentModel paymentModelIn) =>
            _paymentModelCollection.ReplaceOne(paymentModel => paymentModel.Id == id, paymentModelIn, new ReplaceOptions {}, default);



        private string HideCreditCardNumber( string creditCardNumber ) {
            try {
                var lastIndex = creditCardNumber.LastIndexOf("-");
                var lenght = creditCardNumber.Length;
                
                return string.Join("XXXX-XXXX-XXXX-", creditCardNumber.Substring(lastIndex, lenght - lastIndex));
            } catch {

                return "";
            }
        }

        private string HideCCV() {

            return "***";
        }
    }
}
