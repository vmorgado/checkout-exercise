using dotnetexample.Models;

namespace dotnetexample.Services
{
    public interface IPaymentService
    {
         PaymentModel Get(string id);
         PaymentResponse Create(CreatePaymentDto createPaymentDto);
    }
}