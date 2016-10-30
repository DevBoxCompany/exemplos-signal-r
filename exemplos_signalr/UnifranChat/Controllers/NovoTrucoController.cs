using System.Linq;
using System.Web.Mvc;
using UnifranChat.Hubs;
using UnifranChat.Models.NovoTruco;

namespace UnifranChat.Controllers
{
    public class NovoTrucoController : Controller
    {
        //[HttpPost]
        public ActionResult NovoTruco()//FormCollection form)
        {
            var nomeTest = "Teste";
            HubNovoTruco.NomeJogador = nomeTest;// form["txtName"];
            var cont = 1;
            while (HubNovoTruco.NovoTruco.ListUsuarios.Any(x => x.Nome == HubNovoTruco.NomeJogador))
            {
                HubNovoTruco.NomeJogador = nomeTest + "(" + cont + ")";//form["txtName"] + "(" + cont + ")";
                cont++;
            }
            return View();
        }

        public ActionResult CarregaJogo(string pagina)
        {
            var retorno = new ViewModelSalaEspera()
            {
                NovoTruco = HubNovoTruco.NovoTruco,
                PaginaSala = int.Parse(pagina)
            };
            return View("NovoTruco/_NovoTrucoMesas", retorno);
        }

        public ActionResult CarregaMesa(string idMesa, string numJogador)
        {
            var model =
                HubNovoTruco.NovoTruco.ListSalas.Select(x => x.ListaMesas.FirstOrDefault(y => y.MesaId == idMesa))
                    .FirstOrDefault();

            var viewModel = new ViewModelMesa(model, numJogador);

            return View("NovoTruco/_NovoTruco", viewModel);
        }

        public ActionResult ModalPanelControle(string idMesa, bool zeraRodada)
        {
            var mesa =
                HubNovoTruco
                    .NovoTruco.ListSalas.Select(x => x.ListaMesas).ToList()
                    .Select(x => x.FirstOrDefault(y => y.MesaId == idMesa)).FirstOrDefault();

            if (mesa != null && zeraRodada)
                mesa.PainelCtrl.ZeraRodada();

            return mesa != null
                ? View("NovoTruco/_PainelTruco", mesa.PainelCtrl)
                : null;
        }
    }
}