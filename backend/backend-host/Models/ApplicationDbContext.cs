using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcore.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace aspcore.Models
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Publisher> Publisher { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public Book BookByIsbn(string isbn)
        {
            return Books.FirstOrDefault(_ => _.Isbn == isbn);
        }

        public IEnumerable<Book> AllBooks()
        {
            return Books.AsNoTracking().ToList();
        }

        public Author AuthorById(int id)
        {
            return Author.FirstOrDefault(_ => _.Id == id);
        }

        public IEnumerable<Author> AllAuthors()
        {
            return Author.AsNoTracking().ToList();
        }

        public Publisher PublisherById(int id)
        {
            return Publisher.FirstOrDefault(_ => _.Id == id);
        }

        public IEnumerable<Publisher> AllPublishers()
        {
            return Publisher.AsNoTracking().ToList();
        }
    }
}
