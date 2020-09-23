using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnetexample.Services;
using dotnetexample.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using dotnetexample.Logging;

namespace dotnetexample.Controllers

{
    [ApiController]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILoggerService _logger;
        public PaymentController(ILoggerService logger, IPaymentService paymentService ) {
            _logger = logger;
            _paymentService = paymentService;
        }
        
        [HttpGet("{id:length(24)}", Name = "GetPayment")]
        public ActionResult<PaymentModel> Get(string id)
        {
            var payment = _paymentService.Get(id);

            if (payment == null)
            {
                return NotFound();
            }

            return payment;
        }

        [HttpPost]
        [Authorize]
        public ActionResult<PaymentResponse> Create(CreatePaymentDto payment)
        {
            var response = _paymentService.Create(payment);

            return response;
        }
    }
}