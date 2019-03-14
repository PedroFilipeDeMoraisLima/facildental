using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using gestaoclinica.Models;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class Evolucao
    {
        [Required]
        public int CodigoProntuario { get; set; }

        public int Codigo { get; set; }

        public Usuario Profissional { get; set; }

        [MaxLength(4000)]
        public string Descricao { get; set; }

        [Required]
        public DateTime DataHoraEvolucao { get; set; }

        [Required]
        public DateTime DataHoraGravacao { get; set; }

        public Evolucao() { }

        public Evolucao(int Codigo)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        E.*,
                                        U.U_CODIGO,
                                        U.U_NOME
                                        FROM
                                        EVOLUCAO E
                                        INNER JOIN USUARIO U
                                        ON (E.E_PROFISSIONAL = U.U_CODIGO)
                                        WHERE
                                        E_CODIGO =@E_CODIGO";

                    Con.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("E_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("E_CODIGO"));
                                this.CodigoProntuario = drSelect.GetInt32(drSelect.GetOrdinal("E_PRONTUARIO"));
                                this.Descricao = drSelect.GetString(drSelect.GetOrdinal("E_DESCRICAO"));
                                this.DataHoraEvolucao = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("E_DATAEVOLUCAO")).Replace("00:00:00",
                                    drSelect.GetString(drSelect.GetOrdinal("E_HORAEVOLUCAO")))));
                                this.DataHoraGravacao = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("E_DATAGRAVACAO")).Replace("00:00:00",
                                    drSelect.GetString(drSelect.GetOrdinal("E_HORAGRAVACAO")))));
                                this.Profissional = new Usuario()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME"))
                                };
                            }
                        }
                    }

                }
                finally
                {
                    Con.Close();
                }
            }
            
        }

        public void Cadastrar(Evolucao e)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    DateTime DataHoraAtual = Firebird.DataHoraServidor();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        EVOLUCAO
                                        (
                                            E_CODIGO,
                                            E_PRONTUARIO,
                                            E_DESCRICAO,
                                            E_DATAEVOLUCAO,
                                            E_HORAEVOLUCAO,
                                            E_DATAGRAVACAO,
                                            E_HORAGRAVACAO,
                                            E_PROFISSIONAL
                                        )
                                        VALUES
                                        (
                                            @E_CODIGO,
                                            @E_PRONTUARIO,
                                            @E_DESCRICAO,
                                            @E_DATAEVOLUCAO,
                                            @E_HORAEVOLUCAO,
                                            @E_DATAGRAVACAO,
                                            @E_HORAGRAVACAO,
                                            @E_PROFISSIONAL
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("E_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("E_PRONTUARIO", e.CodigoProntuario);
                        cmdInsert.Parameters.AddWithValue("E_DESCRICAO", e.Descricao);
                        cmdInsert.Parameters.AddWithValue("E_DATAEVOLUCAO", e.DataHoraEvolucao.Date);
                        cmdInsert.Parameters.AddWithValue("E_HORAEVOLUCAO", e.DataHoraEvolucao.TimeOfDay);
                        cmdInsert.Parameters.AddWithValue("E_DATAGRAVACAO", DataHoraAtual.Date);
                        cmdInsert.Parameters.AddWithValue("E_HORAGRAVACAO", DataHoraAtual.TimeOfDay);
                        cmdInsert.Parameters.AddWithValue("E_PROFISSIONAL", e.Profissional.Codigo);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally 
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(string NovaDescricao, int CodigoEvolucao)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        EVOLUCAO
                                        SET
                                        E_DESCRICAO =@E_DESCRICAO
                                        WHERE
                                        E_CODIGO =@E_CODIGO";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("E_DESCRICAO", NovaDescricao);
                        cmdUpdate.Parameters.AddWithValue("E_CODIGO", CodigoEvolucao);

                        cmdUpdate.ExecuteNonQuery();
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
                                        MAX(E_CODIGO) AS MAX_CODIGO
                                        FROM
                                        EVOLUCAO";

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

        public List<Evolucao> ObterEvolucoesPorProntuario(int CodigoProntuario)
        {
            List<Evolucao> Evolucoes = new List<Evolucao>();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        E.*,
                                        U.U_CODIGO,
                                        U.U_NOME
                                        FROM
                                        EVOLUCAO E
                                        INNER JOIN USUARIO U
                                        ON (E.E_PROFISSIONAL = U.U_CODIGO)
                                        WHERE
                                        E_PRONTUARIO =@E_PRONTUARIO
                                        ORDER BY
                                        E_CODIGO DESC";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("E_PRONTUARIO", CodigoProntuario);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Evolucoes.Add(new Evolucao(){
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("E_CODIGO")),
                                    CodigoProntuario = drSelect.GetInt32(drSelect.GetOrdinal("E_PRONTUARIO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("E_DESCRICAO")),
                                    DataHoraEvolucao = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("E_DATAEVOLUCAO")).Replace("00:00:00",
                                        drSelect.GetString(drSelect.GetOrdinal("E_HORAEVOLUCAO"))))),
                                    DataHoraGravacao = DateTime.Parse(string.Concat(drSelect.GetString(drSelect.GetOrdinal("E_DATAGRAVACAO")).Replace("00:00:00",
                                        drSelect.GetString(drSelect.GetOrdinal("E_HORAGRAVACAO"))))),
                                    Profissional = new Usuario()
                                    {
                                        Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                        Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME"))
                                    }
                                });
                            }
                        }
                    }

                }
                finally
                {
                    Con.Close();
                }
            }
            
            return Evolucoes;
        }

    }
}