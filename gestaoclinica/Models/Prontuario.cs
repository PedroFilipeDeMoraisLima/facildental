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
    public class Prontuario
    {
        [Required]
        public Paciente Paciente { get; set; }

        [MaxLength(25)]
        public int CodigoEmpresaProntuario { get; set; }

        [MaxLength(4000)]
        public string Anotacoes { get; set; }

        public List<Evolucao> Evolucoes { get; set; }

        public Prontuario() { }

        public Prontuario(int Codigo)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        P1.*,
                                        P2.P_CODIGO AS CODIGO_PACIENTE,
                                        P2.*,
                                        E.*
                                        FROM
                                        PRONTUARIO P1
                                        INNER JOIN PACIENTE P2
                                        ON (P1.P_CODIGO = P2.P_CODIGO)
                                        LEFT JOIN EVOLUCAO E
                                        ON (P1.P_CODIGO = E_CODIGO)
                                        WHERE
                                        P1.P_CODIGO =@P_CODIGO";

                    Con.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("P_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Paciente = new Paciente()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("CODIGO_PACIENTE")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("P_NOME")),
                                    Sexo = drSelect.GetString(drSelect.GetOrdinal("P_SEXO")),
                                    DataNascimento = drSelect.GetDateTime(drSelect.GetOrdinal("P_DATANASCIMENTO")),
                                    CPF = drSelect.GetString(drSelect.GetOrdinal("P_CPF")),
                                    Endereco = drSelect.GetString(drSelect.GetOrdinal("P_ENDERECO")),
                                    NumeroEndereco = drSelect.GetString(drSelect.GetOrdinal("P_NUMEROENDERECO")),
                                    Bairro = drSelect.GetString(drSelect.GetOrdinal("P_BAIRRO")),
                                    ComplementoEndereco = drSelect.GetString(drSelect.GetOrdinal("P_COMPENDERECO")),
                                    RG = drSelect.GetString(drSelect.GetOrdinal("P_RG")),
                                    TelefoneResidencial = drSelect.GetString(drSelect.GetOrdinal("P_TELRESIDENCIAL")),
                                    TelefoneComercial = drSelect.GetString(drSelect.GetOrdinal("P_TELCOMERCIAL")),
                                    Celular = drSelect.GetString(drSelect.GetOrdinal("P_CELULAR")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("P_EMAIL")),
                                    CodigoConvenio = !drSelect.IsDBNull(drSelect.GetOrdinal("P_CONVENIO")) ? drSelect.GetInt32(drSelect.GetOrdinal("P_CONVENIO")) : 0,
                                    CodigoCidade = drSelect.GetInt32(drSelect.GetOrdinal("P_CIDADE")),
                                };

                                this.CodigoEmpresaProntuario = !drSelect.IsDBNull(drSelect.GetOrdinal("P_CODIGOINTERNO"))
                                    ? drSelect.GetInt32(drSelect.GetOrdinal("P_CODIGOINTERNO")) : 0;

                                this.Anotacoes = drSelect.GetString(drSelect.GetOrdinal("P_ANOTACOES"));
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

        public void Cadastrar(int CodigoPaciente)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  INSERT 
                                        INTO 
                                        PRONTUARIO
                                        (
                                            P_CODIGO
                                        )
                                        VALUES
                                        (
                                            @P_CODIGO
                                        )";

                    Con.Open();

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("P_CODIGO", CodigoPaciente);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }

        }

        public void AtualizarAnotacoes(Prontuario p)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @" UPDATE PRONTUARIO SET P_ANOTACOES =@P_ANOTACOES WHERE P_CODIGO =@P_CODIGO";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {

                    }

                    Con.Open();

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("P_ANOTACOES", p.Anotacoes);
                        cmdUpdate.Parameters.AddWithValue("P_CODIGO", p.Paciente.Codigo);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void AtualizarCodigoInterno(Prontuario p)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @" UPDATE PRONTUARIO SET P_CODIGOINTERNO =@P_CODIGOINTERNO WHERE P_CODIGO =@P_CODIGO";

                    Con.Open();

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {

                        cmdUpdate.Parameters.AddWithValue("P_CODIGOINTERNO", p.CodigoEmpresaProntuario);
                        cmdUpdate.Parameters.AddWithValue("P_CODIGO", p.Paciente.Codigo);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void CarregarEvolucoes()
        {
            Evolucao Evolucao = new Evolucao();

            this.Evolucoes = new List<Evolucao>();

            this.Evolucoes = Evolucao.ObterEvolucoesPorProntuario(this.Paciente.Codigo);
        }

    }
}