using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Ajax.Utilities;
using UnifranChat.Models.Truco;

namespace UnifranChat.Models.NovoTruco
{
    public class NovoTruco
    {
        public List<UsuarioNvTruco> ListUsuarios = new List<UsuarioNvTruco>();
        public List<SalasNvTruco> ListSalas = new List<SalasNvTruco>();

        public NovoTruco()
        {
            ListSalas.Add(new SalasNvTruco(1));
            ListSalas.Add(new SalasNvTruco(2));
            ListSalas.Add(new SalasNvTruco(3));
        }
    }

    public class UsuarioNvTruco
    {
        public string Nome { get; set; }
        public string ConnectionId { get; set; }
        public bool Ativo { get; set; }
        public bool EmEspera { get; set; }

        public UsuarioNvTruco()
        {
            Ativo = false;
            EmEspera = false;
        }

        public void DesativaUsuario()
        {
            Ativo = false;
            EmEspera = false;
        }
    }

    public class SalasNvTruco
    {
        public int IdSala { get; set; }
        public List<MesaNvTruco> ListaMesas { get; set; }

        public SalasNvTruco(int idSala)
        {
            IdSala = idSala;
            ListaMesas = new List<MesaNvTruco>();

            for (var i = 1; i <= 6; i++)
                ListaMesas.Add(new MesaNvTruco("Sala" + IdSala + "Mesa" + i));
        }
    }

    public class JogadorNvTruco
    {
        public UsuarioNvTruco Usuario { get; set; }
        public int NumJogador { get; set; }

        public JogadorNvTruco()
        {
            Usuario = new UsuarioNvTruco();
            NumJogador = 0;
        }
    }

    public class MesaNvTruco
    {
        public string MesaId { get; set; }                              // Id da mesa 
        public JogadorNvTruco[] Jogadores = new JogadorNvTruco[4];      // Jogadores Sentados na mesa
        public bool Ativa { get; set; }                                 // Mesa Cheia
        public List<string[]> Cadeiras
        {
            get
            {
                return new List<string[]>()
                {
                    new[] {"#cartaJg1", "#cartaJg2", "#cartaJg3"},
                    new[] {"#cartaJg4", "#cartaJg5", "#cartaJg6"},
                    new[] {"#cartaJg7", "#cartaJg8", "#cartaJg9"},
                    new[] {"#cartaJg10", "#cartaJg11", "#cartaJg12"}
                };
            }
        }                               // Id cadeiras da mesa (todas cadeiras)
        public List<DuplaNvTruco> Duplas { get; set; }                  // Duplas Formadas
        public int Rodada { get; set; }                                 // Numero da rodada da vez
        public Baralho Baralhos { get; set; }                           // 1 Baralho - Todas cartas truco
        public QuedaNovoTruco QuedasJogos { get; set; }                 // Controle de pontos da partida
        public Dictionary<int, int> ListJogadoas { get; set; }          // Lista de idJogador e Carta(valor) por rodada

        public int JogadorVez { get; set; }                             // Jogador da vez
        public int UltimoMao { get; set; }                              // Ultimo A Comecar a rodada
        public int VencedorRodada { get; set; }                         // Jogador Ganhou Rodada

        public PainelControle PainelCtrl { get; set; }

