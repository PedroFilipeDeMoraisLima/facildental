using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;

namespace gestaoclinica.Models
{
    public class Usuario
    {
        public int Codigo { get; set; }

        public Clinica Clinica { get; set; }

        [Required] [MaxLength(45)]
        public string Email { get; set; }

        [Required] [MaxLength(180)]
        public string Nome { get; set; }

        [MaxLength(360)]
        public string Senha { get; set; }

        public string Status { get; set; }

        [Required]
        public string Celular { get; set; }

        public enum ePerfil { NULL, Administrador_Clinica, Profissional_Saude, Administrativo }

        [Required]
        public ePerfil Perfil { get; set; }
        
        public Usuario() 
        {
            
        }

        public Usuario(int Codigo, int CodigoClinica)
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
                                        U_CODIGO,
                                        U_EMAIL,
                                        U_SENHA,
                                        U_NOME,
                                        U_STATUS,
                                        U_PERFIL,
                                        U_CELULAR
                                        FROM
                                        USUARIO
                                        WHERE
                                        U_CODIGO =@U_CODIGO AND
                                        U_CLINICA =@U_CLINICA";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {
                        cmdSelect.Parameters.AddWithValue("U_CODIGO", Codigo);
                        cmdSelect.Parameters.AddWithValue("U_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO"));
                                this.Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL"));
                                this.Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME"));
                                this.Status = drSelect.GetString(drSelect.GetOrdinal("U_STATUS"));
                                this.Perfil = ObterPerfilVisaoUsuario(drSelect.GetInt32(drSelect.GetOrdinal("U_PERFIL")));
                                this.Celular = drSelect.GetString(drSelect.GetOrdinal("U_CELULAR"));
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

