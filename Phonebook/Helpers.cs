using ConsoleTableExt;

namespace Phonebook
{
    public static class Helpers
    {
        public const string Message = @"
# Welcome to your PhoneBook!
  Type 'help' to show this message again.
 * exit or 0: stop the program
 * show [optional: sort-descending]: display all contacts
 * add [name], [phone number], [optional: email]: create a new contact
 * update [id], [new name, number or email]: edit an existing contact
 * search [search term]: search in your phonebook
 * remove [id or name]: delete a contact
 * email [name]: email a contact
";

        public const string InitialEmailMessage = @"
Please enter your email and password.
It will be saved, so you don't need to re-enter your credentials again.";

        public const string NoContactsMessage =
            "There are no contacts currently saved. Type 'help' to learn how to create a new contact.";

        public const string AddErrorMessage = @"
add commands should be in the form 'add [name], [phone number], [optional: email].
Example: 'add Emergency Helpline, 911' creates a contact with the name 'Emergency Helpline' with the number '911'.";

        public const string UpdateStringSplitErrorMessage = @"
update commands should be in the form 'update [id], [new name, number or email]'.
Example: 'update 2, John Dalton' changes the contact name of the second record to 'John Dalton'";

        public const string RemoveErrorMessage = @"
remove commands should be in the form 'remove [id or name].
Example: 'remove Kyle' removes Kyle from your phonebook.";

        public const string SearchErrorMessage = "An unexpected error occurred while searching for your contact.";

        public const string CreateErrorMessage = "An unknown error occurred while adding {0} to your contacts.";

        public const string ReadErrorMessage = "An unknown error occurred while fetching your contacts.";

        public const string UpdateErrorMessage = "An unknown error occurred while updating {0}.";

        public const string DeleteErrorMessage = "An unknown error occurred while removing your contact";

        public const string InvalidOperationErrorMessage =
            "Failed to delete {0}, because {0} doesn't exist in your contacts.";

        public const string EmailFormatErrorMessage = @"
email commands should be in the form 'email [names].
Example: 'email kyle, adam' creates an email for kyle and adam.";

        public const string EmailDispatchErrorMessage = "An unknown error occurred whlie sending your email.";

        public const string EmailSuccessMessage = "Successfully sent your email!";

        private static readonly Dictionary<HeaderCharMapPositions, char> HeaderCharacterMap = new()
        {
            { HeaderCharMapPositions.TopLeft, '╒' },
            { HeaderCharMapPositions.TopCenter, '╤' },
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

        public static void DisplayTable<T>(List<T> records, string emptyMessage) where T : class
        {
            if (records.Count == 0)
            {
                Console.WriteLine(emptyMessage);
                return;
            }

            ConsoleTableBuilder.From(records)
                .WithCharMapDefinition(CharMapDefinition.FramePipDefinition, HeaderCharacterMap)
                .ExportAndWriteLine();
        }

        public static (string?, string?, string?) SplitString(string inputString,
            string errorMessage = "Invalid format. Please check 'help' for more info.")
        {
            try
            {
                string[] outputString = inputString.Trim().Split(',');
                return (outputString[0].Trim(), outputString[1].Trim(),
                    outputString.Length > 2 ? outputString[2].Trim() : null);
            }

            catch
            {
                Console.WriteLine(errorMessage);
                return (null, null, null);
            }
        }

        public static string CorrectSpelling(string command)
        {
            var definitions = new List<string> { "exit", "help", "show", "add", "update", "remove", "email" };
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
                    }

                    ;
                }

                return maxPercentage > 40 ? $"Did you mean {correctDefinition}?" : "";
            }

            catch
            {
                return "";
            }
        }

        public static List<Contact> GetSuggestions(string searchTerm, IEnumerable<Contact> contacts)
        {
            return (from contact in contacts
                let matchPercentage = FuzzySharp.Fuzz.PartialRatio(searchTerm, contact.Name)
                where matchPercentage > 40
                select contact).ToList();
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
    }
}