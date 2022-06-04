namespace PhoneBook
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Helpers.message);
            SqlAccess.Create(new Contact { Name = "Felix Parker", PhoneNumber = 7680534290 });
            SqlAccess.Create(new Contact { Name = "Mila Baker", PhoneNumber = 2192670520 });
            SqlAccess.Read();
            SqlAccess.Update(2, "Mila Cooper");
            SqlAccess.Read();
            SqlAccess.Update(2, "7266778447");
            SqlAccess.Read();
            SqlAccess.Delete("2");
            SqlAccess.Read();
            SqlAccess.Delete("Felix Parker");
            SqlAccess.Read();
        }
    }
}