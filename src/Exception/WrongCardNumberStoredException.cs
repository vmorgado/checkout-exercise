namespace dotnetexample.Exception
{
    public class WrongCardNumberStoredException : System.Exception
    {
        public WrongCardNumberStoredException(string message) : base (message) { }
    }
}