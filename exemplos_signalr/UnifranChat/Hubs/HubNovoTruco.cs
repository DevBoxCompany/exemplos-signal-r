using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glimpse.AspNet.Controls;
using Microsoft.AspNet.SignalR;
using UnifranChat.Models.NovoTruco;

namespace UnifranChat.Hubs
{
    public class HubNovoTruco : Hub
    {
        public static NovoTruco NovoTruco = new NovoTruco();
        public static string NomeJogador { get; set; }
        public static List<ChatNvTrucoDois> ListChatNvDois = new List<ChatNvTrucoDois>();

        #region ::: Jogo Novo Truco :::

        public void SentarMesa(string idMesa, string numJogador)
        {
            var usua = RetornaUsuario(Context.ConnectionId);
            if (usua == null || usua.EmEspera) return;

            var mesa = RetornaMesa(idMesa);
            if (mesa == null) return;

            Clients.Clients(NovoTruco.ListUsuarios.Where(x => !x.Ativo).Select(x => x.ConnectionId).ToList())
                   .sentaMesa("#" + idMesa, numJogador);

            mesa.VerificaMesaCheia(usua, numJogador);

            Clients.Client(Context.ConnectionId).abreMesa(idMesa, numJogador);
        }

        public void ExibeCartasViradas(string idMesa)
        {
            var mesa = RetornaMesa(idMesa);
            var jogs = RetornaJogadoresMesa(mesa);

            var a = string.Join(",", jogs.Select(x => x.NumJogador));
            Clients.Clients(jogs.Select(x => x.Usuario.ConnectionId).ToList()).mostraCartas(a);

            if (mesa.Ativa)
            {
                var cartas = mesa.RetornaCartas();
                var contCrt = 0;
                for (var i = 0; i < 3; i++)
                {
                    for (var j = 0; j < 4; j++)
                    {
                        Clients.Client(jogs[j].Usuario.ConnectionId)
                            .distribuiCartas(mesa.Cadeiras[j][i], cartas[contCrt].EnderecoImg, cartas[contCrt].Valor);
                        contCrt++;
                    }
                }
            }
        }

