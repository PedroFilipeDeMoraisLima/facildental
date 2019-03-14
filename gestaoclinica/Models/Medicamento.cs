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
    public class Medicamento
    {
        public int Codigo { get; set; }

        [Required]
        [MaxLength(120)]
        [Display(Name="Nome Comercial")]
        public string NomeComercial { get; set; }

        [Required]
        [MaxLength(360)]
        [Display(Name = "Princípio Ativo")]
        public string PrincipioAtivo { get; set; }

        public string Status { get; set; }

        public Medicamento() { }

        public Medicamento(int Codigo)
        {
            Carregar(Codigo);
        }

        public void Carregar(int Codigo)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {

                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        M_CODIGO,
                                        M_NOMECOMERCIAL,
                                        M_PRINCIPIOATIVO,
                                        M_STATUS
                                        FROM
                                        MEDICAMENTO
                                        WHERE
                                        M_CODIGO =@M_CODIGO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("M_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("M_CODIGO"));
                                this.NomeComercial = drSelect.GetString(drSelect.GetOrdinal("M_NOMECOMERCIAL"));
                                this.PrincipioAtivo = drSelect.GetString(drSelect.GetOrdinal("M_PRINCIPIOATIVO"));
                                this.Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("M_STATUS")));
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

        public List<Medicamento> ObterMedicamentos(string NomeComercial = "")
        {
            List<Medicamento> Medicamentos = new List<Medicamento>();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {

                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        M_CODIGO,
                                        M_NOMECOMERCIAL,
                                        M_PRINCIPIOATIVO,
                                        M_STATUS
                                        FROM
                                        MEDICAMENTO
                                        WHERE
                                        1=1 ";

                    if (NomeComercial != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "M_NOMECOMERCIAL) LIKE @M_NOMECOMERCIAL ");
                    }

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        if (NomeComercial != "")
                        {
                            cmdSelect.Parameters.Add("M_NOMECOMERCIAL", string.Concat("%", NomeComercial.ToUpper(), "%"));
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Medicamentos.Add(new Medicamento()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("M_CODIGO")),
                                    NomeComercial = drSelect.GetString(drSelect.GetOrdinal("M_NOMECOMERCIAL")),
                                    PrincipioAtivo = drSelect.GetString(drSelect.GetOrdinal("M_PRINCIPIOATIVO")),
                                    Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("M_STATUS")))
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

            return Medicamentos;
        }

        public void Cadastrar(Medicamento M)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        MEDICAMENTO
                                        (
                                            M_CODIGO,
                                            M_NOMECOMERCIAL,
                                            M_PRINCIPIOATIVO,
                                            M_STATUS
                                        )
                                        VALUES
                                        (
                                            @M_CODIGO,
                                            @M_NOMECOMERCIAL,
                                            @M_PRINCIPIOATIVO,
                                            @M_STATUS
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("M_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("M_NOMECOMERCIAL", M.NomeComercial);
                        cmdInsert.Parameters.AddWithValue("M_PRINCIPIOATIVO", M.PrincipioAtivo);
                        cmdInsert.Parameters.AddWithValue("M_STATUS", "A");

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(Medicamento M)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        MEDICAMENTO
                                        SET
                                        M_NOMECOMERCIAL =@M_NOMECOMERCIAL,
                                        M_PRINCIPIOATIVO =@M_PRINCIPIOATIVO,
                                        M_STATUS =@M_STATUS
                                        WHERE
                                        M_CODIGO =@M_CODIGO";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("M_CODIGO", M.Codigo);
                        cmdUpdate.Parameters.AddWithValue("M_NOMECOMERCIAL", M.NomeComercial);
                        cmdUpdate.Parameters.AddWithValue("M_PRINCIPIOATIVO", M.PrincipioAtivo);
                        cmdUpdate.Parameters.AddWithValue("M_STATUS", M.Status);

                        cmdUpdate.ExecuteNonQuery();
                    }
                                                
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Excluir(int Codigo)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  DELETE
                                        FROM
                                        MEDICAMENTO
                                        WHERE
                                        M_CODIGO =@M_CODIGO";

                    using (FbCommand cmdDelete = new FbCommand(TxtSQL, Con))
                    {
                        cmdDelete.Parameters.AddWithValue("M_CODIGO", Codigo);

                        cmdDelete.ExecuteNonQuery();
                    }

                }
                finally
                {
                    Con.Close();
                }
            }
        }

        private string ObterStatusVisaoUsuario(string Status)
        {
            if (Status.Equals("A"))
            {
                return "Ativo";
            }
            else
            {
                return "Inativo";
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
                                        MAX(M_CODIGO) AS MAX_CODIGO
                                        FROM
                                        MEDICAMENTO";

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

        
    }
}