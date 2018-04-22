using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcore.Actors
{
    public class AuthenticatorActorProvider
    {
        private ActorSelection ActorInstance { get; set; }

        public AuthenticatorActorProvider(string path, ActorSystem actorSystem)
        {
            ActorSelection server = actorSystem.ActorSelection(path);
            ActorInstance = server;
        }

        public ActorSelection Get()
        {
            return ActorInstance;
        }
    }
}
