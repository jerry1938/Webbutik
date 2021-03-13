using System;
using System.Collections.Generic;
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
            throw new NotImplementedException();
        }

        public void Logout(int userId)
        {

        }
    }
}
