using System;
using System.ComponentModel.DataAnnotations;
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
        public string CardNumber { set; get;}
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CCV { get; set; }
        
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime transactionDate { get; set; }
        public BankResponse response { get; set;}
    }

    public class CreatePaymentDto {
        [Required]
        public string Issuer { get; set; }
        [Required]
        public string CardHolder { get; set; }
        [Required]
        public decimal Value { get; set; }
        [Required]
        [RegularExpression("EURO|DOLLAR", ErrorMessage = "Please select a valid currenct (EURO|DOLLAR)")]
        public string Currency { get; set; }
        [Required]
        [RegularExpression(@"^(1298|1267|4512|4567|8901|8933)([\-\s]?[0-9]{4}){3}$)", ErrorMessage = "Invalid card number")]
        public string CardNumber { get; set; }
        [Required]
        [Range(1, 12)]
        public int ExpiryMonth { get; set; }
        [Required]
        [Range(0, 9999)]
        public int ExpiryYear { get; set; }
        [Required]
        [Range(0, 999)]
        public string CCV { get; set; }
    }

    public class PaymentResponse {
        public PaymentModel paymentRequest { get; set;}
        public BankResponse paymentResponse { get; set; }
    }
}
