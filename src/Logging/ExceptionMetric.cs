using System;

namespace dotnetexample.Logging
{
    public class ExceptionMetric
    {
        public string origin { get; set; }
        public System.Exception exception { get; set; }
        public DateTime time { get; set; }
        public String stack { get; set; }
    }
}