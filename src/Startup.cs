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
using Microsoft.Extensions.Options;

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

            services.Configure<PaymentDatabaseSettings>(Configuration.GetSection(nameof(PaymentDatabaseSettings)));
            services.AddSingleton<IPaymentDatabaseSettings>(sp => sp.GetRequiredService<IOptions<PaymentDatabaseSettings>>().Value);

            // Initializing the Mongo Client at this time should improve performance and make it mockable on tests
            services.AddSingleton<IMongoCollection<PaymentModel>>( sp => {

                var settings = sp.GetRequiredService<IPaymentDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                var database = client.GetDatabase(settings.DatabaseName);
                return database.GetCollection<PaymentModel>(settings.PaymentCollectionName);
            });
            
            var randomSeed = new RandomNumberGenerator();
            services.AddSingleton<IAcquiringBank>(new MockedAcquiringBank(randomSeed));
            services.AddSingleton<IPaymentService>(
                sp => new PaymentService(sp.GetRequiredService<IMongoCollection<PaymentModel>>(), sp.GetRequiredService<IAcquiringBank>()) {}
            );
            services.AddControllers();
            services.AddSwaggerDocument();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseOpenApi();
            app.UseSwaggerUi3();        

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
 