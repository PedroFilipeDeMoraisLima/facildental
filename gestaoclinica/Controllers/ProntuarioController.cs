using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class ProntuarioController : Controller
    {
        // GET: Prontuario
        [HttpGet]
        public ActionResult Index(string pesquisa = "")
        {
            Paciente p = new Paciente();

            ViewBag.Pacientes = p.ObterPacientes(pesquisa);

            return View();
        }

        [Route("Prontuario/Detalhe/{Codigo}")]
        [HttpGet]
        public ActionResult Detalhe(int Codigo)
        {
            Prontuario p = new Prontuario(Codigo);

            p.CarregarEvolucoes();

            p.Paciente.CarregarAgendamentos();

            ViewBag.Prontuario = p;

            Cidade CidadePaciente = new Cidade(p.Paciente.CodigoCidade);

            UF UFPaciente = new UF(CidadePaciente.CodigoUF);

            ViewBag.CidadePaciente = CidadePaciente;
            ViewBag.UFPaciente = UFPaciente;

            ViewBag.UsuarioLogado = Session["UsuarioLogado"];
            ViewBag.DataHoraCorrente = Firebird.DataHoraServidor().ToString("dd/MM/yyyy HH:mm");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CadastrarEvolucao(int CodigoProntuarioEvolucao, int CodigoProfissionalEvolucao, DateTime DataHoraEvolucao, string DescricaoEvolucao)
        {
            try
            {
                Evolucao e = new Evolucao();

                e.CodigoProntuario = CodigoProfissionalEvolucao;
                e.Profissional = new Usuario() { Codigo = CodigoProfissionalEvolucao };
                e.DataHoraEvolucao = DataHoraEvolucao;
                e.Descricao = DescricaoEvolucao;

                e.Cadastrar(e);

                TempData["MsgSucesso"] = "Evolução do paciente cadastrada com sucesso.";

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoProntuarioEvolucao });
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoProntuarioEvolucao });
            }
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}