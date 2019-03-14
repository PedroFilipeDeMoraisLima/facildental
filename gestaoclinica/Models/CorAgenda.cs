using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class CorAgenda
    {
        public string Fundo { get; set; }
        public string Fonte { get; set; }

        public CorAgenda() { }

        public CorAgenda ObterCorAgendaRandomica()
        {
            CorAgenda CorAgenda = new CorAgenda();

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  SELECT
                                        FIRST 1
                                        *
                                        FROM
                                        CORAGENDA
                                        ORDER BY
                                        RAND()";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                CorAgenda.Fundo = drSelect.GetString(drSelect.GetOrdinal("C_FUNDO"));
                                CorAgenda.Fonte = drSelect.GetString(drSelect.GetOrdinal("C_FONTE"));
                            }
                        }
                    }
                                        
                }
                finally
                {
                    Conexao.Close();
                }
            }
            
            return CorAgenda;
        }
    }
}