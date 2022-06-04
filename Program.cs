namespace PhoneBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Helpers.message);
            //SqlAccess.Create(new Contact { Name = "Abhinav", PhoneNumber = 123456789 });
            SqlAccess.Read();
        }
    }
}