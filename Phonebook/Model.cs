namespace Phonebook;

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; }
    public long PhoneNumber { get; set; }
    public string? Email { get; set; }

    public Contact GetDeepClone() => new() { Id = Id, Name = Name, PhoneNumber = PhoneNumber, Email = Email };
}