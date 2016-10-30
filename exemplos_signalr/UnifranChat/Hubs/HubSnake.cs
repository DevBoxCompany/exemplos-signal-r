using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;

namespace UnifranChat.Hubs
{
    public class HubSnake : Hub
    {
        public void AtualizaPosicao(string direcaoTomar)
        {
            Clients.All.movePlayer(direcaoTomar);
        }

        public override Task OnConnected()
        {
            Clients.All.inicializaPosicao();
            return base.OnConnected();
        }
    }
}