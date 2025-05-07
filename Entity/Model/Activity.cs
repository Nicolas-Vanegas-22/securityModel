using System;

namespace Entity.Model
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public TimeSpan DurationHours { get; set; } = TimeSpan.Zero; // Fixed initialization
        public DateTime? DeleteAt { get; set; }
        public DateTime? CreateAt { get; set; }
    }

}