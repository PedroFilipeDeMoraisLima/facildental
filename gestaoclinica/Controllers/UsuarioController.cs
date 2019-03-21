using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Controllers;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class UsuarioController : Controller
    {
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Usuario u = new Usuario();

            ViewBag.Usuarios = u.ObterUsuarios(ObterCodigoClinicaUsuarioLogado(), pesquisa);

            return View();
        }

        [HttpGet]
        public ActionResult Cadastro()
        {
            return View();
        }

        [HttpGet]
        [Route("Usuario/Edicao/{Codigo}")]
        public ActionResult Edicao(int Codigo)
        {
            Usuario u = new Usuario(Codigo, ObterCodigoClinicaUsuarioLogado());

            ViewBag.Usuario = u;

            return View();
        }

        [HttpGet]
        [Route("Usuario/Detalhe/{Codigo}")]
        public ActionResult Detalhe(int Codigo)
        {
            Usuario u = new Usuario(Codigo, ObterCodigoClinicaUsuarioLogado());

            ViewBag.Usuario = u;

            return View();
        }

        [HttpPost]
        public ActionResult RealizarCadastro(Usuario u)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    u.Cadastrar(u, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Usuário cadastrado com sucesso.";
                
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Cadastro");
                }

            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RealizarEdicao(Usuario u)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    u.Atualizar(u, ObterCodigoClinicaUsuarioLogado());

                    TempData["MsgSucesso"] = "Alteração realizada com sucesso.";

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Usuario = u;

                    return View("Edicao");
                }

            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarSenha(int Codigo, string Senha)
        {
            try
            {
                Usuario u = new Usuario();

                u.AlterarSenha(Codigo, Senha, ObterCodigoClinicaUsuarioLogado());
                
                TempData["MsgSucesso"] = "Senha do usuário alterada com sucesso.";

                return RedirectToAction("Index", "Usuario");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index", "Usuario");
            }
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}