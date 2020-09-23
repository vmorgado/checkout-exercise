using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dotnetexample.Logging
{
    public class LogEntry
    {   
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string level { get; set; }
        public string traceId { get; set; } 
        // is this like 'any' in typescript?
        public dynamic payload { get; set; }
        public string logType { get; set; }
    }
}
