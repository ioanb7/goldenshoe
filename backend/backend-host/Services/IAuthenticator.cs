using System.Threading.Tasks;
using aspcore.Actors;
using Akka.Actor;
using Messages;

namespace aspcore.Services
{
    public interface IAuthenticator
    {
        Task<object> Register(string name, string username, string password, string password2);
        Task<object> Login(string username, string password);
    }

    public class Authenticator : IAuthenticator
    {
        private ActorSelection _actor;

        public Authenticator(AuthenticatorActorProvider aap)
        {
            _actor = aap.Get();
        }

        public async Task<object> Register(string name, string username, string password, string password2)
        {
            return await _actor.Ask(new Messages.Authenticator.RegisterCommand
            {
                Name = name,
                Username = username,
                Password = password,
                Password2 = password2
            });
        }

        public async Task<object> Login(string username, string password)
        {
            return await _actor.Ask(new Messages.Authenticator.LoginCommand
            {
                Username = username,
                Password = password
            });
        }
    }
}