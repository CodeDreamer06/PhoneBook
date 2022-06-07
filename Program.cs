using ExtensionMethods;

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
                    var (name, phoneNumber) = Helpers.SplitString(rawCommand.RemoveKeyword("add "), Helpers.AddErrorMessage);

                    if (name == null || phoneNumber == null) continue;

                    SqlAccess.Create(new Contact { Name = name, PhoneNumber = Helpers.IsNumber(phoneNumber).Item2 });
                }

                else if (command.StartsWith("update "))
                {
                    var (id, contactProperty) = Helpers.SplitString(rawCommand.RemoveKeyword("update "), Helpers.UpdateStringSplitErrorMessage);

                    if (id == null || contactProperty == null) continue;

                    SqlAccess.Update((int) Helpers.IsNumber(id).Item2, contactProperty);
                }

                else if(command.StartsWith("remove "))
                {
                    string contactProperty;

                    try
                    {
                        contactProperty = rawCommand.RemoveKeyword("remove").Trim();
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

                    try
                    {
                        string name = SqlAccess.Delete(contactProperty);
                        Console.WriteLine($"Successfully removed {name}!");
                    }

                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(InvalidOperationException))
                        {
                            Console.WriteLine(Helpers.InvalidOperationErrorMessage, contactProperty);
                        }

                        else
                        {
                            Console.WriteLine(Helpers.DeleteErrorMessage);
                        }
                    }
                }

                else if(command.StartsWith("search "))
                {
                    SqlAccess.Search(rawCommand.RemoveKeyword("search").Trim());
                }

                else if(command.StartsWith("email "))
                {
                    try
                    {
                        var recievers = rawCommand.RemoveKeyword("email").Trim().Split(',');
                        var emailService = new EmailService(recievers);

                        if (EmailService.UserName == null || EmailService.Password == null)
                        {
                            Console.WriteLine(Helpers.InitialEmailMessage);

                            Console.Write("Your Email: ");
                            EmailService.UserName = Console.ReadLine()!;

                            Console.Write("Password: ");
                            EmailService.Password = Console.ReadLine()!;
                        }

                        Console.Write("Subject: ");
                        string subject = Console.ReadLine()!;

                        Console.Write("Email Body: ");
                        string body = Console.ReadLine()!;

                        emailService.SendEmails(subject, body);

                    }
                    
                    catch (FormatException)
                    {
                        Console.WriteLine(Helpers.EmailFormatErrorMessage);
                        continue;
                    }
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