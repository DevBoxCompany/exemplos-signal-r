using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnifranChat.Models.Truco;

namespace UnifranChat.Hubs
{
    public class JovoTruco : Hub
    {
        public static readonly List<string> Jogadores = new List<string>();
        public static readonly Dictionary<string, string> JogadoresFixo = new Dictionary<string, string>();
        public static readonly List<Mesa> ListaMesas = new List<Mesa>();
        public static string NomeJogador { get; set; }
        public static int TotalMesa { get; set; }
        public static int TotalJogadores { get; set; }

        public static readonly List<ListConversa> ListaChat = new List<ListConversa>();
        //public static readonly Dictionary<string, string> ListEspera = new Dictionary<string, string>();
        public static readonly Dictionary<string, string[]> ListEspera = new Dictionary<string, string[]>();

        #region ::: Jogo Truco :::
        public void CartaNaMesa(string dados, string mesa, string jog, string idCarta, string jgVez, int valorCarta)
        {
            var a = ListaMesas.FirstOrDefault(x => x.MesaId == mesa);

            if (a != null)
            {
                a.PntJogada.Add(int.Parse(jog), valorCarta);

                if (a.PntJogada.Count == 2)
                {
                    #region ::: Verifica Rodada :::

                    var aaa = "";
                    var jogadas = a.PntJogada.Select(x => x).ToList();

                    if (jogadas[0].Value > jogadas[1].Value)
                    {
                        a.LJogada123[a.Rodada - 1] = jogadas[0].Key;
                    }
                    else if (jogadas[0].Value < jogadas[1].Value)
                    {
                        a.LJogada123[a.Rodada - 1] = jogadas[1].Key;
                    }
                    else if (jogadas[0].Value == jogadas[1].Value)
                    {
                        #region ::: Casos Empate :::
                        if (a.Rodada == 1)
                        {
                            a.LJogada123[a.Rodada - 1] = 0;
                        }
                        if (a.Rodada == 2)
                        {
                            if (a.LJogada123[0] == 1)
                                a.LJogada123[a.Rodada - 1] = 1;
                            else if (a.LJogada123[0] == 2)
                                a.LJogada123[a.Rodada - 1] = 2;
                            else
                            {
                                a.PntJog1++;
                                a.PntJog2++;
                            }
                        }
                        if (a.Rodada == 3)
                        {
                            if (a.LJogada123[0] == 1)
                                a.LJogada123[a.Rodada - 1] = 1;
                            else if (a.LJogada123[0] == 2)
                                a.LJogada123[a.Rodada - 1] = 2;
                            else
                                a.LJogada123[a.Rodada - 1] = 0;
                        }
                        #endregion
                    }

                    switch (a.LJogada123[a.Rodada - 1])
                    {
                        case 1:
                            a.PntJog1++;
                            jgVez = "2";
                            break;
                        case 2:
                            a.PntJog2++;
                            jgVez = "1";
                            break;
                        case 0:
                            a.PntJog1++;
                            a.PntJog2++;
                            break;
                    }


                    a.Rodada++;
                    a.PntJogada.Remove(1);
                    a.PntJogada.Remove(2);

                    #endregion
                }
                else
                {
                    Clients.Client(a.Jogador1).limpaMesa();
                    Clients.Client(a.Jogador2).limpaMesa();
                }

                Clients.Client(a.Jogador1).cartaMesa(dados, jog, idCarta, jgVez == "1" ? "2" : "1", a.PntJog1, a.PntJog2);
                Clients.Client(a.Jogador2).cartaMesa(dados, jog, idCarta, jgVez == "1" ? "2" : "1", a.PntJog1, a.PntJog2);

                switch (VerificaFimDoJog(a))
                {
                    #region ::: Testa Fim De Jogo :::
                    case "1":
                        a.SaldoPntJog1++;
                        a.JogoAtivo = false;
                        if (a.SaldoPntJog2 == 12)
                        {
                            Clients.Client(a.Jogador1).encerraJogo("Jogador 1 venceu.", "fim");
                            Clients.Client(a.Jogador2).encerraJogo("Jogador 1 venceu.", "fim");
                        }
                        else
                        {
                            Clients.Client(a.Jogador1).fimJogo("Jogador 1 venceu.", a.SaldoPntJog1, a.SaldoPntJog2, a.MesaId, a.Jogador1);
                            Clients.Client(a.Jogador2).fimJogo("Jogador 1 venceu.", a.SaldoPntJog1, a.SaldoPntJog2, a.MesaId, a.Jogador2);
                        }
                        break;
                    case "2":
                        a.SaldoPntJog2++;
                        a.JogoAtivo = false;
                        if (a.SaldoPntJog2 == 12)
                        {
                            Clients.Client(a.Jogador1).encerraJogo("Jogador 2 venceu.", "fim");
                            Clients.Client(a.Jogador2).encerraJogo("Jogador 2 venceu.", "fim");
                        }
                        else
                        {
                            Clients.Client(a.Jogador1).fimJogo("Jogador 2 venceu.", a.SaldoPntJog1, a.SaldoPntJog2, a.MesaId, a.Jogador1);
                            Clients.Client(a.Jogador2).fimJogo("Jogador 2 venceu.", a.SaldoPntJog1, a.SaldoPntJog2, a.MesaId, a.Jogador2);
                        }
                        break;
                    #endregion
                }
            }
        }

