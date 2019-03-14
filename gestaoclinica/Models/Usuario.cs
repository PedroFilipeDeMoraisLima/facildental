using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class Usuario
    {
        public int Codigo { get; set; }

        [Required] [MaxLength(45)]
        public string Email { get; set; }

        [Required] [MaxLength(180)]
        public string Nome { get; set; }

        [MaxLength(360)]
        public string Senha { get; set; }

        public string Status { get; set; }
        
        public Usuario() { }

        public Usuario(int Codigo)
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
                                        U_CODIGO,
                                        U_EMAIL,
                                        U_SENHA,
                                        U_NOME,
                                        U_STATUS
                                        FROM
                                        USUARIO
                                        WHERE
                                        U_CODIGO =@U_CODIGO";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("U_CODIGO", Codigo);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO"));
                                this.Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL"));
                                this.Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME"));
                                this.Status = drSelect.GetString(drSelect.GetOrdinal("U_STATUS"));
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

        public void Cadastrar(Usuario u)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  INSERT
                                        INTO
                                        USUARIO
                                        (
                                            U_CODIGO,
                                            U_NOME,
                                            U_EMAIL,
                                            U_SENHA,
                                            U_STATUS
                                        )
                                        VALUES
                                        (
                                            @U_CODIGO,
                                            @U_NOME,
                                            @U_EMAIL,
                                            @U_SENHA,
                                            @U_STATUS
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("U_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("U_NOME", u.Nome);
                        cmdInsert.Parameters.AddWithValue("U_EMAIL", u.Email);
                        cmdInsert.Parameters.AddWithValue("U_STATUS", "A");
                        cmdInsert.Parameters.AddWithValue("U_SENHA", Criptografia.Codifica(u.Senha));

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(Usuario u)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        USUARIO
                                        SET
                                        U_NOME =@U_NOME,
                                        U_STATUS =@U_STATUS
                                        WHERE
                                        U_CODIGO =@U_CODIGO";
                                        

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("U_CODIGO", u.Codigo);
                        cmdUpdate.Parameters.AddWithValue("U_NOME", u.Nome);
                        cmdUpdate.Parameters.AddWithValue("U_STATUS", u.Status);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public Usuario Autenticar(string Email, string Senha)
        {
            Usuario UsuarioAutenticado = new Usuario();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        U_CODIGO,
                                        U_SENHA
                                        FROM
                                        USUARIO
                                        WHERE
                                        U_EMAIL =@U_EMAIL AND
                                        U_STATUS = 'A'";
                    Con.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("U_EMAIL", Email);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read() && !drSelect.IsDBNull(drSelect.GetOrdinal("U_CODIGO")))
                            {
                                if (Criptografia.Compara(Senha, drSelect.GetString(drSelect.GetOrdinal("U_SENHA"))))
                                {
                                    UsuarioAutenticado.Carregar(drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")));
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Con.Close();
                }
            }

            return UsuarioAutenticado;
        }

        public void AlterarSenha(int Codigo, string NovaSenha)
        {
            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Con.Open();

                    string TxtSQL = @"  UPDATE
                                        USUARIO
                                        SET
                                        U_SENHA =@U_SENHA
                                        WHERE
                                        U_CODIGO =@U_CODIGO";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("U_SENHA", Criptografia.Codifica(NovaSenha));
                        cmdUpdate.Parameters.AddWithValue("U_CODIGO", Codigo);

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

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        MAX(U_CODIGO) AS MAX_CODIGO
                                        FROM
                                        USUARIO";

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

        public List<Usuario> ObterUsuarios(string Nome = "")
        {
            List<Usuario> Usuarios = new List<Usuario>();

            using (FbConnection Con = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        U_CODIGO,
                                        U_EMAIL,
                                        U_NOME,
                                        U_STATUS
                                        FROM
                                        USUARIO
                                        WHERE
                                        1=1 ";

                    Con.Open();

                    if (Nome != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "U_NOME) LIKE @U_NOME ");
                    }

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {

                        if (Nome != "")
                        {
                            cmdSelect.Parameters.Add("U_NOME", string.Concat("%", Nome.ToUpper(), "%"));
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {

                            while (drSelect.Read())
                            {
                                Usuarios.Add(new Usuario()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME")),
                                    Status = drSelect.GetString(drSelect.GetOrdinal("U_STATUS")).Equals("A") ? "Ativo" : "Inativo"
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

            return Usuarios;
        }

        public string PrimeiroNome()
        {
            return this.Nome.Split(' ')[0];
        }

    }
}