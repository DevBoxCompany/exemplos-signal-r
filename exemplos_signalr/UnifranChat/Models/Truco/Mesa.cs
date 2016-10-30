using System;
using System.Collections.Generic;
using System.Web.UI;

namespace UnifranChat.Models.Truco
{
    public class Mesa
    {
        public string MesaId { get; set; }
        public string Jogador1 { get; set; }
        public string Jogador2 { get; set; }
        public bool JogoAtivo { get; set; }

        public int Rodada { get; set; }

        public List<int> LJogada123 { get; set; }

        public int PntJog1 { get; set; }
        public int PntJog2 { get; set; }

        public int SaldoPntJog1 { get; set; }
        public int SaldoPntJog2 { get; set; }

        public int UltimoComeco { get; set; }

        public Baralho Baralhos { get; set; }
        public List<int> Cartas1Jog { get; set; }
        public List<Cartas> ListaCartasJg { get; set; }


        public enum EJogada
        {
            Vitoria1,
            Vitoria2,
            Empate
        }

        public Dictionary<int, int> PntJogada { get; set; }

        public Mesa()
        {
            LJogada123 = new List<int>() { 0,0,0 };
            PntJogada = new Dictionary<int, int>();
            PntJog1 = 0;
            PntJog2 = 0;
            Rodada = 1;
            JogoAtivo = true;
            UltimoComeco = 1;
            Baralhos = new Baralho();
        }
    }
}