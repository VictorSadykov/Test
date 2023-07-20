﻿namespace Test.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description{ get; set; }
        public decimal Price { get; set; }
        public DateTime PublishedOn { get; set; }
        public string? ImageUrl { get; set; }
        public int AuthorId { get; set; }
        public int? GenreId { get; set; }        
    }
}
