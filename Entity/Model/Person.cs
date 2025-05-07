using System;

namespace Entity.Model
{
    public class Person
    {
        public int PersonId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int Document { get; set; }
        public int PhoneNumber { get; set; }
        public required string Email { get; set; }
   }
}