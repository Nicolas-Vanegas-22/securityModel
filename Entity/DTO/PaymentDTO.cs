using System;

namespace Entity.DTO
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public required string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public required string Activity { get; set; }
    }
}