        public string VerificaFimDoJog(Mesa mesa)
        {
            if ((mesa.PntJog1 == 2 && mesa.PntJog2 == 0) ||
                (mesa.PntJog1 == 3 && mesa.PntJog2 == 2) ||
                (mesa.PntJog1 == 2 && mesa.PntJog2 == 1))
                return "1";
            if ((mesa.PntJog2 == 2 && mesa.PntJog1 == 0) ||
                   (mesa.PntJog2 == 3 && mesa.PntJog1 == 2) ||
                   (mesa.PntJog2 == 2 && mesa.PntJog1 == 1))
                return "2";

            return "";
        }

        public void InicioJogo(string chm = "")
        {
            if (chm == "")
            {
                Jogadores.Add(Context.ConnectionId);
                TotalJogadores++;
            }

            if (Jogadores.Count == 2)
            {
                TotalMesa++;
                var ms = new Mesa()
                {
                    MesaId = "mesa" + TotalMesa,
                    Jogador1 = Jogadores[0],
                    Jogador2 = Jogadores[1],

                };
                ListaMesas.Add(ms);

                var jsCartas = GeraCartas(ms, new List<int>());

                //id Mesa - Numero Jogador - Jogador da vez (Partial), jsonCartas (Partial), num Jogador(Principal) , Jog Vez(Principal)

                Clients.Client(Jogadores[0]).ativaMesa(ms.MesaId + ",1,1", jsCartas[0], "1", "1");
                if (Jogadores.Count > 1)
                {
                    Clients.Client(Jogadores[1]).ativaMesa(ms.MesaId + ",2,1", jsCartas[1], "2", "1");
                    Jogadores.RemoveAt(0);
                }
                Jogadores.RemoveAt(0);
            }
        }

        public void ListaJogadores()
        {
            string list = JogadoresFixo.Aggregate("", (current, jg) => current + (current == "" ? jg.Key + ";" + jg.Value : "," + jg.Key + ";" + jg.Value));

            foreach (var s in JogadoresFixo.Where(s => s.Value != ""))
                Clients.Client(s.Key).listaJogadoresOn(list);
        }

        public string[] GeraCartas(Mesa ms, List<int> listIndice, int qntCartas = 6)
        {
            //valor-img;valor-img;valor-img]valor-img;valor-img;valor-img  (Cartas 1,2 e 3) 2 Jogadores
            var jsonCartas = "";
            while (listIndice.Count < qntCartas)
            {
                var rd = new Random().Next(0, 40);

                if (listIndice.All(x => x != rd))
                {
                    listIndice.Add(rd);
                    jsonCartas +=
                        ms.Baralhos.LBaralho[rd].Valor + "-" +
                        ms.Baralhos.LBaralho[rd].EnderecoImg + (listIndice.Count == 3 ? "]" : listIndice.Count == 6 ? "" : ";");
                }
            }
            return jsonCartas.Split(']');
        }

        public void NovoJogo(string mesaId, string jogId)
        {
            var ms = ListaMesas.FirstOrDefault(x => x.MesaId == mesaId);
            var qntCrt = 6;
            if (ms == null) return;

            if (!ms.JogoAtivo)
            {
                ms.LJogada123 = new List<int>() { 0, 0, 0 };
                ms.PntJogada = new Dictionary<int, int>();
                ms.PntJog1 = 0;
                ms.PntJog2 = 0;
                ms.Rodada = 1;
                ms.JogoAtivo = true;
                ms.UltimoComeco = ms.UltimoComeco == 1 ? 2 : 1;
                ms.Cartas1Jog = new List<int>();
                qntCrt = 3;
            }

            var jsCartas = GeraCartas(ms, ms.Cartas1Jog, qntCrt);

            Clients.Client(jogId).ativaMesa(
                ms.MesaId + "," + (jogId == ms.Jogador1 ? "1" : "2") + "," + ms.UltimoComeco,
                jsCartas[0],
                (jogId == ms.Jogador1 ? "1" : "2"),
                ms.UltimoComeco);
        }
        #endregion

        #region ::: Chat :::

