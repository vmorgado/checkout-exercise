using dotnetexample.Services;
namespace dotnetexample.Models
{
    public interface IAcquiringBank
    {
        BankResponse processPayment(BankRequest request);
    }

    public class BankResponse {
        public string id { get; set; }
        public bool successful { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; } 
    }

    public class BankRequest {
        public string Issuer { get; set; }
        public string CardHolder { get; set; }
        public decimal Value { get; set; }
        public string Currency { get; set; }
        public string CardNumber { set; get;}
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string CCV { get; set; }
    }
}