        public MesaNvTruco(string idMesa)
        {
            Rodada = 1;
            Baralhos = new Baralho();
            MesaId = idMesa;
            Ativa = false;
            JogadorVez = 1;
            UltimoMao = 1;
            VencedorRodada = 0;
        }
        public bool MesaCheia()
        {
            Ativa = true;

            for (var i = 0; i < 4; i++)
                Jogadores[i].Usuario.Ativo = true;

            Duplas = new List<DuplaNvTruco>()
            {
                new DuplaNvTruco(Jogadores[0],Jogadores[2] , 0),
                new DuplaNvTruco(Jogadores[1],Jogadores[3] , 1)
            };
            QuedasJogos = new QuedaNovoTruco(Duplas);
            ListJogadoas = new Dictionary<int, int>();
            PainelCtrl = new PainelControle(
                new[]
                {
                    Jogadores[0].Usuario.Nome,
                    Jogadores[2].Usuario.Nome,
                    Jogadores[1].Usuario.Nome,
                    Jogadores[3].Usuario.Nome,
                });

            return true;
        }
        public bool VerificaMesaCheia(UsuarioNvTruco usua, string numJogador)
        {
            usua.EmEspera = true;
            var jg = new JogadorNvTruco()
            {
                Usuario = usua,
                NumJogador = int.Parse(numJogador)
            };
            Jogadores[jg.NumJogador - 1] = jg;

            return Jogadores.Count(x => x != null) == 4 && MesaCheia();
        }
        public List<Cartas> RetornaCartas()
        {
            var listCartas = new List<Cartas>();
            var listIndice = new List<int>();
            while (listCartas.Count < 12)
            {
                var rd = new Random().Next(0, 40);
                if (listIndice.All(x => x != rd))
                {
                    listIndice.Add(rd);
                    listCartas.Add(Baralhos.LBaralho[rd]);
                }
            }
            return listCartas;
        }
        public int ProxJogadorVez(int ultimoJgVez)
        {
            var proximo = ultimoJgVez == 1 ? 2 : ultimoJgVez == 2 ? 3 : ultimoJgVez == 3 ? 4 : 1;
            JogadorVez = proximo;
            return proximo;
        }
        public bool TestaJogada(string valor, string idJg)
        {
            ListJogadoas.Add(int.Parse(idJg), int.Parse(valor));
            return ListJogadoas.Count == 4;
        }

        public void ZeraMesa()
        {
            Jogadores[0] = null;
            Jogadores[1] = null;
            Jogadores[2] = null;
            Jogadores[3] = null;
            Rodada = 1;
            Ativa = false;
            JogadorVez = 1;
            UltimoMao = 1;
            VencedorRodada = 0;
            Duplas.Clear();
            QuedasJogos = null;
            ListJogadoas.Clear();
        }

        public void ExcluiJogador(string connectionId)
        {
            for (var i = 0; i < Jogadores.Length; i++)
            {
                if (Jogadores[i] != null && Jogadores[i].Usuario.ConnectionId == connectionId)
                    Jogadores[i] = null;
            }
        }
    }

    public class DuplaNvTruco
    {
        public JogadorNvTruco[] Jogador = new JogadorNvTruco[2];
        public int IdDupla { get; set; }

        public int VitoriasTotal { get; set; }

        public DuplaNvTruco(JogadorNvTruco jogadore1, JogadorNvTruco jogadore2, int idDupla)
        {
            Jogador[0] = jogadore1;
            Jogador[1] = jogadore2;

            IdDupla = idDupla;
            VitoriasTotal = 0;
        }
    }

    public class QuedaNovoTruco
    {
        public int[] IdDupla = new int[2];
        public int[] PntDupla = new int[2];
        public int[] Rodadas = new int[3];         // Id Dupla q Ganhou a rodada

        public int[] VitoriasRodada = new int[2];     // 1 Pontos

        public TipoTruco ValorPonto { get; set; }  // valor rodada (caso truco)
        public TipoMesa MaoJogo { get; set; }                           // Jogador Ganhou Rodada

        public int IdDuplaTruco { get; set; }
        public int IdDuplaMaoDeOnze { get; set; }

        public RegrasNvTruco RegraJogo { get; set; }

        public QuedaNovoTruco(IReadOnlyList<DuplaNvTruco> listDuplas)
        {
            IdDupla[0] = listDuplas[0].IdDupla;
            IdDupla[1] = listDuplas[1].IdDupla;
            ValorPonto = TipoTruco.Normal;
            RegraJogo = new RegrasNvTruco();
            PntDupla[0] = PntDupla[1] = 0;
            Rodadas[0] = Rodadas[1] = Rodadas[2] = 3;
            VitoriasRodada[0] = VitoriasRodada[1] = 0;
            IdDuplaTruco = 3;
            IdDuplaMaoDeOnze = 3;
            MaoJogo = TipoMesa.Normal;
        }

