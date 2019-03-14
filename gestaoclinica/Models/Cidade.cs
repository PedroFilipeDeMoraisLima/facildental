using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace gestaoclinica.Models
{
    public class Cidade
    {
        [Required][Key]
        public int Codigo { get; set; }

        [Required]
        public string Descricao { get; set; }

        [Required]
        public int CodigoUF { get; set; }

        public Cidade() { }

        public Cidade(int Codigo) 
        {
            using (FbConnection Con = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        C_CODIGO,
                                        C_DESCRICAO,
                                        C_UF
                                        FROM
                                        CIDADE
                                        WHERE
                                        C_CODIGO =@C_CODIGO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("C_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("C_CODIGO"));
                                this.Descricao = drSelect.GetString(drSelect.GetOrdinal("C_DESCRICAO"));
                                this.CodigoUF = drSelect.GetInt32(drSelect.GetOrdinal("C_UF"));
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

        public List<Cidade> ObterCidadesPorUF(int CodigoUF)
        {
            List<Cidade> Cidades = new List<Cidade>();

            using (FbConnection Con = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  SELECT
                                        C_CODIGO,
                                        C_DESCRICAO
                                        FROM
                                        CIDADE
                                        WHERE
                                        C_UF =@C_UF";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("C_UF", CodigoUF);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Cidades.Add(new Cidade()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("C_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("C_DESCRICAO"))
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
            
            return Cidades;
        }

    }
}