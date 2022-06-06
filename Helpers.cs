using System.Globalization;
using ConsoleTableExt;

namespace PhoneBook
{
    public class Helpers
    {
        public static string Message = @"
# Welcome to your PhoneBook!
  Type 'help' to show this message again.
 * exit or 0: stop the program
 * show [optional: sort-descending]: display all contacts
 * add [name] [phone number]: create a new contact
 * update [id] [new name or new number]: edit an existing contact
 * search [search term]: search in your phonebook
 * remove [id or name]: delete a contact
";

        public static string AddErrorMessage = @"
add commands should be in the form 'add [name] [phone number]'.
Example: 'add Emergency 911' creates a contact with the name 'Emergency' with the number '911'.";

        public static string UpdateStringSplitErrorMessage = @"
update commands should be in the form 'update [id] [new name or new number]'.
Example: 'update 2 John' changes the contact name of the second record to 'John'";

        public static string RemoveErrorMessage = @"
remove commands should be in the form 'remove [id or name].
Example: 'remove Kyle' removes Kyle from your phonebook.";

        public static string SearchErrorMessage = "An unexpected error occurred while searching for your contact.";

        public static string CreateErrorMessage = "An unknown error occurred while adding {0} to your contacts.";

        public static string ReadErrorMessage = "An unknown error occurred while fetching your contacts.";

        public static string UpdateErrorMessage = "An unknown error occurred while updating {0}.";

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

        public static void DisplayContactsAsTable(List<Contact> contacts)
        {
            if (contacts.Count == 0)
            {
                Console.WriteLine("There are no contacts currently saved. Type 'help' to learn how to create a new contact.");
                return;
            }

            for (int i = 0; i < contacts.Count; i++) contacts[i].Id = i + 1;

            ConsoleTableBuilder.From(contacts)
                .WithCharMapDefinition(CharMapDefinition.FramePipDefinition, HeaderCharacterMap)
                .ExportAndWriteLine();
        }

        public static (bool, long) IsNumber(object Expression)
        {
            long number;
            bool isNum = long.TryParse(
                Convert.ToString(Expression),
                NumberStyles.Any,
                NumberFormatInfo.InvariantInfo,
                out number);

            return (isNum, number);
        }

        public static (string?, string?) SplitString(string inputString, string errorMessage = "Invalid format. Please check 'help' for more info.")
        {
            try
            {
                string[] outputString = inputString.Trim().Split();
                return (outputString[1], outputString[2]);
            }

            catch (FormatException)
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