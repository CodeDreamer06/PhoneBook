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

                Helpers.DisplayContactsAsTable(contacts, new List<string> { "Id", "Name", "Phone" });
            }
        }

        public static dynamic Update(int id, string contactProperty)
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();

                var contact = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .Where(contact => contact.Id == id).First();

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

                var (isNumber, id) = Helpers.IsNumber(contactProperty);

                var contact = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .Where(contact => isNumber && id > 0 ?
                            contact.Id == id : contact.Name == contactProperty).First();

                db.Remove(contact);

                db.SaveChanges();

                return contact.Name;
            }
        }
    }
}
