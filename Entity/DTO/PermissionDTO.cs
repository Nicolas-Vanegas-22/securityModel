namespace Entity.DTO
{
    public class PermissionDTO
    {
        public int PermissionId { get; set; }
        public required string PermissionName { get; set; }
        public string? Description { get; set; }
    }
}