        public void Cadastrar(Usuario u, int CodigoClinica)
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
                                            U_STATUS,
                                            U_CLINICA,
                                            U_PERFIL,
                                            U_CELULAR
                                        )
                                        VALUES
                                        (
                                            @U_CODIGO,
                                            @U_NOME,
                                            @U_EMAIL,
                                            @U_SENHA,
                                            @U_STATUS,
                                            @U_CLINICA,
                                            @U_PERFIL,
                                            @U_CELULAR
                                        )";

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Con))
                    {
                        cmdInsert.Parameters.AddWithValue("U_CODIGO", GerarCodigo());
                        cmdInsert.Parameters.AddWithValue("U_NOME", u.Nome);
                        cmdInsert.Parameters.AddWithValue("U_EMAIL", u.Email);
                        cmdInsert.Parameters.AddWithValue("U_STATUS", "A");
                        cmdInsert.Parameters.AddWithValue("U_SENHA", Criptografia.Codifica(u.Senha));
                        cmdInsert.Parameters.AddWithValue("U_CLINICA", CodigoClinica);
                        cmdInsert.Parameters.AddWithValue("U_PERFIL", ObterPerfilVisaoBanco(u.Perfil));
                        cmdInsert.Parameters.AddWithValue("U_CELULAR", u.Celular);

                        cmdInsert.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Con.Close();
                }
            }
        }

        public void Atualizar(Usuario u, int CodigoClinica)
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
                                        U_STATUS =@U_STATUS,
                                        U_PERFIL =@U_PERFIL,
                                        U_CELULAR =@U_CELULAR
                                        WHERE
                                        U_CODIGO =@U_CODIGO AND
                                        U_CLINICA =@U_CLINICA";
                                        

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("U_CODIGO", u.Codigo);
                        cmdUpdate.Parameters.AddWithValue("U_NOME", u.Nome);
                        cmdUpdate.Parameters.AddWithValue("U_STATUS", u.Status);
                        cmdUpdate.Parameters.AddWithValue("U_CLINICA", CodigoClinica);
                        cmdUpdate.Parameters.AddWithValue("U_PERFIL", ObterPerfilVisaoBanco(u.Perfil));
                        cmdUpdate.Parameters.AddWithValue("U_CELULAR", u.Celular);

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
                                        U_SENHA,
                                        U_CLINICA,
                                        CC_NOME
                                        FROM
                                        USUARIO
                                        INNER JOIN
                                        CLINICACONTRATO
                                        ON (U_CLINICA = CC_CODIGO)
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
                                    UsuarioAutenticado.Carregar(drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")), drSelect.GetInt32(drSelect.GetOrdinal("U_CLINICA")));

                                    UsuarioAutenticado.Clinica = new Models.Clinica()
                                    {
                                        Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CLINICA")),
                                        Nome = drSelect.GetString(drSelect.GetOrdinal("CC_NOME"))
                                    };
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

        public void AlterarSenha(int Codigo, string NovaSenha, int CodigoClinica)
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
                                        U_CODIGO =@U_CODIGO AND
                                        U_CLINICA =@U_CLINICA";

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Con))
                    {
                        cmdUpdate.Parameters.AddWithValue("U_SENHA", Criptografia.Codifica(NovaSenha));
                        cmdUpdate.Parameters.AddWithValue("U_CODIGO", Codigo);
                        cmdUpdate.Parameters.AddWithValue("U_CLINICA", CodigoClinica);

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

        public List<Usuario> ObterUsuarios(int CodigoClinica, string Nome = "", string Status = "A")
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
                                        U_STATUS,
                                        U_PERFIL,
                                        U_CELULAR
                                        FROM
                                        USUARIO
                                        WHERE
                                        U_CLINICA =@U_CLINICA ";

                    Con.Open();

                    if (Nome != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "U_NOME) LIKE @U_NOME ");
                    }

                    if (Status != "")
                    {
                        TxtSQL += "AND U_STATUS =@U_STATUS ";
                    }

                    TxtSQL += "ORDER BY U_NOME";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Con))
                    {

                        if (Nome != "")
                        {
                            cmdSelect.Parameters.Add("U_NOME", string.Concat("%", Nome.ToUpper(), "%"));
                        }

                        if (Status != "")
                        {
                            cmdSelect.Parameters.Add("U_STATUS", Status);
                        }

                        cmdSelect.Parameters.AddWithValue("U_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {

                            while (drSelect.Read())
                            {
                                Usuarios.Add(new Usuario()
                                {
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("U_CODIGO")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("U_EMAIL")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("U_NOME")),
                                    Status = drSelect.GetString(drSelect.GetOrdinal("U_STATUS")).Equals("A") ? "Ativo" : "Inativo",
                                    Perfil = ObterPerfilVisaoUsuario(drSelect.GetInt32(drSelect.GetOrdinal("U_PERFIL"))),
                                    Celular = drSelect.GetString(drSelect.GetOrdinal("U_CELULAR"))
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

        private ePerfil ObterPerfilVisaoUsuario(int Codigo)
        {
            switch (Codigo)
            {
                case 1:
                    {
                        return ePerfil.Administrador_Clinica;
                    }

                case 2:
                    {
                        return ePerfil.Profissional_Saude;
                    }

                case 3:
                    {
                        return ePerfil.Administrativo;
                    }

                default:
                    {
                        return ePerfil.NULL;
                    }
            }
        }

        private int ObterPerfilVisaoBanco(ePerfil Perfil)
        {
            switch (Perfil)
            {
                case ePerfil.Administrador_Clinica:
                    {
                        return 1;
                    }

                case ePerfil.Profissional_Saude:
                    {
                        return 2;
                    }

                case ePerfil.Administrativo:
                    {
                        return 3;
                    }

                default:
                    {
                        return 0;
                    }
            }
        }

        public List<SelectListItem> ObterSelectItemProfissionaisAgenda(int CodigoClinica, string NomeClinica)
        {
            List<SelectListItem> Profissioanais = new List<SelectListItem>();

            Profissioanais.Add(new SelectListItem() { Text = "Clínica: " + NomeClinica, Value = "" });

            Usuario u = new Usuario();

            foreach (Usuario ObjUsuario in u.ObterUsuarios(CodigoClinica))
            {
                Profissioanais.Add(new SelectListItem() { Text = ObjUsuario.Nome, Value = ObjUsuario.Codigo.ToString() });
            }

            return Profissioanais;
        }

    }
}