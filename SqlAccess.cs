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

        public static void Read()
        {
            using (var db = new PhoneBookContext())
            {
                db.Database.EnsureCreated();
                var contacts = db.Contacts
                    .OrderBy(contact => contact.Name)
                    .ToList();

                Helpers.DisplayContactsAsTable(contacts, new List<string> { "Id", "Name", "Phone Number" });
            }
        }
    }
}
