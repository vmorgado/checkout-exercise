using dotnetexample.Models;
using System;

namespace dotnetexample.Services
{
public class MockedAcquiringBank : IAcquiringBank {
        
        IRandomNumberGenerator _randomSeed;
        public MockedAcquiringBank (IRandomNumberGenerator randomSeed ) {
            _randomSeed = randomSeed;
        }
        public BankResponse processPayment(BankRequest request) {
            var seed = _randomSeed.Generate();
            var transactionId = new DateTimeOffset().ToUnixTimeMilliseconds().ToString();
            
            // very dangerous, don't use it on production :D
            if (seed  < 7 || request.CardNumber == "0000-0000-0000-0000") {
                return new BankResponse {
                    id = transactionId,
                    successful = true,
                    message = "successful transaction",
                    statusCode = 1
                };
            }

            if (seed < 9) {
                return new BankResponse {
                    id = transactionId,
                    successful = false,
                    message = "not enough credit to perform transaction",
                    statusCode = 3
                };
            }

            throw new System.Exception("Bank Service Unavailable");
        }
    }
} 