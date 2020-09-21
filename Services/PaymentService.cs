
using dotnetexample.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace dotnetexample.Services
{
    public class PaymentService
    {
        private readonly IMongoCollection<PaymentModel> _paymentModels;

        public PaymentService(IPaymentDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _paymentModels = database.GetCollection<PaymentModel>(settings.PaymentCollectionName);
        }

        public List<PaymentModel> Get() =>
            _paymentModels.Find(PaymentModel => true).ToList();

        public PaymentModel Get(string id) =>
            _paymentModels.Find<PaymentModel>(PaymentModel => PaymentModel.Id == id).FirstOrDefault();

        public PaymentModel Create(PaymentModel PaymentModel)
        {
            _paymentModels.InsertOne(PaymentModel);
            return PaymentModel;
        }

        public void Update(string id, PaymentModel PaymentModelIn) =>
            _paymentModels.ReplaceOne(PaymentModel => PaymentModel.Id == id, PaymentModelIn);

        public void Remove(PaymentModel PaymentModelIn) =>
            _paymentModels.DeleteOne(PaymentModel => PaymentModel.Id == PaymentModelIn.Id);

        public void Remove(string id) => 
            _paymentModels.DeleteOne(PaymentModel => PaymentModel.Id == id);        
    }
}