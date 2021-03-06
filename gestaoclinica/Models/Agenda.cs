﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using gestaoclinica.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;


namespace gestaoclinica.Models
{
    public class Agenda
    {
        [Required]
        public int Codigo { get; set; }

        [Required]
        public Usuario Profissional { get; set; }

        public Paciente Paciente { get; set; }

        public string Titulo { get; set; }

        public DateTime DataHoraInicial { get; set; }

        public DateTime DataHoraFinal { get; set; }

        public string BgColor { get; set; }

        public string TextColor { get; set; }

        public Tratamento Tratamento { get; set; }

        public string Status { get; set; }

        public Agenda() { }

        public List<Agenda> ObterAgendamentos(int CodigoClinica, int CodigoPaciente = 0, int CodigoProfissional = 0)
        {
            List<Agenda> Agendamentos = new List<Agenda>();

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  SELECT 
                                        * 
                                        FROM 
                                        AGENDA INNER JOIN PACIENTE ON (A_PACIENTE = P_CODIGO) 
                                        INNER JOIN TRATAMENTO ON (A_TRATAMENTO = T_CODIGO)
                                        INNER JOIN USUARIO ON (A_PROFISSIONAL = U_CODIGO)
                                        WHERE
                                        A_CLINICA =@A_CLINICA ";

                    if (CodigoProfissional != 0)
                    {
                        TxtSQL += "AND A_PROFISSIONAL =@A_PROFISSIONAL ";
                    }

                    if (CodigoPaciente != 0)
                    {
                        TxtSQL += "AND P_CODIGO =@P_CODIGO ";
                    }

                    TxtSQL += "ORDER BY A_DATAINICIAL DESC";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("A_CLINICA", CodigoClinica);

                        if (CodigoPaciente != 0)
                        {
                            cmdSelect.Parameters.AddWithValue("P_CODIGO", CodigoPaciente);
                        }

                        if (CodigoProfissional != 0)
                        {
                            cmdSelect.Parameters.AddWithValue("A_PROFISSIONAL", CodigoProfissional);
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Agenda Agenda = new Models.Agenda();

                                Agenda.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("A_CODIGO"));
                                Agenda.Paciente = new Paciente()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("P_CODIGO")), Nome = drSelect.GetString(drSelect.GetOrdinal("P_NOME"))
                                };
                                Agenda.Titulo = Agenda.Paciente.Nome;
                                Agenda.DataHoraInicial = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("A_DATAINICIAL")).Replace("00:00:00", 
                                    drSelect.GetString(drSelect.GetOrdinal("A_HORAINICIAL")))));
                                Agenda.DataHoraFinal = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("A_DATAFINAL")).Replace("00:00:00",
                                    drSelect.GetString(drSelect.GetOrdinal("A_HORAFINAL")))));
                                Agenda.BgColor = drSelect.GetString(drSelect.GetOrdinal("A_BGCOLOR"));
                                Agenda.TextColor = drSelect.GetString(drSelect.GetOrdinal("A_TEXTCOLOR"));
                                Agenda.Tratamento = new Models.Tratamento
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("T_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("T_DESCRICAO"))
                                };
                                Agenda.Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("A_STATUS")));
                                Agenda.Profissional = new Usuario()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL"))
                                };

                                Agendamentos.Add(Agenda);
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
            
            return Agendamentos;
        }

        public Agenda ObterAgendamentoPorCodigo(int Codigo, int CodigoClinica)
        {
            Agenda Agenda = new Agenda();

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  SELECT 
                                        * 
                                        FROM 
                                        AGENDA INNER JOIN PACIENTE ON (A_PACIENTE = P_CODIGO) 
                                        INNER JOIN TRATAMENTO ON (A_TRATAMENTO = T_CODIGO)
                                        INNER JOIN USUARIO ON (A_PROFISSIONAL = U_CODIGO)
                                        WHERE
                                        A_CODIGO =@A_CODIGO AND
                                        A_CLINICA =@A_CLINICA";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("A_CODIGO", Codigo);
                        cmdSelect.Parameters.AddWithValue("A_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                Agenda.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("A_CODIGO"));
                                Agenda.Paciente = new Paciente()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("P_CODIGO")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("P_NOME"))
                                };
                                Agenda.Titulo = Agenda.Paciente.Nome;
                                Agenda.DataHoraInicial = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("A_DATAINICIAL")).Replace("00:00:00",
                                    drSelect.GetString(drSelect.GetOrdinal("A_HORAINICIAL")))));
                                Agenda.DataHoraFinal = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("A_DATAFINAL")).Replace("00:00:00",
                                    drSelect.GetString(drSelect.GetOrdinal("A_HORAFINAL")))));
                                Agenda.BgColor = drSelect.GetString(drSelect.GetOrdinal("A_BGCOLOR"));
                                Agenda.TextColor = drSelect.GetString(drSelect.GetOrdinal("A_TEXTCOLOR"));
                                Agenda.Tratamento = new Models.Tratamento
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("T_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("T_DESCRICAO"))
                                };
                                Agenda.Status = drSelect.GetString(drSelect.GetOrdinal("A_STATUS"));
                                Agenda.Profissional = new Usuario()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL"))
                                };
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }

            return Agenda;
        }

        public void Cadastrar(int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    CorAgenda CorAgenda = new Models.CorAgenda();

                    CorAgenda = CorAgenda.ObterCorAgendaRandomica();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        AGENDA
                                        (
                                            A_CODIGO,
                                            A_PACIENTE,
                                            A_DATAINICIAL,
                                            A_HORAINICIAL,
                                            A_DATAFINAL,
                                            A_HORAFINAL,
                                            A_BGCOLOR,
                                            A_TEXTCOLOR,
                                            A_TRATAMENTO,
                                            A_STATUS,
                                            A_CLINICA,
                                            A_PROFISSIONAL
                                        )
                                        VALUES
                                        (
                                            @A_CODIGO,
                                            @A_PACIENTE,
                                            @A_DATAINICIAL,
                                            @A_HORAINICIAL,
                                            @A_DATAFINAL,
                                            @A_HORAFINAL,
                                            @A_BGCOLOR,
                                            @A_TEXTCOLOR,
                                            @A_TRATAMENTO,
                                            @A_STATUS,
                                            @A_CLINICA,
                                            @A_PROFISSIONAL
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdInsert.Parameters.AddWithValue("A_CODIGO", this.GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("A_PACIENTE", this.Paciente.Codigo);
                        cmdInsert.Parameters.AddWithValue("A_DATAINICIAL", this.DataHoraInicial.Date);
                        cmdInsert.Parameters.AddWithValue("A_HORAINICIAL", this.DataHoraInicial.TimeOfDay);
                        cmdInsert.Parameters.AddWithValue("A_DATAFINAL", this.DataHoraFinal.Date);
                        cmdInsert.Parameters.AddWithValue("A_HORAFINAL", this.DataHoraFinal.TimeOfDay);
                        cmdInsert.Parameters.AddWithValue("A_BGCOLOR", CorAgenda.Fundo);
                        cmdInsert.Parameters.AddWithValue("A_TEXTCOLOR", CorAgenda.Fonte);
                        cmdInsert.Parameters.AddWithValue("A_TRATAMENTO", this.Tratamento.Codigo);
                        cmdInsert.Parameters.AddWithValue("A_STATUS", "1");
                        cmdInsert.Parameters.AddWithValue("A_CLINICA", CodigoClinica);
                        cmdInsert.Parameters.AddWithValue("A_PROFISSIONAL", this.Profissional.Codigo);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
        }

        public void Atualizar(int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  UPDATE
                                        AGENDA
                                        SET
                                        A_PACIENTE =@A_PACIENTE,
                                        A_DATAINICIAL =@A_DATAINICIAL,
                                        A_HORAINICIAL =@A_HORAINICIAL,
                                        A_DATAFINAL =@A_DATAFINAL,
                                        A_HORAFINAL =@A_HORAFINAL,
                                        A_TRATAMENTO =@A_TRATAMENTO
                                        WHERE
                                        A_CODIGO =@A_CODIGO AND
                                        A_CLINICA =@A_CLINICA";
                    
                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdUpdate.Parameters.AddWithValue("A_CODIGO", this.GerarCodigo());
                        cmdUpdate.Parameters.AddWithValue("A_PACIENTE", this.Paciente.Codigo);
                        cmdUpdate.Parameters.AddWithValue("A_DATAINICIAL", this.DataHoraInicial.Date);
                        cmdUpdate.Parameters.AddWithValue("A_HORAINICIAL", this.DataHoraInicial.TimeOfDay);
                        cmdUpdate.Parameters.AddWithValue("A_DATAFINAL", this.DataHoraFinal.Date);
                        cmdUpdate.Parameters.AddWithValue("A_HORAFINAL", this.DataHoraFinal.TimeOfDay);
                        cmdUpdate.Parameters.AddWithValue("A_TRATAMENTO", this.Tratamento.Codigo);
                        cmdUpdate.Parameters.AddWithValue("A_CLINICA", CodigoClinica);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
        }

        public void Atualizar(int Codigo, DateTime DataHoraInicial, DateTime DataHoraFinal, int CodigoTratamento, 
            string StatusAgendamento, int CodigoProfissional, int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  UPDATE
                                        AGENDA
                                        SET
                                        A_DATAINICIAL =@A_DATAINICIAL,
                                        A_HORAINICIAL =@A_HORAINICIAL,
                                        A_DATAFINAL =@A_DATAFINAL,
                                        A_HORAFINAL =@A_HORAFINAL,
                                        A_TRATAMENTO =@A_TRATAMENTO,
                                        A_STATUS =@A_STATUS,
                                        A_PROFISSIONAL =@A_PROFISSIONAL
                                        WHERE
                                        A_CODIGO =@A_CODIGO AND
                                        A_CLINICA =@A_CLINICA";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdUpdate.Parameters.AddWithValue("A_CODIGO", Codigo);
                        cmdUpdate.Parameters.AddWithValue("A_CLINICA", CodigoClinica);
                        cmdUpdate.Parameters.AddWithValue("A_DATAINICIAL", DataHoraInicial.Date);
                        cmdUpdate.Parameters.AddWithValue("A_HORAINICIAL", DataHoraInicial.TimeOfDay);
                        cmdUpdate.Parameters.AddWithValue("A_DATAFINAL", DataHoraFinal.Date);
                        cmdUpdate.Parameters.AddWithValue("A_HORAFINAL", DataHoraFinal.TimeOfDay);
                        cmdUpdate.Parameters.AddWithValue("A_TRATAMENTO", CodigoTratamento);
                        cmdUpdate.Parameters.AddWithValue("A_STATUS", StatusAgendamento);
                        cmdUpdate.Parameters.AddWithValue("A_PROFISSIONAL", CodigoProfissional);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
        }

        public void AtualizarStatus(int Codigo, string NovoStatus, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        AGENDA
                                        SET
                                        A_STATUS =@A_STATUS 
                                        WHERE
                                        A_CODIGO =@A_CODIGO AND
                                        A_CLINICA =@A_CLINICA";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("A_STATUS", NovoStatus);
                        cmdUpdate.Parameters.AddWithValue("A_CODIGO", Codigo);
                        cmdUpdate.Parameters.AddWithValue("A_CLINICA", CodigoClinica);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }

        }

        public void Excluir(int Codigo, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  DELETE
                                        FROM
                                        AGENDA
                                        WHERE
                                        A_CODIGO =@A_CODIGO AND
                                        A_CLINICA =@A_CLINICA";

                    using (FbCommand cmdDelete = new FbCommand(TxtSQL, Con))
                    {
                        cmdDelete.Parameters.AddWithValue("A_CODIGO", Codigo);
                        cmdDelete.Parameters.AddWithValue("A_CLINICA", CodigoClinica);

                        cmdDelete.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        private int GerarCodigo()
        {
            int Codigo = 0;

            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        MAX(A_CODIGO) AS MAX_CODIGO
                                        FROM
                                        AGENDA";

                    Conexao.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read() && !drSelect.IsDBNull(drSelect.GetOrdinal("MAX_CODIGO")))
                            {
                                Codigo = drSelect.GetInt32(drSelect.GetOrdinal("MAX_CODIGO"));
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }

            return Codigo + 1;

        }

        private string ObterStatusVisaoUsuario(string Status)
        {
            if (Status.Equals("1"))
            {
                return "Agendado";
            }
            else if (Status.Equals("2"))
            {
                return "Atendido";
            }
            else if (Status.Equals("3"))
            {
                return "Cancelado";
            }
            else
            {
                return "";
            }
        }

        public List<SelectListItem> ObterSelectItemStatus()
        {
            List<SelectListItem> StatusAgendamento = new List<SelectListItem>();

            StatusAgendamento.Add(new SelectListItem { Text = "Agendado", Value = "1" });

            StatusAgendamento.Add(new SelectListItem { Text = "Atendido", Value = "2" });

            StatusAgendamento.Add(new SelectListItem { Text = "Cancelado", Value = "3" });

            return StatusAgendamento;
        }


    }
}