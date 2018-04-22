using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcore.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aspcore.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticatorController : Controller
    {
        private IAuthenticator authenticator;

        public AuthenticatorController(IAuthenticator authenticator)
        {
            this.authenticator = authenticator;
        }
        
        [HttpGet]
        public string Get()
        {
            var userName = User.Identity.Name;

            return $"Super secret content, I hope you've got clearance for this {userName}...";
        }

        [HttpPost("register")]
        public async Task<object> Register([FromBody]Messages.Authenticator.RegisterCommand cmd)
        {
            var result = await authenticator.Register(cmd.Name, cmd.Username, cmd.Password, cmd.Password2);

            if (result is Messages.Authenticator.RegisterSuccess)
            {
                Response.StatusCode = 200;
                return result;
            }

            if (result is Messages.Authenticator.RegisterUsernameExists)
            {
                Response.StatusCode = 400;
                return result;
            }
            if (result is Messages.Authenticator.FormInvalidData)
            {
                Response.StatusCode = 400;
                return result;
            }

            Response.StatusCode = 500;
            return new object();
        }

        [HttpPost("login")]
        public async Task<object> Login([FromBody]Messages.Authenticator.LoginCommand cmd)
        {
            var result = await authenticator.Login(cmd.Username, cmd.Password);

            if (result is Messages.Authenticator.LoginSuccess)
            {
                Response.StatusCode = 200;
                return result;
            }

            if (result is Messages.Authenticator.FormInvalidData)
            {
                Response.StatusCode = 400;
                return result;
            }

            if (result is Messages.Authenticator.LoginFailedPassword)
            {
                Response.StatusCode = 400;
                return result;
            }

            Response.StatusCode = 500;
            return new object();
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