        public void FinalRodada
            (MesaNvTruco mesa, string idMesa, string idCarta, string backImage, string valor, string idJg, string eWhich, string jgVez)
        {
            var condFim = false;
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                    .colocaCartaMesa("#" + idCarta, backImage, valor, idJg, eWhich, jgVez);

            mesa.JogadorVez = int.Parse(jgVez);
            var retorno = mesa.QuedasJogos.VerificaVencedor(mesa);

            mesa.Rodada++;
            if (retorno || mesa.Rodada == 4)
            {
                condFim = mesa.QuedasJogos.FechaQueda(); // True = 12 pontos Fecha Partida

                #region ::: Preenche Parametros :::
                var a1 = mesa.QuedasJogos.PntDupla[0].ToString();
                var a2 = mesa.QuedasJogos.PntDupla[1].ToString();
                var a3 = mesa.QuedasJogos.VitoriasRodada[0].ToString();
                var a4 = mesa.QuedasJogos.VitoriasRodada[1].ToString();
                var a5 = mesa.JogadorVez.ToString();
                #endregion

                if (!condFim)
                {
                    Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                        .finalizaRodada(a1, a2, a3, a4, a5);
                }
                else
                {
                    var a = mesa.Duplas.First(x => x.Jogador.Any(y => y.NumJogador == mesa.VencedorRodada));
                    var b = a.Jogador[0].Usuario.Nome + ',' + a.Jogador[0].Usuario.Nome;
                    Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                         .finalizaRodadaGeral(a1, a2, a3, a4, a5, b);
                }

                mesa.Rodada = 1;
            }
            mesa.ListJogadoas.Clear();

            if (retorno)
            {
                if (!condFim) ExibeCartasViradas(idMesa);
                Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList()).esconderMesa(mesa.JogadorVez.ToString());

                if (mesa.QuedasJogos.MaoJogo == TipoMesa.MaoDeOnze)
                {
                    var v = RetornaMesaDupla1(mesa, mesa.QuedasJogos.IdDuplaMaoDeOnze);
                    var p = RetornaMesaDupla2(mesa, mesa.QuedasJogos.IdDuplaMaoDeOnze);

                    Clients.Clients(v.Jogador.Select(x => x.Usuario.ConnectionId).ToList()).maoDeOnzeEspera();
                    Clients.Clients(p.Jogador.Select(x => x.Usuario.ConnectionId).ToList()).maoDeOnzeEscolhe();
                }
            }
            else
                Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList()).esconderMesa(mesa.JogadorVez.ToString());

            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                  .preenchePainel(mesa.UltimoMao, mesa.JogadorVez, mesa.VencedorRodada);
        }

        public void JogarCartaMesas(string idMesa, string idCarta, string backImage, string valor, string idJg, string eWhich, string jgVez)
        {
            var mesa = RetornaMesa(idMesa);
            var teste = mesa.TestaJogada(valor, idJg);

            //True fecha rodada ou partida
            if (teste)
            {
                FinalRodada(mesa, idMesa, idCarta, backImage, valor, idJg, eWhich, jgVez);
                Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                      .atualizaPainelModal();
                return;
            }

            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                         .colocaCartaMesa("#" + idCarta, backImage, valor, idJg, eWhich, mesa.ProxJogadorVez(int.Parse(jgVez)));

            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
             .preenchePainel(mesa.UltimoMao, mesa.ProxJogadorVez(int.Parse(jgVez)), 0);

            mesa.PainelCtrl.AtualizaJogVezVenc(mesa.ProxJogadorVez(int.Parse(jgVez)));
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                .atualizaPainelModal();
        }

        public void PedirTruco(string idMesa, string numJogador, string valorTruco)
        {
            var mesa = RetornaMesa(idMesa);
            var dupla = RetornaDuplasMesa(mesa, int.Parse(numJogador));

            if (dupla.IdDupla == mesa.QuedasJogos.IdDuplaTruco)
                return;

            mesa.QuedasJogos.IdDuplaTruco = dupla.IdDupla;

            var proxValor = int.Parse(valorTruco) + 3;

            Clients.Clients(dupla.Jogador.Select(x => x.Usuario.ConnectionId).ToList())
                   .pedirTrucoResult("#divModalTrucado", valorTruco, "0", "0", "0");

            Clients.Clients(mesa.Duplas.First(x => x.IdDupla != dupla.IdDupla).Jogador.Select(x => x.Usuario.ConnectionId).ToList())
                   .pedirTrucoResult("#divModalTrucar", valorTruco, proxValor, ((TipoTruco)proxValor).ToString(), FrasesTruco.Frase[int.Parse(valorTruco)]);
        }

        public void AceitarTruco(string idMesa, string valorTruco)
        {
            var mesa = RetornaMesa(idMesa);
            mesa.QuedasJogos.PedidoTruco(int.Parse(valorTruco));

            var proxValor = int.Parse(valorTruco) + 3;

            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                .aceitarTrucoResult(proxValor, ((TipoTruco)proxValor).ToString());
        }

        public void CorrerTruco(string idMesa)
        {
            var mesa = RetornaMesa(idMesa);
            var retorno = mesa.QuedasJogos.ContabilizaCorrerTruco(mesa);
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList()).correrTrucoResult();
        }

        public void ProximaRodadaCartas(string idMesa)
        {
            var mesa = RetornaMesa(idMesa);

            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList()).finalizaRodada(
                     mesa.QuedasJogos.PntDupla[0].ToString(),
                     mesa.QuedasJogos.PntDupla[1].ToString(),
                     mesa.QuedasJogos.VitoriasRodada[0].ToString(),
                     mesa.QuedasJogos.VitoriasRodada[1].ToString(),
                     mesa.JogadorVez.ToString()
                 );

            mesa.Rodada = 1;

            ExibeCartasViradas(idMesa);
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList()).esconderMesa(mesa.JogadorVez.ToString());
        }

        public void DesativaJogadorMesa(MesaNvTruco mesa)
        {
            if (mesa != null)
            {
                var jogs =
                    RetornaJogadoresMesa(mesa).Where(x => x.Usuario.ConnectionId != Context.ConnectionId).ToList();

                if (mesa.Ativa)
                {
                    var usuarios = jogs.Select(x => x.Usuario).ToList();
                    foreach (var usua in usuarios)
                        usua.DesativaUsuario();
                    mesa.ZeraMesa();

                    Clients
                        .Clients(
                           NovoTruco.ListUsuarios
                               .Where(x => x.ConnectionId != Context.ConnectionId)
                               .Select(x => x.ConnectionId).ToList())
                       .abandonarJogoGeral("#" + mesa.MesaId, Context.ConnectionId);

                    if (usuarios.Any())
                        Clients.Clients(usuarios.Select(x => x.ConnectionId).ToList()).carregaJogoMesa();
                }
                else
                {
                    var numJog = mesa.Jogadores.First(x => x != null && x.Usuario.ConnectionId == Context.ConnectionId);

                    mesa.ExcluiJogador(Context.ConnectionId);

                    Clients.Clients(
                      NovoTruco.ListUsuarios
                        .Where(x => x.ConnectionId != Context.ConnectionId)
                        .Select(x => x.ConnectionId).ToList())
                        .abandonarJogoMesaJgs("#" + mesa.MesaId, Context.ConnectionId, numJog.NumJogador);
                }
            }
            else
            {
                Clients.Clients(NovoTruco.ListUsuarios
                    .Where(x => x.ConnectionId != Context.ConnectionId)
                    .Select(x => x.ConnectionId).ToList())
                    .abandonarJogoGeral("", Context.ConnectionId);
            }
        }

        public void CorrerMaoDeOnze(string idMesa)
        {
            var mesa = RetornaMesa(idMesa);
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                .maoDeOnzeResult("txtTrucoResultNo", "Dupla Correu", "false");
        }

        public void AceiatrMaoDeOnze(string idMesa)
        {
            var mesa = RetornaMesa(idMesa);
            Clients.Clients(mesa.Jogadores.Select(x => x.Usuario.ConnectionId).ToList())
                .maoDeOnzeResult("txtTrucoResultOk", "Jogar Mão de Onze", "true");
        }

        #endregion

        #region ::: Chat Novo Truco :::

        #region ::: Chat Dois :::
        public void EnviarMensagem(string connectId, string conteudo)
        {
            var destinatario = NovoTruco.ListUsuarios.FirstOrDefault(x => x.ConnectionId == connectId);
            var remetente = NovoTruco.ListUsuarios.First(x => x.ConnectionId == Context.ConnectionId);

            if (destinatario == null)
            {
                Clients.Client(remetente.ConnectionId).usuarioDisconect(connectId);
                return;
            }
            var menssagem = "<div><b>" + remetente.Nome + ":</b>" + conteudo + "</div>";

            var idChat1 = destinatario.ConnectionId + remetente.ConnectionId;
            var idChat2 = remetente.ConnectionId + destinatario.ConnectionId;

            if (!ListChatNvDois.Any(x => x.IdCHatDois == idChat1 || x.IdCHatDois == idChat2))
            {
                ListChatNvDois.Add(new ChatNvTrucoDois(new[] { destinatario, remetente }, idChat1));
                var lChat = ListChatNvDois.First(x => x.IdCHatDois == idChat1);
                lChat.ChatAberto[0] = true;
                lChat.ChatAberto[1] = true;
            }

            var listChat = RetornaChatDois(idChat1, idChat2);
            listChat.AdicionaConversa(menssagem);

            Clients.Client(destinatario.ConnectionId)
                .transmitirMensagemDest(remetente.ConnectionId, remetente.Nome, listChat.RetornaConversa(destinatario.ConnectionId, menssagem));

            Clients.Client(remetente.ConnectionId)
                .transmitirMensagemReme(destinatario.ConnectionId, destinatario.Nome, listChat.RetornaConversa(remetente.ConnectionId, menssagem));

            listChat.AtvDesativaUsuario(destinatario.ConnectionId, true);
            listChat.AtvDesativaUsuario(remetente.ConnectionId, true);
        }

        public void Digitando(string remetenteId, string onOff)
        {
            Clients.Client(remetenteId).digitandoRec(Context.ConnectionId, onOff);
        }

        public void FecharChat(string idOutro)
        {
            var idChat1 = idOutro + Context.ConnectionId;
            var idChat2 = Context.ConnectionId + idOutro;

            var listChat = RetornaChatDois(idChat1, idChat2);
            if (listChat != null)
                listChat.AtvDesativaUsuario(Context.ConnectionId, false);
        }

        public void DesativaUsuarioOff(string connectId)
        {
            //var chats = ListChatNvDois
            //    .Select(x => x).Where(x => x.Usuarios.Any(y => y.ConnectionId == connectId)).ToList();
            //foreach (var a in chats)
            //{
            //    ListChatNvDois.RemoveAll(x => x.IdCHatDois == a.IdCHatDois);
            //}
            ListChatNvDois.RemoveAll(x => x.Usuarios.Any(y => y.ConnectionId == connectId));
        }
        #endregion

        #region ::: Chat Geral :::

        public void EnviarMenssagemGeral(string conteudo)
        {
            if (!NovoTruco.ListUsuarios.Any())
                return;
            var usua = NovoTruco.ListUsuarios.First(x => x.ConnectionId == Context.ConnectionId);

            var menssagem = "<div><b>" + usua.Nome + ":</b>" + conteudo + "</div>";

            Clients.Clients(NovoTruco.ListUsuarios.Select(x => x.ConnectionId).ToList())
                .transmiteMessageGeral(menssagem);
        }

        #endregion

        #endregion

        #region ::: Metodos Hub :::

        public override Task OnConnected()
        {
            if (NovoTruco.ListUsuarios.Any(x => x.ConnectionId == Context.ConnectionId))
                return null;

            var usua = new UsuarioNvTruco()
            {
                ConnectionId = Context.ConnectionId,
                Nome = NomeJogador
            };
            NovoTruco.ListUsuarios.Add(usua);
            Clients.Client(Context.ConnectionId).marcaUsuario(usua.ConnectionId, usua.Nome);

            Clients.Clients(NovoTruco.ListUsuarios.Where(x => !x.Ativo).Select(x => x.ConnectionId).ToList())// && !x.EmEspera
                .carregaJogo();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var mesa = RetornaMesaJg(Context.ConnectionId);

            DesativaJogadorMesa(mesa);

            NovoTruco.ListUsuarios.RemoveAll(x => x.ConnectionId == Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        #endregion

        #region ::: MetodosAuxiliares :::

        public MesaNvTruco RetornaMesa(string mesaId)
        {
            return NovoTruco.ListSalas.Select(x => x.ListaMesas.FirstOrDefault(y => y.MesaId == mesaId)).FirstOrDefault();
        }

        public MesaNvTruco RetornaMesaJg(string connectionId)
        {
            return NovoTruco.ListSalas.ToList()
                 .Select(x => x.ListaMesas).ToList()
                 .Select(y => y
                     .FirstOrDefault(z => z.Jogadores.Any(i => i != null && i.Usuario.ConnectionId == connectionId)))
                 .FirstOrDefault();
        }
        public UsuarioNvTruco RetornaUsuario(string connection)
        {
            return NovoTruco.ListUsuarios.FirstOrDefault(x => x.ConnectionId == connection);
        }
        public List<JogadorNvTruco> RetornaJogadoresMesa(MesaNvTruco mesa)
        {
            return mesa.Jogadores.Where(x => x != null).ToList();
        }

        public DuplaNvTruco RetornaDuplasMesa(MesaNvTruco mesa, int numJogador)
        {
            //return mesa.Duplas.FirstOrDefault(x => x.Jogador == x.Jogador.Where(y => y.NumJogador == numJogador));
            return mesa.Duplas
                    .FirstOrDefault(x => x.Jogador.Any(y => y.NumJogador == numJogador));
        }

        public DuplaNvTruco RetornaMesaDupla1(MesaNvTruco mesa, int idDupla)
        {
            return mesa.Duplas.FirstOrDefault(x => x.IdDupla == idDupla);
        }
        public DuplaNvTruco RetornaMesaDupla2(MesaNvTruco mesa, int idDupla)
        {
            return mesa.Duplas.FirstOrDefault(x => x.IdDupla != idDupla);
        }

        #endregion

        #region ::: Metodos Auxiliares Chat :::

        public ChatNvTrucoDois RetornaChatDois(string id1, string id2)
        {
            return ListChatNvDois.FirstOrDefault(x => x.IdCHatDois == id1 || x.IdCHatDois == id2);
        }


        #endregion
    }
}