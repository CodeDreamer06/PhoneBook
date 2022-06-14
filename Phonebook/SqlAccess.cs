using Microsoft.EntityFrameworkCore;

namespace Phonebook
{
    public class PhonebookContext : DbContext
    {
        public DbSet<Contact> Contacts => Set<Contact>();

        public PhonebookContext(DbContextOptions<PhonebookContext> options) : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    try
        //    {
        //        optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"]
        //            .ConnectionString);
        //    }

        //    catch
        //    {
        //        Console.WriteLine(
        //            "An unknown error occurred while creating the database. Please make sure SQL server is running.");
        //    }
        //}

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

                Helpers.DisplayContactsAsTable(contacts);
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
                if (relativeId == 0)
                {
                    Console.WriteLine(Helpers.UpdateStringSplitErrorMessage);
                    return;
                }

                var contact = _db.Contacts
                    .OrderBy(contact => contact.Name)
                    .ToList()[(int)relativeId - 1];

                var (isNumber, phoneNumber) = Helpers.IsNumber(contactProperty);
                dynamic changedProperty;

                if (isNumber)
                {
                    changedProperty = contact.PhoneNumber;
                    contact.PhoneNumber = phoneNumber;
                }
                else
                {
                    changedProperty = contact.Name;
                    contact.Name = contactProperty;
                }

                _db.SaveChanges();

                Console.WriteLine($"Successfully changed {changedProperty} to {contactProperty}!");
            }

            catch (Exception ex)
            {
                Console.WriteLine(Helpers.UpdateErrorMessage, contactProperty);
                Console.WriteLine(ex.Message);
            }
        }

        public void Delete(string contactProperty)
        {
            var (isNumber, relativeId) = Helpers.IsNumber(contactProperty);

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

        public void Search(string searchTerm)
        {
            var contacts = _db.Contacts
                .OrderBy(contact => contact.Name)
                .ToList();

            try
            {
                var suggestedContacts = Helpers.GetSuggestions(searchTerm, contacts);

                Helpers.DisplayContactsAsTable(suggestedContacts);
            }

            catch
            {
                Console.WriteLine(Helpers.SearchErrorMessage);
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
            _db.Contacts.OrderBy(contact => contact.Name).ToList().FindIndex(contact => contact.Name == name) + 1;
    }
}