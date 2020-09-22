using dotnetexample.Models;

namespace dotnetexample.Services
{
public class MockedAcquiringBank : IAcquiringBank {
        
        IRandomNumberGenerator _randomSeed;
        public MockedAcquiringBank (IRandomNumberGenerator randomSeed ) {
            _randomSeed = randomSeed;
        }
        public BankResponse processPayment(BankRequest request) {
            var seed = _randomSeed.Generate();
            
            if (seed  < 7) {
                return new BankResponse {
                    id = "successful-transaction",
                    successful = true
                };
            }

            if (seed < 9) {
                return new BankResponse {
                    id = "failed-transaction",
                    successful = false
                };
            }

            throw new System.Exception("Bank Service Unavailable");
        }
    }
}