namespace UnifranChat.Models.NovoTruco
{
    public class ViewModelMesa
    {
        public MesaNvTruco Mesa { get; set; }
        public string NumJogador { get; set; }

        public ViewModelMesa(MesaNvTruco mesa, string numJg)
        {
            Mesa = mesa;
            NumJogador = numJg;
        }
    }
}