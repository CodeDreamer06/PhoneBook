using System.Net;
using System.Net.Mail;

namespace Phonebook;

internal class EmailService
{
    private string? _userName;

    private string? _password;

    public EmailService()
    {
        try
        {
            using TextReader reader = new StreamReader("emailConfig.txt");

            string? email = reader.ReadLine(), password = reader.ReadLine();
            if (email is null || password is null) return;
            _userName = email;
            _password = password;
        }

        catch (FileNotFoundException) {}
    }

    public void SetEmailConfig(string email, string password)
    {
        if (Validation.IsEmailValid(email!)) _userName = email;

        else throw new FormatException();

        _password = password;

        using TextWriter writer = new StreamWriter("emailConfig.txt");
        writer.WriteLine(email);
        writer.WriteLine(password);
    }

    public bool IsAccountNull() => _userName is null || _password is null;

    public void SendEmail(string destination, string subject, string body)
    {
        try
        {
            using var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_userName, _password);
            MailMessage email = new MailMessage();
            email.From = new MailAddress(_userName!);
            email.To.Add(destination);
            email.Subject = subject;
            email.Body = body;
            client.Send(email);
        }

        catch
        {
            Console.WriteLine(Helpers.EmailDispatchErrorMessage);
        }
    }
}