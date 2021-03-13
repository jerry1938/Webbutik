using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Webbutik.Models;

namespace Webbutik.Database
{
    class ShopContext : DbContext
    {
        private const string DatabaseName = "WebbShopJeremyMatthiessen";
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<SoldBook> SoldBooks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($@"Data Source=(localdb)\MSSQLLocalDB;Database={DatabaseName};Integrated Security=True;");
        }
    }
}
