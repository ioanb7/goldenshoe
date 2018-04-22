using System;
using System.Collections.Generic;
using System.Text;

namespace Messages
{
    public class Authenticator
    {
        public class RegisterCommand
        {
            public string Name { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Password2 { get; set; }
        }

        public class LoginCommand
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }






        public class RegisterSuccess : ResponseMessage
        {
            public RegisterSuccess() : base("Register success")
            {
            }
        }
        public class RegisterUsernameExists : ResponseMessage
        {
            public RegisterUsernameExists() : base("Username already exists")
            {
            }
        }



        public class FormInvalidData : ResponseMessage
        {
            public FormInvalidData(string field) : base("Invalid data for field " + field)
            {
            }
        }

        


        public class LoginSuccess : ResponseMessage
        {
            public LoginSuccess(string apiKey) : base("Login successsful")
            {
                ApiKey = apiKey;
            }

            public string ApiKey { get; set; }
        }

        public class LoginFailedPassword : ResponseMessage
        {
            public LoginFailedPassword() : base("Login Failed - Wrong Password")
            {
            }
        }

        

    }
}
