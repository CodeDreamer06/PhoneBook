using Microsoft.EntityFrameworkCore;

namespace Phonebook;

public class PhonebookContext : DbContext
{
    public DbSet<Contact> Contacts => Set<Contact>();

    public PhonebookContext(DbContextOptions<PhonebookContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Contact>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("Id");
            entity.Property(e => e.Name).HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.PhoneNumber).HasMaxLength(10).IsUnicode(false);
            entity.Property(e => e.Email).HasMaxLength(320).IsUnicode(false);
        });
    }
}

public class SqlAccess
{
    private readonly PhonebookContext _db;

    public SqlAccess(PhonebookContext context)
    {
        _db = context;
        _db.Database.EnsureCreated();
    }

    public void Create(Contact contact)
    {
        try
        {
            _db.Contacts.Add(contact);
            _db.SaveChanges();
            Console.WriteLine($"Successfully added {contact.Name}!");
        }

        catch
        {
            Console.WriteLine(Helpers.CreateErrorMessage, contact.Name);
        }
    }

    public void Read(bool sortByDescendingOrder = false)
    {
        try
        {
            var contacts = _db.Contacts
                .OrderBy(contact => contact.Name)
                .ToList();

            if (sortByDescendingOrder) contacts.Reverse();

            var contactsClone = contacts.ConvertAll(contact => contact.GetDeepClone());

            for (int i = 0; i < contactsClone.Count; i++) contactsClone[i].Id = i + 1;

            Helpers.DisplayTable(contactsClone, Helpers.NoContactsMessage);
        }

        catch
        {
            Console.WriteLine(Helpers.ReadErrorMessage);
        }
    }

    public void Update(int relativeId, string contactProperty)
    {
        try
        {
            if (relativeId is 0)
            {
                Console.WriteLine(Helpers.UpdateStringSplitErrorMessage);
                return;
            }

            var contact = _db.Contacts
                .OrderBy(contact => contact.Name)
                .ToList()[relativeId - 1];

            var (isNumber, phoneNumber) = Validation.IsNumber(contactProperty);
            var isEmail = Validation.IsEmailValid(contactProperty);
            dynamic oldProperty;

            if (isNumber)
            {
                oldProperty = contact.PhoneNumber;
                contact.PhoneNumber = phoneNumber;
            }

            else if (isEmail)
            {
                oldProperty = contact.Email!;
                contact.Email = contactProperty;
            }

            else
            {
                oldProperty = contact.Name;
                contact.Name = contactProperty;
            }

            _db.SaveChanges();

            Console.WriteLine($"Successfully changed {oldProperty} to {contactProperty}!");
        }

        catch
        {
            Console.WriteLine(Helpers.UpdateErrorMessage, contactProperty);
        }
    }

    public void Delete(string contactProperty)
    {
        try
        {
            var (isNumber, relativeId) = Validation.IsNumber(contactProperty);

            Contact contact;

            if (isNumber)
            {
                contact = _db.Contacts
                    .OrderBy(c => c.Name).ToList()[(int)relativeId - 1];
            }

            else
            {
                contact = _db.Contacts
                    .OrderBy(c => c.Name).First(c => c.Name == contactProperty);
            }

            _db.Remove(contact);

            _db.SaveChanges();

            Console.WriteLine($"Successfully removed {contact.Name}!");
        }

        catch (Exception ex)
        {
            if (ex.GetType() == typeof(InvalidOperationException))
            {
                Console.WriteLine(Helpers.InvalidOperationErrorMessage, contactProperty);
            }

            else Console.WriteLine(Helpers.DeleteErrorMessage);
        }
    }

    public List<Contact> Search(string searchTerm)
    {
        var contacts = _db.Contacts
            .OrderBy(contact => contact.Name)
            .ToList();

        try
        {
            return Helpers.GetSuggestions(searchTerm, contacts);
        }

        catch
        {
            Console.WriteLine(Helpers.SearchErrorMessage);
            return new List<Contact>();
        }
    }

    public Contact GetLastContact()
    {
        try
        {
            return _db.Contacts.OrderBy(contact => contact.Id).Last();
        }

        catch
        {
            return new Contact();
        }
    }

    public int GetRelativeId(string name) =>
        _db.Contacts.OrderBy(contact => contact.Name).ToList()
            .FindIndex(contact => contact.Name == name) + 1;

    public string? GetEmail(string name)
    {
        if (!_db.Contacts.Any(contact => contact.Name == name))
        {
            Console.WriteLine($"{name} doesn't exist in your contacts.");
            return null;
        }
        
        return _db.Contacts.First(contact => contact.Name == name).Email!;
    }
}