namespace PhoneBook
{
    class Program
    {
        private static string command = "";

        static void Main(string[] args)
        {
            Console.WriteLine(Helpers.Message);

            while (true)
            {
                var rawCommand = Console.ReadLine()!;
                command = rawCommand.Trim().ToLower();

                if (command == "exit" || command == "0") break;

                else if (command == "help") Console.WriteLine(Helpers.Message);

                else if (string.IsNullOrWhiteSpace(command)) continue;

                else if (command.StartsWith("show"))
                {
                    SqlAccess.Read(command.Contains("sort-descending"));
                }

                else if (command.StartsWith("add "))
                {
                    var (name, phoneNumber) = Helpers.SplitString(rawCommand, Helpers.AddErrorMessage);

                    if (name == null || phoneNumber == null) continue;

                    SqlAccess.Create(new Contact { Name = name, PhoneNumber = Helpers.IsNumber(phoneNumber).Item2 });
                }

                else if (command.StartsWith("update "))
                {
                    var (id, contactProperty) = Helpers.SplitString(rawCommand, Helpers.UpdateStringSplitErrorMessage);

                    if (id == null || contactProperty == null) continue;

                    SqlAccess.Update((int) Helpers.IsNumber(id).Item2, contactProperty);
                }

                else if(command.StartsWith("remove "))
                {
                    string contactProperty;

                    try
                    {
                        contactProperty = rawCommand.Replace("remove", "").Trim();
                    }

                    catch
                    {
                        Console.WriteLine(Helpers.RemoveErrorMessage);
                        continue;
                    }

                    Console.WriteLine($"Are you sure you want to remove {contactProperty}? True/False");

                    bool cancelled = true;

                    try
                    {
                        cancelled = !bool.Parse(Console.ReadLine()!);
                    }

                    catch
                    {
                        Console.WriteLine("Please type either True or False.");
                    }

                    if (cancelled) continue;

                    string name = SqlAccess.Delete(contactProperty);
                    Console.WriteLine($"Successfully removed {name}!");
                }

                else if(command.StartsWith("search "))
                {
                    SqlAccess.Search(rawCommand.Replace("search", "").Trim());
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