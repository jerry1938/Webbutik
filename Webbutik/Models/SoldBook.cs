using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Webbutik.Models
{
    class SoldBook
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public int Price { get; set; }
        public DateTime PurchasedDate { get; set; }

        public int CategoryId { get; set; }
        public BookCategory Category { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}
