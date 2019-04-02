using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using gestaoclinica.Models;

namespace gestaoclinica.Controllers
{
    public class AgendaController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            Paciente p = new Paciente();

            ViewBag.Pacientes = new SelectList(p.ObterPacientes(ObterCodigoClinicaUsuarioLogado()), "Codigo", "Nome");

            Tratamento T = new Tratamento();

            ViewBag.Tratamentos = new SelectList(T.ObterTratamentos(ObterCodigoClinicaUsuarioLogado()), "Codigo", "Descricao");

            Agenda Agenda = new Models.Agenda();

            ViewBag.StatusAgendamento = Agenda.ObterSelectItemStatus();

            Usuario u = new Usuario();

            ViewBag.Profissionais = new SelectList(u.ObterUsuarios(ObterCodigoClinicaUsuarioLogado()), "Codigo", "Nome");

            var UsuarioLogado = @Session["UsuarioLogado"] as gestaoclinica.Models.Usuario;

            ViewBag.SelecaoAgenda = u.ObterSelectItemProfissionaisAgenda(ObterCodigoClinicaUsuarioLogado(), UsuarioLogado.Clinica.Nome);

            return View();
        }

        [HttpGet]
        public JsonResult ObterAgendamentosJson(int CodigoProfissional = 0)
        {
            AgendaJson AgendaJson = new AgendaJson();

            List<AgendaJson> Agendamentos = new List<AgendaJson>();

            Agendamentos = AgendaJson.ObterAgendamentosJson(ObterCodigoClinicaUsuarioLogado(), CodigoProfissional);

            return Json(Agendamentos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObterAgendaJson(int Codigo)
        {
            Agenda AgendaJson = new Agenda();

            AgendaJson = AgendaJson.ObterAgendamentoPorCodigo(Codigo, ObterCodigoClinicaUsuarioLogado());

            return Json(AgendaJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RealizarAgendamento(Agenda Agenda)
        {
            try
            {
                Agenda.Cadastrar(ObterCodigoClinicaUsuarioLogado());

                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index");
            }

        }

        public ActionResult AtualizarOuExcluirAgendamento(int v_id, DateTime v_data_inicial, DateTime v_data_final, int CodigoTratamento,  
            string StatusAgendamento, int CodigoProfissional, FormCollection Collection)
        {
            try
            {

                string Acao = Collection.Get("txtAcao");

                if (Acao.Equals("excluir"))
                {
                    Agenda Agenda = new Models.Agenda();

                    Agenda.Excluir(v_id, ObterCodigoClinicaUsuarioLogado());
                }
                else
                {
                    Agenda Agenda = new Models.Agenda();

                    Agenda.Atualizar(v_id, v_data_inicial, v_data_final, CodigoTratamento, StatusAgendamento, 
                        CodigoProfissional, ObterCodigoClinicaUsuarioLogado());
                }

                return RedirectToAction("Index");

            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index");
            }
        }

        public ActionResult AtualizarStatus(int CodigoPaciente, int CodigoAgendamento, string Status)
        {
            try
            {
                Agenda a = new Agenda();

                a.AtualizarStatus(CodigoAgendamento, Status, ObterCodigoClinicaUsuarioLogado());

                TempData["MsgSucesso"] = "Status do agendamento atualizado com sucesso.";

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoPaciente });
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoPaciente });
            }
        }

        private int ObterCodigoClinicaUsuarioLogado()
        {
            return int.Parse(Session["CodigoClinica"].ToString());
        }

    }
}