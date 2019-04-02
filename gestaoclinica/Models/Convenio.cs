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
    public class Convenio
    {
        [Display(Name = "Código")]
        public int Codigo { get; set; }

        [Required]
        [MaxLength(120)]
        [Display(Name="Descrição")]
        public string Descricao { get; set; }

        public string Status { get; set; }

        public List<Tratamento> TratamentosTabela { get; set; }

        public Convenio() { }

        public Convenio(int Codigo, int CodigoClinica)
        {
            Carregar(Codigo, CodigoClinica);
        }

        public void Carregar(int Codigo, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        C_CODIGO,
                                        C_DESCRICAO,
                                        C_STATUS
                                        FROM
                                        CONVENIO
                                        WHERE
                                        C_CODIGO =@C_CODIGO AND
                                        C_CLINICA =@C_CLINICA";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("C_CODIGO", Codigo);
                        cmdSelect.Parameters.AddWithValue("C_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("C_CODIGO"));
                                this.Descricao = drSelect.GetString(drSelect.GetOrdinal("C_DESCRICAO"));
                                this.Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("C_STATUS")));
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

        public void Cadastrar(Convenio c, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        CONVENIO
                                        (
                                            C_CODIGO,
                                            C_DESCRICAO,
                                            C_STATUS,
                                            C_CLINICA
                                        )
                                        VALUES
                                        (
                                            @C_CODIGO,
                                            @C_DESCRICAO,
                                            @C_STATUS,
                                            @C_CLINICA
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("C_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("C_CLINICA", CodigoClinica);
                        cmdInsert.Parameters.AddWithValue("C_DESCRICAO", c.Descricao);
                        cmdInsert.Parameters.AddWithValue("C_STATUS", "A");

                        cmdInsert.ExecuteNonQuery();
                    }

                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(Convenio c, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE  
                                        CONVENIO
                                        SET
                                        C_DESCRICAO =@C_DESCRICAO,
                                        C_STATUS =@C_STATUS
                                        WHERE
                                        C_CODIGO =@C_CODIGO AND
                                        C_CLINICA =@C_CLINICA";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("C_DESCRICAO", c.Descricao);
                        cmdUpdate.Parameters.AddWithValue("C_STATUS", c.Status);
                        cmdUpdate.Parameters.AddWithValue("C_CODIGO", c.Codigo);
                        cmdUpdate.Parameters.AddWithValue("C_CLINICA", CodigoClinica);

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
                                        CONVENIO
                                        WHERE
                                        C_CODIGO =@C_CODIGO AND
                                        C_CLINICA =@C_CLINICA";

                    using (FbCommand cmdDelete = new FbCommand(TxtSQL, Con))
                    {
                        cmdDelete.Parameters.AddWithValue("C_CODIGO", Codigo);
                        cmdDelete.Parameters.AddWithValue("C_CLINICA", CodigoClinica);

                        cmdDelete.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
            

        }

        public List<Convenio> ObterConvenios(int CodigoClinica, string Descricao = "", string Status = "")
        {
            List<Convenio> Convenios = new List<Convenio>();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        C_CODIGO,
                                        C_DESCRICAO,
                                        C_STATUS
                                        FROM
                                        CONVENIO
                                        WHERE
                                        C_CLINICA =@C_CLINICA ";

                    if (Status != "")
                    {
                        TxtSQL += "C_STATUS =@C_STATUS ";
                    }

                    if (Descricao != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "C_DESCRICAO) LIKE @C_DESCRICAO ");
                    }

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("C_CLINICA", CodigoClinica);

                        if (Status != "")
                        {
                            cmdSelect.Parameters.AddWithValue("C_STATUS", Status);
                        }

                        if (Descricao != "")
                        {
                            cmdSelect.Parameters.Add("C_DESCRICAO", string.Concat("%", Descricao.ToUpper(), "%"));
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Convenios.Add(new Convenio()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("C_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("C_DESCRICAO")),
                                    Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("C_STATUS")))
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


            return Convenios;
        }

        private int GerarCodigo()
        {
            int Codigo = 0;

            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        MAX(C_CODIGO) AS MAX_CODIGO
                                        FROM
                                        CONVENIO";

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
            if (Status.Equals("A"))
            {
                return "Ativo";
            }
            else
            {
                return "Inativo";
            }
        }

    }
}