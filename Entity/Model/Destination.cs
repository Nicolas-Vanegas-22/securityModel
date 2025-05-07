namespace Entity.Model
{
    public class Destination
    {
        public int DestinationId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? CreateAt { get; set; }

    }
}