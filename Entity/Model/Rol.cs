namespace Entity.Model
{
    public class Rol
    {
        public int RolId { get; set; }
        public string? RolName { get; set; }
        public string? Description { get; set; }
        public DateTime? DeleteAt { get; set; }
        public DateTime? CreateAt { get; set; }

    }
}