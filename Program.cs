namespace PhoneBook
{
    class Program
    {
        private static string command = "";

        static void Main(string[] args)
        {
            Console.WriteLine(Helpers.message);

            while (true)
            {
                var rawCommand = Console.ReadLine()!;
                command = rawCommand.Trim().ToLower();

                if (command == "exit" || command == "0") break;

                else if (command == "help") Console.WriteLine(Helpers.message);

                else if (string.IsNullOrWhiteSpace(command)) continue;

                else if (command.StartsWith("show "))
                {
                    SqlAccess.Read(command.Contains("sort-descending"));
                }

                else if (command.StartsWith("add "))
                {
                    var (name, phoneNumber) = Helpers.SplitString(rawCommand);
                    SqlAccess.Create(new Contact { Name = name, PhoneNumber = Helpers.IsNumber(phoneNumber).Item2 });
                    Console.WriteLine($"Successfully added {name}!");
                }

                else if (command.StartsWith("update "))
                {
                    var (id, contactProperty) = Helpers.SplitString(rawCommand);
                    var oldProperty = SqlAccess.Update((int) Helpers.IsNumber(id).Item2, contactProperty);
                    Console.WriteLine($"Successfully changed {oldProperty} to {contactProperty}!");
                }

                else if(command.StartsWith("remove "))
                {
                    string name = SqlAccess.Delete(rawCommand.Replace("remove", "").Trim());
                    Console.WriteLine($"Successfully removed {name}!");
                }

                else
                {
                    string suggestion = Helpers.CorrectSpelling(command.Split()[0]);
                    Console.WriteLine($"Not a command. Type 'help' for the list of all commands. {suggestion}");
                }
            }
        }
    }
}