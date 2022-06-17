using Microsoft.EntityFrameworkCore;
using ExtensionMethods;

namespace Phonebook;

public static class UserInput
{
    internal static void ShowMenu()
    {
        var contextOptions = new DbContextOptionsBuilder<PhonebookContext>()
            .UseSqlServer(System.Configuration.ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString)
            .Options;

        var context = new PhonebookContext(contextOptions);
        var db = new SqlAccess(context);

        while (true)
        {
            var rawCommand = Console.ReadLine()!;
            var command = rawCommand.Trim().ToLower();

            if (command is "exit" or "0") break;

            if (command == "help") Console.WriteLine(Helpers.Message);

            else if (string.IsNullOrWhiteSpace(command)) continue;

            else if (command.StartsWith("show"))
            {
                db.Read(command.Contains("sort-descending"));
            }

            else if (command.StartsWith("add "))
            {
                var (name, phoneNumber, email) =
                    Helpers.SplitString(rawCommand.RemoveKeyword("add "), Helpers.AddErrorMessage);

                if (Validation.IsContactValid(name, phoneNumber, email))
                    db.Create(new Contact
                        { Name = name!, PhoneNumber = Validation.IsNumber(phoneNumber!).Item2, Email = email });
            }

            else if (command.StartsWith("update "))
            {
                var (id, contactProperty, _) = Helpers.SplitString(rawCommand.RemoveKeyword("update "),
                    Helpers.UpdateStringSplitErrorMessage);

                if (contactProperty is null) continue;

                db.Update((int)Validation.IsNumber(id!).Item2, contactProperty);
            }

            else if (command.StartsWith("remove "))
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
                    db.Delete(contactProperty);
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

            else if (command.StartsWith("search "))
            {
                var suggestedContacts = db.Search(rawCommand.RemoveKeyword("search").Trim());
                Helpers.DisplayTable(suggestedContacts, "No results found.");
            }

            else if (command.StartsWith("email "))
            {
                try
                {
                    var receiver = rawCommand.RemoveKeyword("email").Trim();

                    try
                    {
                        receiver = db.GetEmail(receiver)!;
                    }

                    catch
                    {
                        Console.WriteLine($"{receiver} doesn't exist in your contacts.");
                    }

                    var emailService = new EmailService();

                    if (emailService.IsAccountNull())
                    {
                        Console.WriteLine(Helpers.InitialEmailMessage);
                        string email = "", password = "";


                        Console.Write("Your Email: ");
                        email = Console.ReadLine()!;


                        Console.Write("Password: ");
                        password = Console.ReadLine()!;

                        try
                        {
                            emailService.SetEmailConfig(email, password);
                        }

                        catch (FormatException)
                        {
                            Console.WriteLine("Please enter a valid email.");
                        }
                    }

                    Console.Write("Subject: ");
                    string subject = Console.ReadLine()!;

                    Console.Write("Email Body: ");
                    string body = Console.ReadLine()!;

                    emailService.SendEmail(receiver, subject, body);
                    Console.WriteLine(Helpers.EmailSuccessMessage);
                }

                catch (FormatException)
                {
                    Console.WriteLine(Helpers.EmailFormatErrorMessage);
                }
            }

            else
            {
                string suggestion = Helpers.CorrectSpelling(command.Split()[0]);
                Console.WriteLine($"Not a command. Type 'help' for the list of all commands. {suggestion}");
            }
        }

        context.Dispose();
    }
}