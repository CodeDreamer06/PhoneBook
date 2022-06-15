using System.Globalization;
using System.Text.RegularExpressions;
using ConsoleTableExt;
using ExtensionMethods;

namespace Phonebook
{
    public class Helpers
    {
        public static string Message = @"
# Welcome to your PhoneBook!
  Type 'help' to show this message again.
 * exit or 0: stop the program
 * show [optional: sort-descending]: display all contacts
 * add [name], [phone number], [optional: email]: create a new contact
 * update [id], [new name, email or number]: edit an existing contact
 * search [search term]: search in your phonebook
 * remove [id or name]: delete a contact
 * email [names]: email a contact
";
        public static string InitialEmailMessage = @"
Please enter your email and password.
It will be saved, so you don't need to re-enter your credentials again.";

        public static string NoContactsMessage = "There are no contacts currently saved. Type 'help' to learn how to create a new contact.";

        public static string AddErrorMessage = @"
add commands should be in the form 'add [name], [phone number], [optional: email].
Example: 'add Emergency Helpline, 911' creates a contact with the name 'Emergency Helpline' with the number '911'.";

        public static string UpdateStringSplitErrorMessage = @"
update commands should be in the form 'update [id], [new name, email or number]'.
Example: 'update 2, John Dalton' changes the contact name of the second record to 'John Dalton'";

        public static string RemoveErrorMessage = @"
remove commands should be in the form 'remove [id or name].
Example: 'remove Kyle' removes Kyle from your phonebook.";

        public static string SearchErrorMessage = "An unexpected error occurred while searching for your contact.";

        public static string CreateErrorMessage = "An unknown error occurred while adding {0} to your contacts.";

        public static string ReadErrorMessage = "An unknown error occurred while fetching your contacts.";

        public static string UpdateErrorMessage = "An unknown error occurred while updating {0}.";

        public static string DeleteErrorMessage = "An unknown error occurred while removing your contact";

        public static string InvalidOperationErrorMessage = "Failed to delete {0}, because {0} doesn't exist in your contacts.";

        public static string EmailFormatErrorMessage = @"
email commands should be in the form 'email [names].
Example: 'email kyle, adam' creates an email for kyle and adam.";

        public static string EmailDispatchErrorMessage = "An unknown error occurred whlie sending your email.";

        public static Dictionary<HeaderCharMapPositions, char> HeaderCharacterMap = new Dictionary<HeaderCharMapPositions, char> {
                        {HeaderCharMapPositions.TopLeft, '╒' },
                        {HeaderCharMapPositions.TopCenter, '╤' },
                        { HeaderCharMapPositions.TopRight, '╕' },
                        { HeaderCharMapPositions.BottomLeft, '╞' },
                        { HeaderCharMapPositions.BottomCenter, '╪' },
                        { HeaderCharMapPositions.BottomRight, '╡' },
                        { HeaderCharMapPositions.BorderTop, '═' },
                        { HeaderCharMapPositions.BorderRight, '│' },
                        { HeaderCharMapPositions.BorderBottom, '═' },
                        { HeaderCharMapPositions.BorderLeft, '│' },
                        { HeaderCharMapPositions.Divider, '│' },
                    };

        public static string EmailRegex = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

        public static void DisplayContactsAsTable(List<Contact> contacts)
        {
            if (contacts.Count == 0)
            {
                Console.WriteLine(NoContactsMessage);
                return;
            }

            List<Contact> contactsClone = contacts.ConvertAll(contact => contact.GetDeepClone());

            for (int i = 0; i < contactsClone.Count; i++) contactsClone[i].Id = ++i;

            ConsoleTableBuilder.From(contactsClone)
                .WithCharMapDefinition(CharMapDefinition.FramePipDefinition, HeaderCharacterMap)
                .ExportAndWriteLine();
        }

        public static (bool, long) IsNumber(object testObject)
        {
            long number;
            bool isNum = long.TryParse(
                Convert.ToString(testObject),
                NumberStyles.Any,
                NumberFormatInfo.InvariantInfo,
                out number);

            return (isNum, number);
        }

        public static bool IsEmailValid(string email)
        {
            return Regex.IsMatch(email, EmailRegex, RegexOptions.IgnoreCase);
        }

        public static (string?, string?) SplitString(string inputString, string errorMessage = "Invalid format. Please check 'help' for more info.")
        {
            try
            {
                string[] outputString = inputString.Trim().Split(',');
                return (outputString[0].Trim(), outputString[1].Trim());
            }

            catch
            {
                Console.WriteLine(errorMessage);
                return (null, null);
            }
        }

        public static string CorrectSpelling(string command)
        {
            var definitions = new List<string> { "exit", "help", "show", "add", "update", "remove" };
            string correctDefinition = "";
            int maxPercentage = 0;

            try
            {
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

            catch
            {
                return "";
            }
        }

        public static List<Contact> GetSuggestions(string searchTerm, List<Contact> contacts)
        {
            var suggestedContacts = new List<Contact>();

            foreach (var contact in contacts)
            {
                int matchPercentage = FuzzySharp.Fuzz.PartialRatio(searchTerm, contact.Name);
                if (matchPercentage > 40) suggestedContacts.Add(contact);
            }

            return suggestedContacts;
        }
    }
}

namespace ExtensionMethods
{
    public static class Extensions
    {
        public static string RemoveKeyword(this string str, string keyword)
        {
            return str.Replace(keyword, "");
        }

        public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> handler)
        {
            int index = 0;
            foreach (T item in enumerable)
                handler(item, index++);
        }
    }
}