namespace Phonebook
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long PhoneNumber { get; set; }
        public string? Email { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Phone number: {PhoneNumber}" + (Email is null? "" : $", {Email}");
        }
    }
}