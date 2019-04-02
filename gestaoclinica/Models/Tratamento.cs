using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class Tratamento
    {
        [Required]
        public int Codigo { get; set; }

        [Required][MaxLength(180)]
        public string Descricao { get; set; }

        [Required]
        public string Status { get; set; }

        public Tratamento() { }

        public Tratamento(int Codigo, int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        T_CODIGO,
                                        T_DESCRICAO,
                                        T_STATUS
                                        FROM
                                        TRATAMENTO
                                        WHERE
                                        T_CODIGO =@T_CODIGO AND
                                        T_CLINICA =@T_CLINICA";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("T_CODIGO", Codigo);
                        cmdSelect.Parameters.AddWithValue("T_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("T_CODIGO"));
                                this.Descricao = drSelect.GetString(drSelect.GetOrdinal("T_DESCRICAO"));
                                this.Status = drSelect.GetString(drSelect.GetOrdinal("T_STATUS"));
                                //this.DescricaoComValor = this.Descricao + " - " + "R$" + string.Format("{0:N}", this.Valor);
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

        public List<Tratamento> ObterTratamentos(int CodigoClinica, string Descricao = "")
        {
            List<Tratamento> Tratamentos = new List<Tratamento>();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        T_CODIGO,
                                        T_DESCRICAO,
                                        T_STATUS
                                        FROM
                                        TRATAMENTO
                                        WHERE
                                        T_CLINICA =@T_CLINICA ";

                    if (Descricao != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "T_DESCRICAO) LIKE @T_DESCRICAO ");
                    }

                    TxtSQL += "ORDER BY T_DESCRICAO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("T_CLINICA", CodigoClinica);

                        if (Descricao != "")
                        {
                            cmdSelect.Parameters.AddWithValue("T_DESCRICAO", string.Concat("%", Descricao.ToUpper(), "%"));
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Tratamentos.Add(new Tratamento() 
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("T_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("T_DESCRICAO")),
                                    Status = ObterStatusVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("T_STATUS")))
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
            
            return Tratamentos;
        }

        public void Cadastrar(int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        TRATAMENTO
                                        (
                                            T_CODIGO,
                                            T_DESCRICAO,
                                            T_STATUS,
                                            T_CLINICA
                                        )
                                        VALUES
                                        (
                                            @T_CODIGO,
                                            @T_DESCRICAO,
                                            @T_STATUS,
                                            @T_CLINICA
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("T_CODIGO", this.GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("T_DESCRICAO", this.Descricao);
                        cmdInsert.Parameters.AddWithValue("T_STATUS", this.Status);
                        cmdInsert.Parameters.AddWithValue("T_CLINICA", CodigoClinica);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(int CodigoClinica)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        TRATAMENTO
                                        SET
                                        T_DESCRICAO =@T_DESCRICAO,
                                        T_STATUS =@T_STATUS
                                        WHERE
                                        T_CODIGO =@T_CODIGO AND
                                        T_CLINICA =@T_CLINICA";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("T_DESCRICAO", this.Descricao);
                        cmdUpdate.Parameters.AddWithValue("T_STATUS", this.Status);
                        cmdUpdate.Parameters.AddWithValue("T_CODIGO", this.Codigo);
                        cmdUpdate.Parameters.AddWithValue("T_CLINICA", CodigoClinica);

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
                                        TRATAMENTO
                                        WHERE
                                        T_CODIGO =@T_CODIGO AND
                                        T_CLINICA =@T_CLINICA";

                    using (FbCommand cmdDelete = new FbCommand(TxtSQL, Con))
                    {
                        cmdDelete.Parameters.AddWithValue("T_CODIGO", Codigo);
                        cmdDelete.Parameters.AddWithValue("T_CLINICA", CodigoClinica);

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
                                        MAX(T_CODIGO) AS MAX_CODIGO
                                        FROM
                                        TRATAMENTO";

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