using System.Collections.Generic;

namespace UnifranChat.Models.Truco
{
    public class TrChat
    {
        public string UsuarioId { get; set; }
        public string UsuarioNome { get; set; }
    }

    public class ListConversa
    {
        //public List<TrChat> ListConv { get; set; }
        public string IdConversa { get; set; }
        public string IdConteudo { get; set; }
        public Status ChatStatus { get; set; }
        public string UsuarioRemId { get; set; }
        public string UsuarioRemNome { get; set; }
        public string UsuarioConvId { get; set; }
        public string UsuarioConvNome { get; set; }

        //public ListConversa(Dictionary<string,string> usua)
        //{
        //    //ListConv = new List<TrChat>();
        //    ChatStatus = Status.Espera;

        //    //foreach (var u in usua)
        //    //{
        //    //    ListConv.Add(new TrChat()
        //    //    {
        //    //        UsuarioId = u.Key,
        //    //        UsuarioNome = u.Value
        //    //    });
        //    //}

        //}
    }

    public enum Status
    {
        Espera,
        Ativo,
        Desativo
    }
}