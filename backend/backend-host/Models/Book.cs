
using System.ComponentModel.DataAnnotations;

namespace aspcore.Data
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public string Isbn { get; set; }

        public string Name { get; set; }

        public Author Author { get; set; }

        public Publisher Publisher { get; set; }
    }
}
