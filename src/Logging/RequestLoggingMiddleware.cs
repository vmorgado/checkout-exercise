using dotnetexample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetexample.Logging
{
    public class RequestLogginggMiddleware
    {
        private ILoggerService _logger;
        private RequestDelegate _next;
        public RequestLogginggMiddleware(RequestDelegate next, ILoggerService logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {

                var request = httpContext.Request;
                var traceId = httpContext.TraceIdentifier;
                if (request.Path.StartsWithSegments(new PathString("/payment")))
                {
                    var stopWatch = Stopwatch.StartNew();
                    var requestTime = DateTime.UtcNow;
                    var requestBodyContent = await ReadRequestBody(request);

                    // original Stream
                    var originalBodyStream = httpContext.Response.Body;


                    using (var responseBody = new MemoryStream())
                    {
                        // works like a streamn
                        var response = httpContext.Response;


                        response.Body = responseBody;
                        await _next(httpContext);
                        stopWatch.Stop();

                        string responseBodyContent = null;
                        responseBodyContent = await ReadResponseBody(response);

                        await responseBody.CopyToAsync(originalBodyStream);

                        await SafeLog(
                            traceId,
                            requestTime,
                            stopWatch.ElapsedMilliseconds,
                            response.StatusCode,
                            request.Method,
                            request.Path,
                            request.QueryString.ToString(),
                            requestBodyContent,
                            responseBodyContent);
                    }
                }
                else
                {
                    await _next(httpContext);
                }
            }
            catch (System.Exception e)
            {
                var exceptionMetric = new ExceptionMetric {
                    origin = nameof(RequestLogginggMiddleware),
                    exception = e,
                    time = new DateTimeOffset().Date,
                    stack = e.StackTrace
                };

                _logger.Log<ExceptionMetric>(LogLevel.Error, new EventId {}, exceptionMetric, e );
                await _next(httpContext);
            }
        }
        private async Task<string> ReadRequestBody(HttpRequest request)
        {
            request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            var bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task<string> ReadResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            return bodyAsText;
        }

        private async Task SafeLog(
                            string traceIdentifier,
                            DateTime requestTime,
                            long responseMillis,
                            int statusCode,
                            string method,
                            string path,
                            string queryString,
                            string requestBody,
                            string responseBody)
        {   

            if (method == "POST" && path == "/payment") {
                
                // when we intercept the payment request we have access to the card data at this point. in this case we should remove it before logging

                var unserializedReqBody = BsonSerializer.Deserialize<CreatePaymentDto>(requestBody);
                unserializedReqBody.CardNumber = "";
                unserializedReqBody.CCV = "";
                unserializedReqBody.ExpiryMonth = 1;
                unserializedReqBody.ExpiryYear = 1999;
                
                requestBody = Newtonsoft.Json.JsonConvert.SerializeObject(unserializedReqBody);
            }

            if (requestBody.Length > 500)
            {
                requestBody = $"(Truncated to 500 chars) {requestBody.Substring(0, 500)}";
            }

            if (responseBody.Length > 500)
            {
                responseBody = $"(Truncated to 500 chars) {responseBody.Substring(0, 500)}";
            }

            if (queryString.Length > 500)
            {
                queryString = $"(Truncated to 500 chars) {queryString.Substring(0, 500)}";
            }

            int manySeconds = unchecked((int)new DateTimeOffset().ToUnixTimeSeconds());
            var eventIdentifier = new EventId(manySeconds, traceIdentifier);

            var requestMetric = new RequestMetric
            {
                RequestTime = requestTime,
                ResponseMillis = responseMillis,
                StatusCode = statusCode,
                Method = method,
                Path = path,
                QueryString = queryString,
                RequestBody = requestBody,
                ResponseBody = responseBody
            };

            _logger.Log<RequestMetric>( LogLevel.Information, eventIdentifier, requestMetric );
        }

    }
}