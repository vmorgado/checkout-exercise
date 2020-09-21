using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnetexample.Models
{
    public class PaymentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Issuer { get; set; }
        public string CardHolder { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CCV { get; set; }
        
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime transactionDate { get; set; }
        public BankResponse response { get; set;}
    }

    public class CreatePaymentDto {
        public string Issuer { get; set; }
        public string CardHolder { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CCV { get; set; }
    }

    public class PaymentResponse {
        public PaymentModel paymentRequest { get; set;}
        public BankResponse paymentResponse { get; set; }
    }
}
