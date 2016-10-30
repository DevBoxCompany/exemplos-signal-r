using System.Collections.Generic;

namespace UnifranChat.Models.NovoTruco
{
    public class ViewModelSalaEspera
    {
        public NovoTruco NovoTruco { get; set; }
        public int PaginaSala { get; set; }

        public ViewModelSalaEspera()
        {
            NovoTruco = new NovoTruco();
        }
    }
}