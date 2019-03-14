using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class UFController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObterCidadesPorUF(int codigoUF)
        {
            Cidade Cidade = new Models.Cidade();

            List<Cidade> Cidades = new List<Cidade>();

            Cidades = Cidade.ObterCidadesPorUF(codigoUF);

            return Json(Cidades, JsonRequestBehavior.AllowGet);
        }
    }
}