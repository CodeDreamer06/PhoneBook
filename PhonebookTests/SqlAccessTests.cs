using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Phonebook;

namespace PhonebookTests
{
    public class SqlAccessTests
    {
        private SqlAccess? _db;

        [SetUp]
        public void Init()
        {
            var contextOptions = new DbContextOptionsBuilder<PhonebookContext>()
                .UseSqlServer("Data Source=ABHI;Initial Catalog=phonebook;Integrated Security=True")
                .Options;

            var context = new PhonebookContext(contextOptions);
            _db = new SqlAccess(context);
        }

        [Test]
        public void Create_WithoutEmail_DbRowIsAdded()
        {
            var contact = new Contact { Name = "Abhinav", PhoneNumber = 7969630205 };
            _db?.Create(contact);

            Assert.AreEqual(contact, _db?.GetLastContact()!);
            _db?.Delete(contact.Name);
        }

        [Test]
        public void Create_WithEmail_DbRowIsAdded()
        {
            var contact = new Contact { Name = "Abhinav", PhoneNumber = 7969630205, Email = "Abhinav06@gmail.com" };
            _db?.Create(contact);

            Assert.AreEqual(contact, _db?.GetLastContact()!);
            _db?.Delete(contact.Name);
        }

        [Test]
        public void Update_ChangeName_DbRowIsUpdated()
        {
            var contact = new Contact { Name = "Robert", PhoneNumber = 8106852006 };
            _db?.Create(contact);

            int relativeId = (int)_db?.GetRelativeId(contact.Name)!;
            _db?.Update(relativeId, "John");

            Assert.AreEqual(contact.Name, _db?.GetLastContact()!.Name);
            _db?.Delete(contact.Name);
        }

        [Test]
        public void Update_ChangePhoneNumber_DbRowIsUpdated()
        {
            var contact = new Contact { Name = "James", PhoneNumber = 8320344831 };
            _db?.Create(contact);

            int relativeId = (int)_db?.GetRelativeId(contact.Name)!;
            _db?.Update(relativeId, "8416428549");

            Assert.AreEqual(contact.PhoneNumber, _db?.GetLastContact()!.PhoneNumber);
            _db?.Delete(contact.Name);
        }

        [Test]
        public void Update_ChangeEmail_DbRowIsUpdated()
        {
            var contact = new Contact { Name = "Stephen Paul", PhoneNumber = 7467246466, Email = "Paul1996@gmail.com" };
            _db?.Create(contact);

            int relativeId = (int)_db?.GetRelativeId(contact.Name)!;
            _db?.Update(relativeId, "Stephen1996@gmail.com");

            Assert.AreEqual(contact.Email, _db?.GetLastContact()!.Email);
            _db?.Delete(contact.Name);
        }

        [Test]
        public void Delete_UsingName_DbRowIsRemoved()
        {
            var contact = new Contact { Name = "Daniel", PhoneNumber = 9668382716 };

            _db?.Create(contact);
            _db?.Delete(contact.Name);

            Assert.AreNotEqual(contact.Name, _db?.GetLastContact()!.Name);
        }

        [Test]
        public void Delete_UsingRelativeId_DbRowIsRemoved()
        {
            var contact = new Contact { Name = "Andrew", PhoneNumber = 9870804574 };

            _db?.Create(contact);

            int relativeId = (int)_db?.GetRelativeId(contact.Name)!;

            _db?.Delete(relativeId.ToString());

            Assert.AreNotEqual(contact, _db?.GetLastContact()!);
        }

        [Test]
        public void GetEmail_ValidName_ReturnsEmail()
        {
            var contact = new Contact { Name = "Thomas", PhoneNumber = 9649966698, Email = "thomas96@gmail.com" };

            _db?.Create(contact);

            Assert.AreEqual(_db?.GetLastContact()!.Email, _db?.GetEmail(contact.Name));
            _db?.Delete(contact.Name);
        }
    }
}