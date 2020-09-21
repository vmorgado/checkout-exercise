using dotnetexample.Services;
namespace dotnetexample.Models
{
    public class MockedAcquiringBank : IAcquiringBank {
        
        IRandomNumberGenerator _randomSeed;
        public MockedAcquiringBank (IRandomNumberGenerator randomSeed ) {
            _randomSeed = randomSeed;
        }
        public BankResponse processPayment() {
            
            if (_randomSeed.Generate() < 7) {
                return new BankResponse {
                    id = "successful-transaction",
                    successful = true
                };
            }
            if (_randomSeed.Generate() < 9) {
                return new BankResponse {
                    id = "failed-transaction",
                    successful = false
                };
            }

            throw new System.Exception("Bank Service Unavailable");
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
