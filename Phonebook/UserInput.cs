using ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace Phonebook
{
    public static class UserInput
    {
        private static string _command = "";
        private static SqlAccess? _db;

        internal static void ShowMenu()
        {
            var contextOptions = new DbContextOptionsBuilder<PhonebookContext>()
                .UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString)
                .Options;

            var context = new PhonebookContext(contextOptions);
            _db = new SqlAccess(context);

            while (true)
            {
                var rawCommand = Console.ReadLine()!;
                _command = rawCommand.Trim().ToLower();

                if (_command is "exit" or "0") break;

                else if (_command == "help") Console.WriteLine(Helpers.Message);

                else if (string.IsNullOrWhiteSpace(_command)) continue;

                else if (_command.StartsWith("show"))
                {
                    _db.Read(_command.Contains("sort-descending"));
                }

                else if (_command.StartsWith("add "))
                {
                    var (name, phoneNumber) =
                        Helpers.SplitString(rawCommand.RemoveKeyword("add "), Helpers.AddErrorMessage);

                    if (name == null || phoneNumber == null) continue;

                    _db.Create(new Contact { Name = name, PhoneNumber = Helpers.IsNumber(phoneNumber).Item2 });
                }

                else if (_command.StartsWith("update "))
                {
                    var (id, contactProperty) = Helpers.SplitString(rawCommand.RemoveKeyword("update "),
                        Helpers.UpdateStringSplitErrorMessage);

                    if (id == null || contactProperty == null) continue;

                    _db.Update((int)Helpers.IsNumber(id).Item2, contactProperty);
                }

                else if (_command.StartsWith("remove "))
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
                        _db.Delete(contactProperty);
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

                else if (_command.StartsWith("search "))
                {
                    _db.Search(rawCommand.RemoveKeyword("search").Trim());
                }

                else if (_command.StartsWith("email "))
                {
                    try
                    {
                        var receivers = rawCommand.RemoveKeyword("email").Trim().Split(',');
                        var emailService = new EmailService(receivers);

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
                    }
                }

                else
                {
                    string suggestion = Helpers.CorrectSpelling(_command.Split()[0]);
                    Console.WriteLine($"Not a command. Type 'help' for the list of all commands. {suggestion}");
                }
            }

            context.Dispose();
        }
    }
}