using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class ConvenioController : Controller
    {
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Convenio c = new Convenio();

            ViewBag.Convenios = c.ObterConvenios(ObterCodigoClinicaUsuarioLogado(), pesquisa);

            return View();
        }

        [HttpPost]
        public ActionResult Cadastrar(Convenio c)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    c.Cadastrar(c, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Convênio cadastrado com sucesso.";

                    return RedirectToAction("Index", "Convenio");
                }
                else
                {
                    return View("Index");
                }
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Convenio");
            }
        }

        [HttpPost]
        public ActionResult Atualizar(Convenio c)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    c.Atualizar(c, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Convênio atualizado com sucesso.";

                    return RedirectToAction("Index", "Convenio");
                }
                else
                {
                    return View();
                }

            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Convenio");
            }
        }

        [HttpPost]
        public ActionResult Excluir(int Codigo)
        {
            try
            {
                Convenio c = new Convenio();

                c.Excluir(Codigo, ObterCodigoClinicaUsuarioLogado());

                TempData["MsgSucesso"] = "Convênio excluído com sucesso.";

                return RedirectToAction("Index", "Convenio");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Convenio");
            }
        }

        [HttpGet]
        public JsonResult ObterConvenioJson(int Codigo)
        {
            Convenio c = new Convenio(Codigo, ObterCodigoClinicaUsuarioLogado());

            return Json(c, JsonRequestBehavior.AllowGet);
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}