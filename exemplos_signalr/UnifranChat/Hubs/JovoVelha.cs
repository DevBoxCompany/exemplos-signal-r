using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace UnifranChat.Hubs
{
    public class JovoVelha : Hub
    {
        public static readonly List<string >Jogadores = new List<string>();
        public static int controlaJogaga = 0;
        public void Marcar(string nome)
        {
            Clients.All.MarcaDiv(nome);
            if (controlaJogaga == 0)
            {
                Clients.Client(Jogadores[0]).disativaJogada();
                Clients.Client(Jogadores[1]).ativaJogada();
                controlaJogaga++;
            }
            else
            {
                Clients.Client(Jogadores[1]).disativaJogada();
                Clients.Client(Jogadores[0]).ativaJogada();
                controlaJogaga--;
            }

            //Clients.Client(Context.ConnectionId).verificaVitoria();
        }

        public override Task OnConnected()
        {
            Jogadores.Add(Context.ConnectionId);
            if (Jogadores.Count > 1)
                Clients.Client(Jogadores[1]).disativaJogada();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Jogadores.RemoveAll(x => x == Context.ConnectionId);
            controlaJogaga = 0;
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {

            return base.OnReconnected();
        }
    }
}