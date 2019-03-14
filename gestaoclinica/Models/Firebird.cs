using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FirebirdSql.Data.FirebirdClient;
using System.Data;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class Firebird
    {
        public const string CHARSET = "_iso8859_1 '' || ";

        private FbConnection Conexao = new FbConnection();

        public FbCommand ComandoSQL { get; set; }

        public static string StringConexao = WebConfigurationManager.ConnectionStrings["ClinicaWeb"].ConnectionString;

        public Firebird()
        {
            this.Conexao = new FbConnection(StringConexao);
            this.ComandoSQL = new FbCommand();
            this.ComandoSQL.Connection = this.Conexao;
        }

        public Firebird(FbCommand ComandoSQL)
        {
            this.Conexao = new FbConnection(StringConexao);
            this.ComandoSQL = ComandoSQL;
            this.ComandoSQL.Connection = this.Conexao;
        }

        public bool AbrirConexao()
        {
            try
            {
                Conexao.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool FecharConexao()
        {
            try
            {
                if (Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public void ReiniciarConexao()
        {
            FecharConexao();
            AbrirConexao();
        }

        public int ExecutarComando()
        {
            int Retorno;

            AbrirConexao();

            Retorno = this.ComandoSQL.ExecuteNonQuery();

            this.ComandoSQL.Dispose();

            FecharConexao();

            return Retorno;
        }

        public DataTable ObterTabelaDeDados()
        {
            AbrirConexao();

            DataTable DataTable = new DataTable();

            DataTable.Load(this.ComandoSQL.ExecuteReader());
            
            this.ComandoSQL.Dispose();
            FecharConexao();

            return DataTable;
        }

        public static DateTime DataHoraServidor()
        {
            DateTime DataHoraServidor = DateTime.MinValue;

            using (FbConnection Con = new FbConnection(StringConexao))
            {
                try
                {
                    Con.Open();

                    string SQL = "SELECT CURRENT_DATE || ' ' || CURRENT_TIME AS DATAHORA FROM RDB$DATABASE";

                    using (FbCommand cmdDataHora = new FbCommand(SQL, Con))
                    {
                        using (FbDataReader drDataHora = cmdDataHora.ExecuteReader())
                        {
                            if (drDataHora.Read())
                            {
                                DataHoraServidor = drDataHora.GetDateTime(drDataHora.GetOrdinal("DATAHORA"));
                            }
                        }
                    }
                }
                finally
                {
                    Con.Close();
                }
            }


            return DataHoraServidor;
        }
    }
}