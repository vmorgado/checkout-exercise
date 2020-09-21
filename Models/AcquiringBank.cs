namespace dotnetexample.Models
{
    public class MockedAquiringBank : IAcquiringBank {

        public BankResponse processPayment() {
            
            return new BankResponse {
                id = "teste",
                successful = true
            };
        }
    }
    public interface IAcquiringBank
    {
        BankResponse processPayment();
    }

    public class BankResponse {
        public string id { get; set; }
        public bool successful { get; set; }
    }
}
