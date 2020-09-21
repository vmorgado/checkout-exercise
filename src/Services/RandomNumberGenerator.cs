using System;

namespace dotnetexample.Services
{
    public class RandomNumberGenerator: IRandomNumberGenerator
    {
        private readonly Random _random = new Random();
        public int Generate() {
            return this._random.Next(0, 10);
        }
    }

    public interface IRandomNumberGenerator {
        int Generate();
    }
}