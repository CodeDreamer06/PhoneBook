using System.Reflection;

namespace PhoneBook
{
    public class Helpers
    {
        public static string message = @"
# Welcome to PhoneBook!
  A simple phone number manager to remember the phone numbers of your friends and other people!
 * exit or 0: stop the program
 * show: display all phone numbers
 * add [name] [phone number]: create a new contact
 * update [id] [new name or new number]: edit an existing contact
 * remove [id or name]: delete a contact
";

        public static void DisplayContactsAsTable(List<Contact> contacts, List<string> headers)
        {
            if (contacts.Count == 0)
            {
                Console.WriteLine("There are no contacts currently saved. Type 'help' to learn how to create a new contact.");
                return;
            }

            int longestNameLength = contacts.Max(contact => contact.Name.Length) + 18;
            string border = new String('-', longestNameLength);

            Console.WriteLine(border);

            foreach (String header in headers) Console.Write(header + " | ");

            Console.WriteLine();

            for (int i = 0; i < contacts.Count(); i++)
            {
                Console.WriteLine(border);

                foreach (PropertyInfo contact in contacts[i].GetType().GetProperties())
                {
                    Console.Write(contact.GetValue(contacts[i], null) + " | ");
                }

                Console.WriteLine();
            }

            Console.WriteLine(border);
        }
    }
}