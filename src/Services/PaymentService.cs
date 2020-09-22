
using dotnetexample.Models;
using MongoDB.Driver;
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
                response = new BankResponse {
                    id = "",
                    successful = false,
                }
            };

            _paymentModelCollection.InsertOne(paymentModel);
            
            var bankResponse = _acquiringBank.processPayment();
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