        public bool VerificaVencedor(MesaNvTruco mesa)
        {
            var jogVencedor = RegraJogo.TestaValoresCartas(mesa.ListJogadoas);
            mesa.VencedorRodada = jogVencedor;

            if (jogVencedor != 0) // Vitoria
            {
                var retorno = ContabilizaRodadaVitoria(mesa, ValorPonto);
                if (retorno)
                {
                    mesa.JogadorVez = mesa.ProxJogadorVez(mesa.UltimoMao);
                    mesa.UltimoMao = mesa.JogadorVez;
                    mesa.PainelCtrl.AtualizaJogVezVenc(mesa.JogadorVez, mesa.VencedorRodada);
                    return true;
                }

                mesa.JogadorVez = jogVencedor;
                mesa.PainelCtrl.AtualizaJogVezVenc(mesa.JogadorVez, mesa.VencedorRodada);
                return false;
            }

            // Empate
            var result = ContabilizaRodadaEmpate(mesa, ValorPonto);
            if (result)
            {
                mesa.JogadorVez = mesa.ProxJogadorVez(mesa.UltimoMao);
                mesa.UltimoMao = mesa.JogadorVez;
            }

            mesa.PainelCtrl.AtualizaJogVezVenc(mesa.JogadorVez, mesa.VencedorRodada);
            return result;
        }

        public bool ContabilizaRodadaVitoria(MesaNvTruco mesa, TipoTruco tipoTruco)
        {
            var dupla = mesa.Duplas
                   .First(x => x.Jogador.Any(y => y.NumJogador == mesa.VencedorRodada));
            var ponto = int.Parse(tipoTruco.GetHashCode().ToString());

            switch (mesa.Rodada)
            {
                case 1:
                    Rodadas[0] = dupla.IdDupla;
                    //mesa.PainelCtrl.AdicionaPontosRodada(dupla.IdDupla, ponto);
                    mesa.PainelCtrl.AtualizaRodada(dupla.IdDupla);
                    return false;
                case 2:
                    if (Rodadas[0] == dupla.IdDupla || Rodadas[0] == 3)
                    {
                        mesa.PainelCtrl.AdicionaPontosRodada(dupla.IdDupla, ponto);
                        PntDupla[dupla.IdDupla] = PntDupla[dupla.IdDupla] + ponto;
                        return true;
                    }
                    Rodadas[1] = dupla.IdDupla;
                    mesa.PainelCtrl.AtualizaRodada(dupla.IdDupla);
                    //mesa.PainelCtrl.AdicionaPontosRodada(dupla.IdDupla, ponto);
                    return false;
                case 3:
                    mesa.PainelCtrl.AdicionaPontosRodada(dupla.IdDupla, ponto);
                    PntDupla[dupla.IdDupla] = PntDupla[dupla.IdDupla] + ponto;
                    return true;
                default:
                    return false;
            }
        }
        public bool ContabilizaRodadaEmpate(MesaNvTruco mesa, TipoTruco tipoTruco)
        {
            switch (mesa.Rodada)
            {
                case 1:
                    mesa.VencedorRodada = 8;
                    mesa.JogadorVez = mesa.ProxJogadorVez(mesa.JogadorVez);
                    mesa.PainelCtrl.AtualizaRodada(0);
                    mesa.PainelCtrl.AtualizaRodada(1);
                    return false;
                case 2:
                    if (Rodadas[0] == 3)
                    {
                        mesa.PainelCtrl.AtualizaRodada(0);
                        mesa.PainelCtrl.AtualizaRodada(1);
                        return false;
                    }

                    var a = mesa.Duplas.First(x => x.IdDupla == IdDupla[Rodadas[0]]);
                    var b = mesa.ListJogadoas
                        .First(x => x.Value == mesa.ListJogadoas.Max(y => y.Value) && (x.Key == a.Jogador[0].NumJogador || x.Key == a.Jogador[1].NumJogador))
                        .Key;
                    mesa.VencedorRodada = b;
                    PntDupla[Rodadas[0]] = PntDupla[Rodadas[0]] + (int.Parse(tipoTruco.GetHashCode().ToString()));
                    mesa.PainelCtrl.AdicionaPontosRodada(Rodadas[0], (int.Parse(tipoTruco.GetHashCode().ToString())));
                    return true;
                case 3:
                    if (Rodadas[1] != 3)
                    {
                        var a1 = mesa.Duplas.First(x => x.IdDupla == IdDupla[Rodadas[0]]);
                        var b2 = mesa.ListJogadoas
                            .First(x => x.Value == mesa.ListJogadoas.Max(y => y.Value) && (x.Key == a1.Jogador[0].NumJogador || x.Key == a1.Jogador[1].NumJogador))
                            .Key;
                        mesa.VencedorRodada = b2;
                        PntDupla[Rodadas[0]] = PntDupla[Rodadas[0]] + (int.Parse(tipoTruco.GetHashCode().ToString()));
                        mesa.PainelCtrl.AdicionaPontosRodada(Rodadas[0], (int.Parse(tipoTruco.GetHashCode().ToString())));
                    }
                    return true;
                default:
                    return false;
            }
        }
        public string[] ContabilizaCorrerTruco(MesaNvTruco mesa)
        {
            //var duplaPerdeu = mesa.Duplas.First(x => x.IdDupla != mesa.QuedasJogos.IdDuplaTruco);
            var duplaGanhou = mesa.Duplas.FirstOrDefault(x => x.IdDupla == mesa.QuedasJogos.IdDuplaTruco);

            if (duplaGanhou != null)
            {
                PntDupla[duplaGanhou.IdDupla] = PntDupla[duplaGanhou.IdDupla] +
                                                (int.Parse(ValorPonto.GetHashCode().ToString()));

                mesa.PainelCtrl.AdicionaPontosRodada(duplaGanhou.IdDupla, int.Parse(ValorPonto.GetHashCode().ToString()));

                var condFim = mesa.QuedasJogos.FechaQueda();

                mesa.JogadorVez = mesa.ProxJogadorVez(mesa.UltimoMao);
                mesa.UltimoMao = mesa.JogadorVez;
                mesa.PainelCtrl.AtualizaJogVezVenc(mesa.JogadorVez, mesa.VencedorRodada);
                return new[]
                {
                    PntDupla[0].ToString(),
                    PntDupla[1].ToString(),
                    VitoriasRodada[0].ToString(),
                    VitoriasRodada[1].ToString(),
                    mesa.JogadorVez.ToString()
                };
            }
            return null;
        }

