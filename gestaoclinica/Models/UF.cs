using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace gestaoclinica.Models
{
    public class UF
    {
        [Required][Key]
        public int Codigo { get; set; }

        [Required]
        public string Sigla { get; set; }

        public string Descricao { get; set; }

        public UF() { }

        public UF(int Codigo)
        {
            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        U_CODIGO,
                                        U_SIGLA,
                                        U_DESCRICAO
                                        FROM
                                        UF
                                        WHERE
                                        U_CODIGO =@U_CODIGO";

                    Conexao.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("U_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO"));
                                Descricao = drSelect.GetString(drSelect.GetOrdinal("U_DESCRICAO"));
                                Sigla = drSelect.GetString(drSelect.GetOrdinal("U_SIGLA"));
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
            
        }

        public List<UF> ObterRegistros()
        {
            List<UF> UF = new List<UF>();

            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        U_CODIGO,
                                        U_SIGLA,
                                        U_DESCRICAO
                                        FROM
                                        UF";

                    Conexao.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                UF.Add(new UF()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Descricao = drSelect.GetString(drSelect.GetOrdinal("U_DESCRICAO")),
                                    Sigla = drSelect.GetString(drSelect.GetOrdinal("U_SIGLA"))
                                });
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
            
            

            
            
            return UF;
        }

        public UF ObterUFPorCidade(int CodigoCidade)
        {
            UF UF = new UF();

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  SELECT
                                        U.U_CODIGO,
                                        U.U_SIGLA,
                                        U.U_DESCRICAO
                                        FROM
                                        CIDADE C
                                        INNER JOIN UF U
                                        ON (C.C_UF = U.U_CODIGO)
                                        WHERE
                                        C.C_CODIGO =@C_CODIGO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("C_CODIGO", CodigoCidade);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                UF.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO"));
                                UF.Sigla = drSelect.GetString(drSelect.GetOrdinal("U_SIGLA"));
                                UF.Descricao = drSelect.GetString(drSelect.GetOrdinal("U_DESCRICAO"));
                            }
                        }
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }

            return UF;
        }

    }
}