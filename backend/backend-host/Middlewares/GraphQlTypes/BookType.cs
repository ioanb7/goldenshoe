using aspcore.Data;
using GraphQL.Types;
namespace aspcore.Middlewares.GraphQlTypes
{
    public class BookType : ObjectGraphType<Book>
    {
        public BookType()
        {
            Field(x => x.Isbn).Description("The isbn of the book.");
            Field(x => x.Name).Description("The name of the book.");
            Field<AuthorType>("author");
            Field<PublisherType>("publisher");

        }
    }
}
