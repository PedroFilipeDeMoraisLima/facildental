using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;


namespace gestaoclinica.Models
{
    public class AgendaJson : Agenda
    {
        public int id { get; set; }
        public int id_tratamento { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string color { get; set; }
        public string textColor { get; set; }

        public AgendaJson() { }

        public List<AgendaJson> ObterAgendamentosJson(int CodigoClinica, int CodigoProfissional)
        {
            List<AgendaJson> Agendamentos = new List<AgendaJson>();

            foreach (Agenda ObjAgenda in this.ObterAgendamentos(CodigoClinica, CodigoProfissional))
            {
                Agendamentos.Add(new AgendaJson() 
                {
                    id = ObjAgenda.Codigo,
                    id_tratamento = ObjAgenda.Tratamento.Codigo,
                    title = ObjAgenda.Paciente.Nome,
                    start = ObjAgenda.DataHoraInicial.ToString("yyyy-MM-dd") + "T" + ObjAgenda.DataHoraInicial.ToString("HH:mm:ss"),
                    end = ObjAgenda.DataHoraFinal.ToString("yyyy-MM-dd") + "T" + ObjAgenda.DataHoraFinal.ToString("HH:mm:ss"),
                    color = ObjAgenda.BgColor,
                    textColor = ObjAgenda.TextColor
                });
            }

            return Agendamentos;
        }
        
    }
}