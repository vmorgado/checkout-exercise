namespace dotnetexample.Exception
{
    public class AcquiringBankNotAvailable : System.Exception
    {
        public AcquiringBankNotAvailable(string message) : base (message) {}
    }
}