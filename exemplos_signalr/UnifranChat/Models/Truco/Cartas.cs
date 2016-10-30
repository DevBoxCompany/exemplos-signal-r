using System.Collections.Generic;

namespace UnifranChat.Models.Truco
{
    public class Cartas
    {
        public string Naipe { get; set; }
        public string Valor { get; set; }
        public string EnderecoImg { get; set; }
        public bool EmUso { get; set; }

    }

    public class Baralho
    {
        public List<Cartas> LBaralho { get; set; }

        public Baralho()
        {
            LBaralho = new List<Cartas>();
            var valores = new[]
            {
                     "5", "6", "7", "8", "9", "10", "11", "12", "13", "17",
                "4", "5", "6",      "8", "9", "10", "11", "12", "13", "16",
                "4", "5", "6", "7", "8", "9", "10",       "12", "13", "15",
                "4", "5", "6",      "8", "9", "10", "11", "12", "13", "14"
            };

            for (var i = 0; i <= 39; i++)
            {
                #region ::: Gera Cartas :::

                if (i >= 0 && i <= 9)
                {
                    var ct = new Cartas()
                    {
                        Naipe = "Paus",
                        Valor = valores[i],
                        EmUso = false,
                        EnderecoImg = "Paus" + valores[i]
                    };
                    LBaralho.Add(ct);
                }
                else if (i >= 10 && i <= 19)
                {
                    var ct = new Cartas()
                    {
                        Naipe = "Copas",
                        Valor = valores[i],
                        EmUso = false,
                        EnderecoImg = "Copas" + valores[i]
                    };
                    LBaralho.Add(ct);
                }
                else if (i >= 20 && i <= 29)
                {
                    var ct = new Cartas()
                    {
                        Naipe = "Espada",
                        Valor = valores[i],
                        EmUso = false,
                        EnderecoImg = "Espada" + valores[i]
                    };
                    LBaralho.Add(ct);
                }
                else if (i >= 30 && i <= 39)
                {
                    var ct = new Cartas()
                    {
                        Naipe = "Ouro",
                        Valor = valores[i],
                        EmUso = false,
                        EnderecoImg = "Ouro" + valores[i]
                    };
                    LBaralho.Add(ct);
                } 

                #endregion
            }
        }

    }

}