using System;

namespace Entity.DTO
{
    public class PersonDTO
    {
        public int PersonId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int Document { get; set; }
        public int PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}