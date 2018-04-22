using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL;
using System.IO;
using System.Collections.Generic;
using aspcore.Data;
using aspcore.Middlewares.GraphQlTypes;
using aspcore.Models;
using GraphQL.Utilities;

namespace aspcore.Middlewares
{
    public class GraphQlMiddleware
    {
        private readonly RequestDelegate _next;

        public GraphQlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ApplicationDbContext applicationContext)
        {
            var sent = false;
            if (httpContext.Request.Path.StartsWithSegments("/graph"))
            {
                using (var sr = new StreamReader(httpContext.Request.Body))
                {
                    var query = await sr.ReadToEndAsync();
                    if (!String.IsNullOrWhiteSpace(query))
                    {
                        BooksQuery bq = new BooksQuery(applicationContext);
                        var schema = new Schema { Query = bq };


                        if (httpContext.Request.Method == "OPTIONS")
                        {
                            var printedSchema = new SchemaPrinter(schema).Print();

                            await WriteResult(httpContext, printedSchema);

                            sent = true;
                        }
                        else
                        {
                            var result = await new DocumentExecuter()
                                .ExecuteAsync(options =>
                                {
                                    options.Schema = schema;
                                    options.Query = query;
                                }).ConfigureAwait(false);

                            CheckForErrors(result);

                            await WriteResult(httpContext, result);

                            sent = true;
                        }
                    }
                }
            }
            if (!sent)
            {
                await _next(httpContext);
            }
        }

        private async Task WriteResult(HttpContext httpContext, ExecutionResult result)
        {
            var json = new DocumentWriter(indent: true).Write(result);

            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(json);
        }

        private async Task WriteResult(HttpContext httpContext, string result)
        {
            httpContext.Response.StatusCode = 200;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(result);
        }

        private void CheckForErrors(ExecutionResult result)
        {
            if (result.Errors?.Count > 0)
            {
                var errors = new List<Exception>();
                foreach (var error in result.Errors)
                {
                    var ex = new Exception(error.Message);
                    if (error.InnerException != null)
                    {
                        ex = new Exception(error.Message, error.InnerException);
                    }
                    errors.Add(ex);
                }
                throw new AggregateException(errors);
            }
        }
    }
}
