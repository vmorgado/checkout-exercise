
using dotnetexample.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace dotnetexample.Services
{
    public class PaymentService
    {
        private readonly IMongoCollection<PaymentModel> _paymentModels;
        private readonly IAcquiringBank _acquiringBank;

        public PaymentService(IPaymentDatabaseSettings settings, IAcquiringBank acquiringBank)
        {
            _acquiringBank = acquiringBank;

            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _paymentModels = database.GetCollection<PaymentModel>(settings.PaymentCollectionName);
        }

        public List<PaymentModel> Get() =>
            _paymentModels.Find(PaymentModel => true).ToList();

        public PaymentModel Get(string id) =>
            _paymentModels.Find<PaymentModel>(PaymentModel => PaymentModel.Id == id).FirstOrDefault();

        public PaymentModel Create(PaymentModel paymentModel)
        {
            _paymentModels.InsertOne(paymentModel);
            _acquiringBank.processPayment();
            return paymentModel;
        }

        public void Update(string id, PaymentModel paymentModelIn) =>
            _paymentModels.ReplaceOne(paymentModel => paymentModel.Id == id, paymentModelIn);

        public void Remove(PaymentModel paymentModelIn) =>
            _paymentModels.DeleteOne(paymentModel => paymentModel.Id == paymentModelIn.Id);

        public void Remove(string id) => 
            _paymentModels.DeleteOne(paymentModel => paymentModel.Id == id);        
    }
}
