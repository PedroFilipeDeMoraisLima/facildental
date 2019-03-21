using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class ClinicaController : Controller
    {
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult CadastrarClinica(string nomeClinica, string telefoneClinica, string nomeUsuario, string celularUsuario, string emailUsuario, string senhaUsuario)
        {
            try
            {
                Clinica c = new Clinica();

                c.Nome = nomeClinica;
                c.Telefone = telefoneClinica;

                c.Cadastrar(c);

                Usuario u = new Usuario();

                u.Nome = nomeUsuario;
                u.Celular = celularUsuario;
                u.Email = emailUsuario;
                u.Senha = senhaUsuario;
                u.Perfil = Usuario.ePerfil.Administrador_Clinica;

                u.Cadastrar(u, c.Codigo);

                Session["UsuarioLogado"] = u;
                Session["CodigoClinica"] = c.Codigo;

                return RedirectToAction("Index", "Home");
            }
            catch (Exception Ex)
            {
                TempData["MensagemValidacao"] = Ex.Message;

                return RedirectToAction("Index", "Login");
            }
        }

        
    }
}