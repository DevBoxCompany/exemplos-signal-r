using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.ModelBinding;
using System.Web.SessionState;
using Microsoft.AspNet.SignalR;
using UnifranChat.Models;

namespace UnifranChat.Hubs
{
    public class Chat : Hub
    {

        public static List<Usuario> usuarios = new List<Usuario>();
        public static string NomeUsuario;

        public void EnviarMensagem(string mensagem,string usuario)
        {
            if(usuario != "0")
                Clients.Clients(new[] { usuario, Context.ConnectionId }).TransmitirMensagem(RetornaNomeUsuario(), mensagem);
            else
                Clients.All.TransmitirMensagem(Context.ConnectionId, mensagem);
        }

        public override Task OnConnected()
        {
            if(usuarios.All(x => x.ConnectionId != Context.ConnectionId))
                usuarios.Add(new Usuario() { Nome = NomeUsuario, ConnectionId = Context.ConnectionId });

            Clients.All.limpaDropdown();
            MontaDropDownUsuarios();
            
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            usuarios.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
            Clients.All.limpaDropdown();
            MontaDropDownUsuarios();
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public string RetornaNomeUsuario()
        {
            return usuarios.First(x => x.ConnectionId == Context.ConnectionId).Nome;
        }


        public void MontaDropDownUsuarios()
        {
            if (!usuarios.Any())
                return;

            string usuariosGeral = usuarios.Aggregate("", (current, usuario) => current + (usuario.Nome + "," + usuario.ConnectionId + ";"));
            usuariosGeral = usuariosGeral.Substring(0, usuariosGeral.Length - 1);

            Clients.All.carregaUsuarios(usuariosGeral);
        }
        //public string RetornaIdUsuario()
        //{
        //    return Context.ConnectionId;
        //}
    }
}