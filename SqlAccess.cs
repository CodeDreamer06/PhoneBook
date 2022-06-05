using Microsoft.EntityFrameworkCore;

namespace PhoneBook
{
    public static class SqlAccess
    {
        public class PhoneBookContext : DbContext
        {
            public DbSet<Contact> Contacts => Set<Contact>();

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Contact>(entity =>
                {
                    entity.Property(entity => entity.Id).HasColumnName("Id");
                    entity.Property(entity => entity.Name).HasMaxLength(50).IsUnicode(false);
                    entity.Property(entity => entity.PhoneNumber).HasMaxLength(10).IsUnicode(false);
                });
            }
        }

        public static void Create(Contact contact)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();
                db.Contacts.Add(contact);
                db.SaveChanges();
            }
        }

        public static void Read(bool sortByDescendingOrder = false)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();
                var contacts = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .ToList();

                if (sortByDescendingOrder) contacts.Reverse();

                Helpers.DisplayContactsAsTable(contacts);
            }
        }

        public static dynamic Update(int relativeId, string contactProperty)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();

                var contact = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .ToList()[(int) relativeId - 1];

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

                db.SaveChanges();

                return changedProperty;
            }
        }

        public static string Delete(string contactProperty)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();

                var (isNumber, relativeId) = Helpers.IsNumber(contactProperty);

                var contact = new Contact();

                if (isNumber)
                {
                    contact = db.Contacts
                    .OrderBy(contact => contact.Name).ToList()[(int) relativeId - 1];
                }

                else
                {
                    contact = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .Where(contact => contact.Name == contactProperty)
                    .First();
                }

                db.Remove(contact);

                db.SaveChanges();

                return contact.Name;
            }
        }

        public static void Search(string searchTerm)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();
                var contacts = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .ToList();

                var suggestedContacts = Helpers.GetSuggestions(searchTerm, contacts);

                Helpers.DisplayContactsAsTable(suggestedContacts);
            }
        }
    }
}
