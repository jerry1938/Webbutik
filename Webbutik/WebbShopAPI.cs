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
        public bool BuyBook(int userId, int bookId)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == userId);
            var book = shopContext.Books.FirstOrDefault(b => b.Id == bookId);

            if (user != null && user.SessionTimer > DateTime.Now.AddMinutes(-15))
            {
                if (book.Amount > 0)
                {
                    shopContext.SoldBooks.Add(new SoldBook
                    {
                        Title = book.Title,
                        AuthorId = book.AuthorId,
                        CategoryId = book.CategoryId,
                        Price = book.Price,
                        UserId = user.Id,
                        PurchasedDate = DateTime.Now
                    });
                    shopContext.SaveChanges();

                    book.Amount -= 1;
                    shopContext.Update(book);
                    shopContext.SaveChanges();

                    return true;
                }
            }

            return false;
        }

        public string Ping(int userId)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user.SessionTimer > DateTime.Now.AddMinutes(-15))
            {
                user.SessionTimer = DateTime.Now;
                shopContext.Update(user);
                shopContext.SaveChanges();
                return "pong";
            }

            Logout(userId);
            return string.Empty;
        }

        public bool Register(string name, string password, string passwordVerify)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Name == name);

            if (user == null && password == passwordVerify)
            {
                shopContext.Users.Add(new User
                {
                    Name = name,
                    Password = password,
                    IsAdmin = false,
                    IsActive = false
                });
                shopContext.SaveChanges();
                return true;
            }

            return false;
        }

        public bool AddBook(int adminId, string title, string author, int price, int amount)
        {
            var book = shopContext.Books.FirstOrDefault(b => b.Title == title);
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);
            var category = shopContext.BookCategories.FirstOrDefault(c => c.Name == "No Category");

            if (user.IsAdmin == true)
            {
                if (book != null)
                {
                    book.Amount += amount;
                    shopContext.Update(book);
                    shopContext.SaveChanges();
                    return true;
                }
                else
                {
                    shopContext.Books.Add(new Book
                    {
                        Title = title,
                        AuthorId = AddAuthor(author),
                        Price = price,
                        Amount = amount,
                        CategoryId = category.Id
                    });
                    shopContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        private int AddAuthor(string name)
        {
            var author = shopContext.Authors.FirstOrDefault(a => a.Name == name);

            if (author == null)
            {
                shopContext.Authors.Add(new Author { Name = name });
                shopContext.SaveChanges();
            }

            return author.Id;
        }

        public int SetAmount(int adminId, int bookId, int amount)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);
            var book = shopContext.Books.FirstOrDefault(b => b.Id == bookId);

            if (book != null)
            {
                book.Amount = amount;
                shopContext.Update(book);
                shopContext.SaveChanges();
                return book.Amount;
            }

            return 0;
        }

        public List<User> ListUsers(int adminId)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);

            if (user.IsAdmin == true)
            {
                return shopContext.Users.Where(u => u.IsAdmin == false).ToList();
            }

            return new List<User>();
        }

        public List<User> FindUser(int adminId, string keyword)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);

            if (user.IsAdmin == true)
            {
                return shopContext.Users.Where(u => u.IsAdmin == false && u.Name.Contains(keyword))
                    .ToList();
            }

            return new List<User>();
        }

        public bool UpdateBook(int adminId, int bookId, string title, string authorName, int price)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);
            var book = shopContext.Books.FirstOrDefault(b => b.Id == bookId);
            var author = shopContext.Authors.FirstOrDefault(a => a.Name == authorName);

            if (user.IsAdmin == true)
            {
                if (book != null)
                {
                    book.Title = title;
                    book.AuthorId = author.Id;
                    book.Price = price;
                    shopContext.Update(book);
                    shopContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        public bool DeleteBook(int adminId, int bookId)
        {
            var user = shopContext.Users.FirstOrDefault(u => u.Id == adminId);
            var book = shopContext.Books.FirstOrDefault(b => b.Id == bookId);

            if (user.IsAdmin == true)
            {
                if (book != null)
                {
                    shopContext.Remove(book);
                    shopContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }
    }
}