        public void Convidar(string convId)
        {
            if (Context.ConnectionId == convId)
                return;

            if(ListaChat.Any(x =>
                (x.UsuarioRemId == Context.ConnectionId && x.UsuarioConvId == convId) ||
                (x.UsuarioConvId == Context.ConnectionId && x.UsuarioRemId == convId)))
                return;

            var a = JogadoresFixo.First(x => x.Key == Context.ConnectionId);

            ListEspera.Add(Context.ConnectionId + convId, new []{ Context.ConnectionId, convId });

            Clients.Client(convId).permitirMensagem(a.Value, a.Key);

        }

        public void ConfiraConvite(string idRem, bool resposta)
        {
            var lEsp = ListEspera.FirstOrDefault(x => x.Key == idRem + Context.ConnectionId);

            if (lEsp.Key != null)
            {
                if (resposta)
                {
                    var conv = new ListConversa()
                    {
                        IdConversa = lEsp.Key,
                        ChatStatus = Status.Ativo,
                        IdConteudo = lEsp.Key.Replace("-", ""),
                        UsuarioRemId = idRem,
                        UsuarioRemNome = JogadoresFixo.First(x => x.Key == idRem).Value,
                        UsuarioConvId = Context.ConnectionId,
                        UsuarioConvNome = JogadoresFixo.First(x => x.Key == Context.ConnectionId).Value,
                    };
                    ListaChat.Add(conv);

                    Clients.Client(idRem)
                        .aceitarConvite(conv.IdConversa, conv.IdConteudo, conv.UsuarioConvNome);
                    Clients.Client(Context.ConnectionId)
                        .aceitarConvite(conv.IdConversa, conv.IdConteudo, conv.UsuarioRemNome);
                }
                else
                    Clients.Client(lEsp.Value[0])
                        .recusarConvite(JogadoresFixo.First(x => x.Key == Context.ConnectionId).Value,"1");

                ListEspera.Remove(lEsp.Key);
            }
            else
                Clients.Client(idRem)
                   .recusarConvite("Jogador abandonou o chat.", "2");
        }

        public void EnviarMensagem(string mensagem, string idConversa, string idConteudo)
        {
            var a = ListaChat.First(x => x.IdConversa == idConversa);

            var nome = a.UsuarioConvId == Context.ConnectionId
                ? a.UsuarioConvNome
                : a.UsuarioRemNome;

            Clients.Client(a.UsuarioRemId).transmitirMensagem(mensagem, nome, idConteudo);
            Clients.Client(a.UsuarioConvId).transmitirMensagem(mensagem, nome, idConteudo);
        }

        public void EncerraConversa(string idConversa , string idChat)
        {
            var a = ListaChat.FirstOrDefault(x => x.IdConversa == idConversa);

            if(a == null)return;

            Clients.Client(a.UsuarioRemId).encrraConvChat(idChat);
            Clients.Client(a.UsuarioConvId).encrraConvChat(idChat);

            ListaChat.RemoveAll(x => x.IdConversa == idConversa);
        }

        #endregion

        #region ::: Metos Hubs :::
        public override Task OnConnected()
        {
            if (Jogadores.Any(x => x == Context.ConnectionId))
                return null;

            JogadoresFixo.Add(Context.ConnectionId, NomeJogador);

            InicioJogo();
            ListaJogadores();

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            TotalJogadores--;

            Jogadores.RemoveAll(x => x == Context.ConnectionId);
            JogadoresFixo.Remove(Context.ConnectionId);

            var a = ListaMesas.FirstOrDefault(x => x.Jogador1 == Context.ConnectionId || x.Jogador2 == Context.ConnectionId);

            #region ::: Retorna Jogador Restante outra mesa :::
            if (a != null)
            {
                var b = a.Jogador1 == Context.ConnectionId
                    ? a.Jogador2
                    : a.Jogador1;

                Jogadores.Add(b);
                ListaMesas.RemoveAll(x => x.MesaId == a.MesaId);

                Clients.Client(b).encerraJogo("Seu adversasrio abandonou o jogo.", "abandono");

                if (Jogadores.Count == 2)
                    InicioJogo("xXx");
            } 
            #endregion

            #region ::: Remove listas chat :::

            var listChat = ListaChat
                .Where(x => x.UsuarioConvId == Context.ConnectionId || x.UsuarioRemId == Context.ConnectionId)
                .ToList();
           
            foreach (var i in listChat)
            {
                EncerraConversa(i.IdConversa, "#divC" + i.IdConteudo);
                ListaChat.RemoveAll(x => x.IdConversa == i.IdConversa);
            }

            var listEspera = ListEspera
                .Where(x => x.Value[0] == Context.ConnectionId || x.Value[1] == Context.ConnectionId)
                .Select(x => x.Key)
                .ToList();

            foreach (var i in listEspera)
                ListEspera.Remove(i); 

            #endregion

            ListaJogadores();
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
        #endregion
    }
}