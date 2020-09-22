using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnetexample.Services;
using dotnetexample.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace dotnetexample.Controllers

{
    [ApiController]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService ) {
            _logger = logger;
            _paymentService = paymentService;

            
        }
        
        [HttpGet("{id:length(24)}", Name = "GetPayment")]
        public IActionResult Get(string id)
        {
            var payment = _paymentService.Get(id);

            if (payment == null)
            {
                return NotFound();
            }

            return new ObjectResult(payment);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(CreatePaymentDto payment)
        {
            var response = _paymentService.Create(payment);

            return new ObjectResult(response);
        }
    }
}