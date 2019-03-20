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
    public class Clinica
    {
        public int Codigo { get; set; }

        [Required]
        [MaxLength(180)]
        public string Nome { get; set; }

        public string Status { get; set; }

        public Clinica() { }

        public Clinica(int Codigo)
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
                                        CC_CODIGO,
                                        CC_NOME,
                                        CC_STATUS
                                        FROM
                                        CLINICACONTRATO
                                        WHERE
                                        CC_CODIGO =@CC_CODIGO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("CC_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("CC_CODIGO"));
                                this.Nome = drSelect.GetString(drSelect.GetOrdinal("CC_NOME"));
                                this.Status = drSelect.GetString(drSelect.GetOrdinal("CC_STATUS"));
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

        public void Cadastrar(Clinica c)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        CLINICACONTRATO
                                        (
                                            CC_CODIGO,
                                            CC_NOME,
                                            CC_STATUS
                                        )
                                        VALUES
                                        (
                                            @CC_CODIGO,
                                            @CC_NOME,
                                            @CC_STATUS
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("CC_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("CC_NOME", c.Nome);
                        cmdInsert.Parameters.AddWithValue("CC_STATUS", c.Status);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(Clinica c)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        CLINICACONTRATO
                                        SET
                                        CC_NOME =@CC_NOME,
                                        CC_STATUS
                                        WHERE
                                        CC_CODIGO =@CC_CODIGO";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("CC_CODIGO", c.Codigo);
                        cmdUpdate.Parameters.AddWithValue("CC_NOME", c.Nome);
                        cmdUpdate.Parameters.AddWithValue("CC_STATUS", c.Status);

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
                                        MAX(CC_CODIGO) AS MAX_CODIGO
                                        FROM
                                        CLINICACONTRATO";

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