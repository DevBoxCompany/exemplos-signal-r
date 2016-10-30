using System.Collections.Generic;

namespace UnifranChat.Models.Truco.ViewModel
{
    public class MesaViewModel
    {
        public string MesaId { get; set; }
        public string NumJogador { get; set; }
        public string JogVez { get; set; }

        public List<Cartas> Cartas { get; set; }

        public MesaViewModel(string jsonCartas)
        {
            var crts = jsonCartas.Split(';');
            Cartas = new List<Cartas>();

            foreach (var crt in crts)
            {
                var a = new Cartas()
                {
                    Valor = crt.Split('-')[0],
                    EnderecoImg = crt.Split('-')[1]
                };
                Cartas.Add(a);
            }
        }
    }
}