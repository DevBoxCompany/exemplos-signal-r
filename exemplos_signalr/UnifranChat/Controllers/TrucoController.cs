using System;
using System.Web.Mvc;
using UnifranChat.Hubs;
using UnifranChat.Models.Truco.ViewModel;

namespace UnifranChat.Controllers
{
    public class TrucoController : Controller
    {
        [HttpPost]
        public ActionResult Index(FormCollection form)
        {
            JovoTruco.NomeJogador = form["txtName"];
            return View();
        }
        public ActionResult AtivaMesa(string mesa, string cartas)
        {
            var ms = new MesaViewModel(cartas)
            {
                MesaId = mesa.Split(',')[0],
                NumJogador = mesa.Split(',')[1],
                JogVez = mesa.Split(',')[2]
            };

            return View("Truco/_TrucoMesa", ms);
        }
    }
}