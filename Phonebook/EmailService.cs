using System.Net;
using System.Net.Mail;

namespace Phonebook
{
    public class EmailService
    {
        public static string? UserName { get; set; }
        public static string? Password { get; set; }
        public static string[] Recievers = {};

        public EmailService(string[] recievers)
        {
            Recievers = recievers;
        }

        public void SendEmails(string subject, string body)
        {
            foreach (var reciever in Recievers) DispatchEmail(reciever, subject, body);
            Console.WriteLine($"Successfully delivered the email!");
        }

        internal static void DispatchEmail(string destination, string subject, string body)
        {        
            try
            {
                if (UserName == null || Password == null) return;

                using (var client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(UserName, Password);
                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(UserName);
                    mailMessage.To.Add(destination);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    client.Send(mailMessage);
                }
            }

            catch
            {
                Console.WriteLine(Helpers.EmailDispatchErrorMessage);
            }

        }
    }
}
;