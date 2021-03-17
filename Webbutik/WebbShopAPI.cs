using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Webbutik.Database;
using Webbutik.Models;

namespace Webbutik
{
    public class WebbShopAPI
    {
        private ShopContext shopContext = new ShopContext();
        public int Login(string userName, string password)
        {
            var user = shopContext.Users.FirstOrDefault(
                u => u.Name == userName && u.Password == password);

            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                user.SessionTimer = DateTime.Now;
                shopContext.Users.Update(user);
                shopContext.SaveChanges();

                return user.Id;
            }

            return 0; // return 0 if the user does not exist.
        }

        public void Logout(int userId)
        {
            var user = shopContext.Users.FirstOrDefault(
                u => u.Id == userId && u.SessionTimer > DateTime.Now.AddMinutes(-15));

            if (user != null)
            {
                user.SessionTimer = DateTime.MinValue;
                shopContext.Users.Update(user);
                shopContext.SaveChanges();
            }
        }
    }
}
