using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class TratamentoController : Controller
    {
        // GET: Tratamento
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Tratamento Tratamento = new Tratamento();

            ViewBag.Tratamentos = Tratamento.ObterTratamentos(pesquisa);

            return View();
        }

        [HttpGet]
        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpGet]
        [Route("Tratamento/Detalhe/{Codigo}")]
        public ActionResult Detalhe(int Codigo)
        {
            Tratamento Tratamento = new Tratamento(Codigo);

            ViewBag.Tratamento = Tratamento;

            return View();
        }

        [HttpGet]
        [Route("Tratamento/Exclusao/{Codigo}")]
        public ActionResult Exclusao(int Codigo)
        {
            Tratamento Tratamento = new Tratamento(Codigo);

            ViewBag.Tratamento = Tratamento;

            return View();
        }

        [HttpGet]
        [Route("Tratamento/Edicao/{Codigo}")]
        public ActionResult Edicao(int Codigo)
        {
            Tratamento Tratamento = new Tratamento(Codigo);

            ViewBag.Tratamento = Tratamento;

            return View();
        }

        [HttpPost]
        public ActionResult RealizarCadastro(Tratamento Tratamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Tratamento.Cadastrar();

                    TempData["MsgSucesso"] = "Tratamento cadastrado com sucesso.";

                    return RedirectToAction("Index", "Tratamento");
                }
                else
                {
                    return View("Cadastro");
                }
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Tratamento");
            }
        }

        [HttpPost]
        public ActionResult RealizarEdicao(Tratamento Tratamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Tratamento.Atualizar();

                    TempData["MsgSucesso"] = "Tratamento atualizado com sucesso.";

                    return RedirectToAction("Index", "Tratamento");
                }
                else
                {
                    ViewBag.Tratamento = Tratamento;

                    return View("Edicao");
                }
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Tratamento");
            }
        }

        [HttpPost]
        public ActionResult RealizarExclusao(int Codigo)
        {
            try
            {
                Tratamento Tratamento = new Tratamento();

                Tratamento.Excluir(Codigo);

                TempData["MsgSucesso"] = "Tratamento excluido com sucesso.";

                return RedirectToAction("Index", "Tratamento");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Exclusao", "Tratamento");
            }
        }
    }
}