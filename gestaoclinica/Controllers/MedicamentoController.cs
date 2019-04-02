using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class MedicamentoController : Controller
    {
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Medicamento m = new Medicamento();

            ViewBag.Medicamentos = m.ObterMedicamentos(ObterCodigoClinicaUsuarioLogado(), pesquisa);
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarMedicamento(Medicamento m)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    m.Cadastrar(m, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Medicamento cadastrado com sucesso.";

                    return RedirectToAction("Index", "Medicamento");
                }
                else
                {
                    Medicamento MedicamentoView = new Medicamento();

                    ViewBag.Medicamentos = MedicamentoView.ObterMedicamentos(ObterCodigoClinicaUsuarioLogado());

                    return View("Index");
                }
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Medicamento");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AtualizarMedicamento(Medicamento m)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    m.Atualizar(m, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Medicamento atualizado com sucesso.";

                    return RedirectToAction("Index", "Medicamento");
                }
                else
                {
                    Medicamento MedicamentoView = new Medicamento();

                    ViewBag.Medicamentos = MedicamentoView.ObterMedicamentos(ObterCodigoClinicaUsuarioLogado());

                    return View("Index");
                }
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Medicamento");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirMedicamento(int Codigo)
        {
            try
            {
                Medicamento m = new Medicamento();

                m.Excluir(Codigo, ObterCodigoClinicaUsuarioLogado());

                TempData["MsgSucesso"] = "Medicamento excluido com sucesso.";

                return RedirectToAction("Index", "Medicamento");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Medicamento");
            }
        }

        [HttpGet]
        public JsonResult ObterMedicamentoJson(int Codigo)
        {
            Medicamento m = new Medicamento(Codigo, ObterCodigoClinicaUsuarioLogado());

            return Json(m, JsonRequestBehavior.AllowGet);
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}