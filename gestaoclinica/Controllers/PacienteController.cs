﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using gestaoclinica.Models;


namespace gestaoclinica.Controllers
{
    public class PacienteController : Controller
    {
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Paciente p = new Paciente();

            ViewBag.Pacientes = p.ObterPacientes(ObterCodigoClinicaUsuarioLogado(), pesquisa);
            
            return View();
        }

        [Route("Paciente/Detalhe/{Codigo}")] [HttpGet]
        public ActionResult Detalhe(int Codigo)
        {
            Paciente Paciente = new Paciente(Codigo, ObterCodigoClinicaUsuarioLogado());

            Cidade CidadePaciente = new Cidade(Paciente.CodigoCidade);

            UF UFPaciente = new UF(CidadePaciente.CodigoUF);

            ViewBag.Paciente = Paciente;
            ViewBag.CidadePaciente = CidadePaciente;
            ViewBag.UFPaciente = UFPaciente;

            return View();
        }

        [Route("Paciente/Cadastro")] [HttpGet]
        public ActionResult Cadastro()
        {
            UF UF = new UF();

            ViewBag.UFS = new SelectList(UF.ObterRegistros().ToList(), "Codigo", "Sigla");

            List<Cidade> Cidades = new List<Cidade>();

            ViewBag.Cidades = new SelectList(Cidades, "Codigo", "Descricao");

            return View();
        }

        [Route("Paciente/Edicao/{Codigo}")]
        [HttpGet]
        public ActionResult Edicao(int Codigo)
        {
            Paciente Paciente = new Paciente(Codigo, ObterCodigoClinicaUsuarioLogado());

            ViewBag.Paciente = Paciente;

            UF UF = new UF();

            UF = UF.ObterUFPorCidade(Paciente.CodigoCidade);

            ViewBag.UFS = new SelectList(UF.ObterRegistros().ToList(), "Codigo", "Sigla", UF.Codigo);

            Cidade Cidade = new Models.Cidade();

            ViewBag.Cidades = new SelectList(Cidade.ObterCidadesPorUF(UF.Codigo), "Codigo", "Descricao", Paciente.CodigoCidade);

            return View();
        }

        [Route("Paciente/Exclusao/{Codigo}")]
        [HttpGet]
        public ActionResult Exclusao(int Codigo)
        {
            Paciente Paciente = new Paciente(Codigo, ObterCodigoClinicaUsuarioLogado());

            Cidade CidadePaciente = new Cidade(Paciente.CodigoCidade);

            UF UFPaciente = new UF(CidadePaciente.Codigo);

            ViewBag.Paciente = Paciente;
            ViewBag.CidadePaciente = CidadePaciente;
            ViewBag.UFPaciente = UFPaciente;

            return View();
        }
        
        [HttpPost]
        public ActionResult RealizarCadastro(Paciente p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    p.Cadastrar(p, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Paciente cadastrado com sucesso.";

                    return RedirectToAction("Detalhe", "Prontuario", new { Codigo = p.Codigo });
                }
                else
                {
                    UF UF = new UF();

                    ViewBag.UFS = new SelectList(UF.ObterRegistros().ToList(), "Codigo", "Sigla");

                    List<Cidade> Cidades = new List<Cidade>();

                    ViewBag.Cidades = new SelectList(Cidades, "Codigo", "Descricao");

                    return View("Cadastro");
                }

            }
            catch (Exception e)
            {
                TempData["MsgErro"] = e.Message;

                return RedirectToAction("Index", "Prontuario");
            }

        }

        [HttpPost]
        public ActionResult RealizarEdicao(Paciente p)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    p.Atualizar(p, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Alterações realizadas com sucesso.";

                    return RedirectToAction("Detalhe", "Prontuario", new { Codigo = p.Codigo });
                }
                else
                {
                    Paciente Paciente = new Paciente(p.Codigo, ObterCodigoClinicaUsuarioLogado());

                    ViewBag.Paciente = Paciente;

                    UF UF = new UF();

                    UF = UF.ObterUFPorCidade(Paciente.CodigoCidade);

                    ViewBag.UFS = new SelectList(UF.ObterRegistros().ToList(), "Codigo", "Sigla", UF.Codigo);

                    Cidade Cidade = new Models.Cidade();

                    ViewBag.Cidades = new SelectList(Cidade.ObterCidadesPorUF(UF.Codigo), "Codigo", "Descricao", Paciente.CodigoCidade);

                    return View("Edicao");
                }

            }
            catch (Exception e)
            {
                TempData["MsgErro"] = e.Message;

                return RedirectToAction("Index", "Paciente");
            }
        }

        [HttpPost]
        public ActionResult RealizarExclusao(Paciente p)
        {
            try
            {
                p.Excluir(p, ObterCodigoClinicaUsuarioLogado());

                TempData["MsgSucesso"] = "Exclusão realizada com sucesso.";

                return RedirectToAction("Index", "Prontuario");
            }
            catch (Exception e)
            {
                TempData["MsgErro"] = e.Message;

                return RedirectToAction("Index", "Prontuario");
            }
        }

        public JsonResult ObterCidadesPorUF(int codigoUF)
        {
            Cidade Cidade = new Models.Cidade();

            List<Cidade> Cidades = new List<Cidade>();

            Cidades = Cidade.ObterCidadesPorUF(codigoUF);

            return Json(Cidades, JsonRequestBehavior.AllowGet);
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}
