using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace authenticator
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        [Index(IsUnique = true)]
        public string Username { get; set; }
        public string Password { get; set; } // TODO: care. these are not encrypted at all.
        //public string ApiKey { get; set; }
        public User() { }
    }
}
