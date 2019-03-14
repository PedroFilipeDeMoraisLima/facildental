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
        // GET: Agenda
        public ActionResult Index()
        {
            Paciente p = new Paciente();

            ViewBag.Pacientes = new SelectList(p.ObterPacientes(), "Codigo", "Nome");

            Tratamento T = new Tratamento();

            ViewBag.Tratamentos = new SelectList(T.ObterTratamentos(), "Codigo", "DescricaoComValor");

            Agenda Agenda = new Models.Agenda();

            ViewBag.StatusAgendamento = Agenda.ObterSelectItemStatus(); 

            return View();
        }

        [HttpGet]
        public JsonResult ObterAgendamentosJson()
        {
            AgendaJson AgendaJson = new AgendaJson();

            List<AgendaJson> Agendamentos = new List<AgendaJson>();

            Agendamentos = AgendaJson.ObterAgendamentosJson();

            return Json(Agendamentos, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ObterAgendaJson(int Codigo)
        {
            Agenda AgendaJson = new Agenda();

            AgendaJson = AgendaJson.ObterAgendamentoPorCodigo(Codigo);

            return Json(AgendaJson, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RealizarAgendamento(Agenda Agenda)
        {
            try
            {
                Agenda.Cadastrar();

                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Index");
            }

        }

        public ActionResult AtualizarOuExcluirAgendamento(int v_id, DateTime v_data_inicial, DateTime v_data_final, int CodigoTratamento,  
            string StatusAgendamento, FormCollection Collection)
        {
            try
            {

                string Acao = Collection.Get("txtAcao");

                if (Acao.Equals("excluir"))
                {
                    Agenda Agenda = new Models.Agenda();

                    Agenda.Excluir(v_id);
                }
                else
                {
                    Agenda Agenda = new Models.Agenda();

                    Agenda.Atualizar(v_id, v_data_inicial, v_data_final, CodigoTratamento, StatusAgendamento);
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

                a.AtualizarStatus(CodigoAgendamento, Status);

                TempData["MsgSucesso"] = "Status do agendamento atualizado com sucesso.";

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoPaciente });
            }
            catch (Exception Ex)
            {
                TempData["MsgErro"] = Ex.Message;

                return RedirectToAction("Detalhe", "Prontuario", new { Codigo = CodigoPaciente });
            }
        }
    }
}