using Microsoft.EntityFrameworkCore;
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
                u => u.Name == userName && u.Password == password && u.IsActive == false);

            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                user.SessionTimer = DateTime.Now;
                user.IsActive = true;
                shopContext.Users.Update(user);
                shopContext.SaveChanges();

                return user.Id;
            }

            return 0; // return 0 if the user does not exist.
        }

        public void Logout(int userId)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                user.SessionTimer = DateTime.MinValue;
                user.IsActive = false;
                shopContext.Users.Update(user);
                shopContext.SaveChanges();
            }
        }

        public List<BookCategory> GetCategories()
        {
            return shopContext.BookCategories.ToList();
        }

        public List<BookCategory> GetCategories(string keyword)
        {
            return shopContext.BookCategories.Where(c => c.Name.Contains(keyword)).ToList();
        }

        public List<Book> GetCategory(int categoryId)
        {
            return shopContext.Books.Where(b => b.CategoryId == categoryId).ToList();
        }

        public List<Book> GetAvailableBooks(int categoryId)
        {
            return shopContext.Books.Where(b => b.Amount > 0 && b.CategoryId == categoryId)
                .ToList();
        }

        public Book GetBook(int bookId)
        {
            return shopContext.Books.Include("Author").FirstOrDefault(b => b.Id == bookId);
        }

        public List<Book> GetBooks(string keyword)
        {
            return shopContext.Books.Where(b => b.Title.Contains(keyword)).ToList();
        }

        public List<Book> GetAuthors(string keyword)
        {
            return shopContext.Books.Include("Author").Where(b => b.Author.Name.Contains(keyword))
                .ToList();
        }
    }
}
