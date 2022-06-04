using System.Reflection;
using System.Globalization;

namespace PhoneBook
{
    public class Helpers
    {
        public static string message = @"
# Welcome to PhoneBook!
  A simple phone number manager to remember the phone numbers of your friends and other people!
 * exit or 0: stop the program
 * show [optional: sort-descending]: display all contacts
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

            int longestNameLength = contacts.Max(contact => contact.Name.Length);
            string border = new String('-', longestNameLength + 19);

            Console.WriteLine(border);

            foreach (String header in headers)
            {
                int headerPadLength = header == "Name" ? longestNameLength - 1 : header == "Phone" ? 10 : 0;
                Console.Write(header.PadRight(headerPadLength) + " | ");
            }

            Console.WriteLine();

            Console.WriteLine(border);

            for (int i = 0; i < contacts.Count(); i++)
            {
                foreach (PropertyInfo contact in contacts[i].GetType().GetProperties())
                {
                    var tableItem = contact.GetValue(contacts[i], null);

                    if(tableItem!.GetType() == typeof(string))
                    {
                        Console.Write(tableItem.ToString()!.PadRight(longestNameLength) + " | ");
                    }

                    else
                    {
                        Console.Write(tableItem + " | ");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine(border);
        }

        public static (bool, long) IsNumber(object Expression)
        {
            long number;

            bool isNum = long.TryParse(Convert.ToString(Expression), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out number);

            return (isNum, number);
        }

        public static (string, string) SplitString(string inputString)
        {
            var outputString = inputString.Trim().Split();
            return (outputString[1], outputString[2]);
        }

        public static string CorrectSpelling(string command)
        {
            var definitions = new List<string> { "exit", "help", "show", "add", "update", "remove" };
            string correctDefinition = "";
            int maxPercentage = 0;

            foreach (var definition in definitions)
            {
                int matchPercentage = FuzzySharp.Fuzz.Ratio(command, definition);

                if (matchPercentage > maxPercentage)
                {
                    maxPercentage = matchPercentage;
                    correctDefinition = definition;
                };
            }

            return maxPercentage > 40 ? $"Did you mean {correctDefinition}?" : "";
        }
    }
}