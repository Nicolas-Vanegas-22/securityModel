namespace Entity.Model
{
    public class Module
    {
        public int ModuleId { get; set; }
        public int Code { get; set; }
        public required string Name { get; set; }
        public bool Active { get; set; }
        public string? DeleteAt { get; set; }  
        public string? CreateAt { get; set; }
    }
}