        public void PedidoTruco(int pedidoTruco = 3)
        {
            ValorPonto = (TipoTruco)pedidoTruco;
        }

        public bool FechaQueda()
        {
            ValorPonto = TipoTruco.Normal;
            MaoJogo = TipoMesa.Normal;
            IdDuplaTruco = 3;
            Rodadas[0] = Rodadas[1] = Rodadas[2] = 3;
            IdDuplaMaoDeOnze = 3;
            if (PntDupla[0] == 11 && PntDupla[1] == 11)
            {
                MaoJogo = TipoMesa.MaoCega;
                return false;
            }
            for (var i = 0; i < PntDupla.Length; i++)
            {
                switch (PntDupla[i])
                {
                    case 12:
                        VitoriasRodada[i]++;
                        PntDupla[0] = 0;
                        PntDupla[1] = 0;
                        return true;
                    case 11:
                        IdDuplaMaoDeOnze = IdDupla[i];
                        MaoJogo = TipoMesa.MaoDeOnze;
                        ValorPonto = TipoTruco.Truco;
                        return false;
                }
            }
            return false;
        }
    }

    public class RegrasNvTruco
    {
        public int TestaValoresCartas(Dictionary<int, int> listJogadoas)
        {
            var jogadas = listJogadoas.Select(x => x.Value).ToList();
            var retorno = Rodada(jogadas);

            return retorno == 0
                ? retorno
                : listJogadoas.FirstOrDefault(x => x.Value == retorno).Key;
        }

        public int Rodada(List<int> valores)
        {
            if (valores[0] > valores[1] && valores[0] > valores[3] && valores[0] >= valores[2])
                return valores[0];
            if (valores[2] > valores[1] && valores[2] > valores[3] && valores[2] >= valores[0])
                return valores[2];
            if (valores[1] > valores[0] && valores[1] > valores[2] && valores[1] >= valores[3])
                return valores[1];
            if (valores[3] > valores[0] && valores[3] > valores[2] && valores[3] >= valores[1])
                return valores[3];

            return 0;
        }
    }

