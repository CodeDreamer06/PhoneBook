using System;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using Phonebook;

namespace PhonebookTests
{
    public class SqlAccessTests
    {
        SqlAccess? _db;

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

            Assert.AreEqual(JsonConvert.SerializeObject(contact), JsonConvert.SerializeObject(_db?.GetLastContact()!));
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

            Assert.AreNotEqual(JsonConvert.SerializeObject(contact),
                JsonConvert.SerializeObject(_db?.GetLastContact()));
        }
    }
}