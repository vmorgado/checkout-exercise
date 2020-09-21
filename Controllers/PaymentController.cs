using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dotnetexample.Services;
using dotnetexample.Models;
using System.Collections.Generic;

namespace dotnetexample.Controllers

{
    [ApiController]
    [Route("payment/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        public PaymentController(ILogger<PaymentController> logger, PaymentService paymentService) {
            _logger = logger;
            _paymentService = paymentService;
        }

        [HttpGet]
        public ActionResult<List<PaymentModel>> Get() =>
            _paymentService.Get();

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
        public ActionResult<PaymentModel> Create(PaymentModel payment)
        {
            _paymentService.Create(payment);

            return CreatedAtRoute("GetPayment", new { id = payment.Id.ToString() }, payment);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id, PaymentModel paymentIn)
        {
            var payment = _paymentService.Get(id);

            if (payment == null)
            {
                return NotFound();
            }

            _paymentService.Update(id, paymentIn);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var payment = _paymentService.Get(id);

            if (payment == null)
            {
                return NotFound();
            }

            _paymentService.Remove(payment.Id);

            return NoContent();
        }
    }
}