    public enum TipoTruco
    {
        Normal = 1,
        Truco = 3,
        Seis = 6,
        Nove = 9,
        Doze = 12
    }

    public enum TipoMesa
    {
        Normal = 1,
        MaoDeOnze = 11,
        MaoCega = 22
    }

    public static class FrasesTruco
    {
        public static List<string> Frase
        {
            get
            {
                return new List<string>()
                {
                    "","","",
                    "Truco Moço...","","",
                    "Seis Ladrão","","",
                    "Nove Franguinho","","",
                    "TacaLepau"
                };
            }
        }
    }

    public class ChatNvTrucoDois
    {
        public UsuarioNvTruco[] Usuarios { get; set; }
        public bool[] ChatAberto { get; set; }
        public string IdCHatDois { get; set; }
        public string Conversa { get; set; }
        public List<string> Conversa2 { get; set; }

        public ChatNvTrucoDois(UsuarioNvTruco[] usuarios, string idChat)
        {
            ChatAberto = new bool[2];
            IdCHatDois = idChat;
            Usuarios = new UsuarioNvTruco[2];
            Conversa2 = new List<string>();
            Usuarios[0] = usuarios[0];
            Usuarios[1] = usuarios[1];
            ChatAberto[0] = false;
            ChatAberto[1] = false;
            Conversa = "";
        }

        public void AdicionaConversa(string conversa)
        {
            Conversa += conversa; // Conversa == "" ? conversa : "[{;}]" + conversa;
        }

        public string RetornaConversa(string connectId, string conteudo)
        {
            for (var i = 0; i < 2; i++)
            {
                if (Usuarios[i].ConnectionId == connectId)
                    return ChatAberto[i] == false ? Conversa : conteudo;
            }

            return "";
        }

        public void AtvDesativaUsuario(string connectId, bool ativaDesativa)
        {
            for (var i = 0; i < 2; i++)
            {
                if (Usuarios[i].ConnectionId == connectId)
                    ChatAberto[i] = ativaDesativa;
            }

        }

    }

    public class PainelControle
    {
        public List<string[]> NomeJogador { get; set; }
        public int[] PontosDupla { get; set; }
        public int[] TotalPontosDupla { get; set; }
        public List<int[]> VitoriaRodada { get; set; }

        public int JogadorVez { get; set; }
        public int UltimoVencedor { get; set; }

        public PainelControle(string[] nomeJogador)
        {
            ZeraPainel(nomeJogador);
        }

        public void ZeraPainel(string[] nomeJogador)
        {
            PontosDupla = new[] { 0, 0 };
            TotalPontosDupla = new[] { 0, 0 };
            NomeJogador = new List<string[]>
            {
                new[] {nomeJogador[0], nomeJogador[1]},
                new[] {nomeJogador[2], nomeJogador[3]}
            };
            VitoriaRodada = new List<int[]>() { new[] { 0, 0 }, new[] { 0, 0 } };

            JogadorVez = 1;
            UltimoVencedor = 0;
        }

        public void AdicionaPontosRodada(int idDupla, int valor)
        {
            PontosDupla[idDupla] += valor;
            if (PontosDupla[idDupla] >= 12)
            {
                TotalPontosDupla[idDupla] += 1;
                ProximaQueda();
                return;
            }
            AtualizaRodada(idDupla);
        }
        public void AtualizaRodada(int idDupla)
        {
            var indice = VitoriaRodada[idDupla][0] == 0 ? 0 : 1;
            VitoriaRodada[idDupla][indice] = 1;
        }

        public void ZeraRodada()
        {
            VitoriaRodada = new List<int[]>() { new[] { 0, 0 }, new[] { 0, 0 } };
        }

        public void ProximaQueda()
        {
            VitoriaRodada = new List<int[]>() { new[] { 0, 0 }, new[] { 0, 0 } };
            UltimoVencedor = 0;
            PontosDupla = new[] { 0, 0 };
        }

        public void AtualizaJogVezVenc(int jogVez, int? ultimoVenc = null)
        {
            JogadorVez = jogVez;

            if (ultimoVenc != null)
                UltimoVencedor = (int)ultimoVenc;
        }
    }

}