using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcore.Data;

namespace aspcore.Models
{
    public interface IApplicationDbContext
    {
        Book BookByIsbn(string isbn);
        IEnumerable<Book> AllBooks();

        Author AuthorById(int id);
        IEnumerable<Author> AllAuthors();

        Publisher PublisherById(int id);
        IEnumerable<Publisher> AllPublishers();
    }
}
