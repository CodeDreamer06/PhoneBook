using System;
using Microsoft.EntityFrameworkCore;
using Phonebook;

namespace PhonebookTests
{
    public class TestDbContext : DbContext
    {
        public DbSet<Contact> Contacts => Set<Contact>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString);
            }

            catch (Exception ex)
            {
                Console.WriteLine("An unknown error occurred while creating the database. Please make sure SQL server is running.");
                Console.WriteLine(ex.Message);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(entity => entity.Id).HasColumnName("Id");
                entity.Property(entity => entity.Name).HasMaxLength(50).IsUnicode(false);
                entity.Property(entity => entity.PhoneNumber).HasMaxLength(10).IsUnicode(false);
                entity.Property(entity => entity.Email).HasMaxLength(320).IsUnicode(false);
            });
        }
    }
}
