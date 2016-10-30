using System;
using System.ComponentModel.DataAnnotations;

namespace UnifranChat.Models.CtrlFinanceiro
{
    public class ViewModelCadCredito
    {
        [ScaffoldColumn(false)]
        public int CodCred { get; set; }
        [ScaffoldColumn(false)]
        public string TipoConta { get; set; }

        [Required]
        [Display(Name = "Valor a Receber :")]
        public string ValorReceber { get; set; }
        [Required]
        [Display(Name = "Parcelas:")]
        public int Parcelas { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Data do Recebimento")]
        public DateTime DataRecebimento { get; set; }
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nome do devedor")]
        public string NomeDevedor { get; set; }
 
    }
}