using System;
using Webbutik.Database;

namespace Webbutik
{
    class Program
    {
        static void Main(string[] args)
        {
            string username;
            string password;
            int loggedIn;

            Seeder.Seed();
            WebbShopAPI webbShopAPI = new WebbShopAPI();

            username = "Administrator";
            password = "CodicRulez";

            loggedIn = webbShopAPI.Login(username, password);

            if (loggedIn > 0)
            {
                Console.WriteLine("Successfully logged in");
            }
            else
            {
                Console.WriteLine("Wrong username or password");
            }
        }
    }
}
