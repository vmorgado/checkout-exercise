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
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CCV { get; set; }
    }
}
