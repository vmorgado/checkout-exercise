using System;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using dotnetexample.Models;
using dotnetexample.Services;
using dotnetexample.Logging;
using Microsoft.Extensions.Options;
using dotnetexample.Authentication;
using System.Text.Encodings.Web;
using NSwag;

namespace dotnetexample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   

            // AUTHENTICATION AND AUTHORIZATION
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
            }).AddApiKeySupport(options => {});
            services.AddAuthorization(options => {});
            services.AddSingleton<IGetApiKeyQuery, InMemoryGetApiKeyQuery>();

            // MONGO DB COLLECTIONS
            services.Configure<PaymentDatabaseSettings>(Configuration.GetSection(nameof(PaymentDatabaseSettings)));
            services.Configure<LoggingDatabaseSettings>(Configuration.GetSection(nameof(LoggingDatabaseSettings)));
            services.AddSingleton<PaymentDatabaseSettings>(sp => { 
                var value = sp.GetRequiredService<IOptions<PaymentDatabaseSettings>>().Value;
                return value;
            });
            services.AddSingleton<LoggingDatabaseSettings>(sp => { 
                var value = sp.GetRequiredService<IOptions<LoggingDatabaseSettings>>().Value;
                return value;
            });
            // Initializing the Mongo Client at this time should improve performance and make it mockable on tests
            services.AddSingleton<IMongoCollection<PaymentModel>>( sp => {
                
                var settings = sp.GetRequiredService<PaymentDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<PaymentModel>(settings.CollectionName);
            });
            // this mongo blocks of code can be refactored into more efficient and readable code
            services.AddSingleton<IMongoCollection<LogEntry>>( sp => {
                
                var settings = sp.GetRequiredService<LoggingDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<LogEntry>(settings.CollectionName);
            });

            // LOGGING SERVICE N STUFF

            services.AddSingleton<ILoggerService>( sp => new LoggerService(sp.GetRequiredService<IMongoCollection<LogEntry>>()));
            // services.AddSingleton<RequestLogginggMiddleware>( sp => new RequestLogginggMiddleware( sp.GetRequiredService<LoggerService>()));


            // THIS IS THE REAL BUSINESS LOGIC :D :D
            var randomSeed = new RandomNumberGenerator();
            services.AddSingleton<IAcquiringBank>(new MockedAcquiringBank(randomSeed));
            services.AddSingleton<IPaymentService>(
                sp => new PaymentService(sp.GetRequiredService<IMongoCollection<PaymentModel>>(), sp.GetRequiredService<IAcquiringBank>(), sp.GetRequiredService<ILoggerService>())
            );


            // SWAGGER AND CONTROLLERS
            services.AddSwaggerDocument( document => {
                document.Description = "payment hub";
                document.Title = "checkout.com payment hub";

                var securityScheme = new OpenApiSecurityScheme {
                    Description = "Api key needed to access the endpoints. X-Api-Key: My_API_Key",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Name = "X-Api-Key",
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    
                };

                document.AddSecurity( "X-Api-Key", new string[] {"payment"}, securityScheme);
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseCors((options) => {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
            });

            app.UseRouting();

            // var loggerService = app.ApplicationServices.GetRequiredService<LoggerService>();

            app.UseMiddleware<RequestLogginggMiddleware>();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3( c => c.WithCredentials = true );  

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
 