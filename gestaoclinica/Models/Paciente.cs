using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.Web.Configuration;
using gestaoclinica.Models;

namespace gestaoclinica.Models
{
    public class Paciente
    {
        [Required] [Key]
        public int Codigo { get; set; }

        [Required][MaxLength(180, ErrorMessage="O campo nome aceita até 180 caracteres")]
        public string Nome { get; set; }

        [Required]
        public string Sexo { get; set; }

        public DateTime DataNascimento { get; set; }

        [MaxLength(180, ErrorMessage="O campo email aceita até 180 caracteres")]
        public string Email { get; set; }

        [MaxLength(45)]
        public string TelefoneResidencial { get; set; }

        [Required] [MaxLength(45)]
        public string Celular { get; set; }

        [MaxLength(45)]
        public string TelefoneComercial { get; set; }

        public int Prontuario { get; set; }

        [MaxLength(45)]
        public string CPF { get; set; }

        [MaxLength(180, ErrorMessage = "O campo endereço aceita até 180 caracteres")]
        public string Endereco { get; set; }

        [MaxLength(10)]
        public string NumeroEndereco { get; set; }

        [MaxLength(60, ErrorMessage = "O campo email aceita até 60 caracteres")]
        public string Bairro { get; set; }

        [MaxLength(45, ErrorMessage = "O campo complemento do endereço aceita até 45 caracteres")]
        public string ComplementoEndereco { get; set; }

        [MaxLength(20)]
        public string RG { get; set; }

        [MaxLength(120, ErrorMessage = "O campo carteira do convênio aceita até 120 caracteres")]
        public string CarteiraConvenio { get; set; }

        public List<Agenda> Agendamentos { get; set; }
        
        public int CodigoCidade { get; set; }

        public int CodigoConvenio { get; set; }

        public Paciente() { }

        public Paciente(int Codigo, int CodigoClinica) 
        {
            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"  SELECT
                                        *
                                        FROM
                                        PACIENTE
                                        WHERE
                                        P_CODIGO =@P_CODIGO AND
                                        P_CLINICA =@P_CLINICA";

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("P_CODIGO", Codigo);
                        cmdSelect.Parameters.AddWithValue("P_CLINICA", CodigoClinica);

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            if (drSelect.Read())
                            {
                                this.Codigo = drSelect.GetInt32(drSelect.GetOrdinal("P_CODIGO"));
                                this.Nome = drSelect.GetString(drSelect.GetOrdinal("P_NOME"));
                                this.Sexo = drSelect.GetString(drSelect.GetOrdinal("P_SEXO"));
                                this.DataNascimento = drSelect.GetDateTime(drSelect.GetOrdinal("P_DATANASCIMENTO"));

                                if (!drSelect.IsDBNull(drSelect.GetOrdinal("P_PRONTUARIO")))
                                {
                                    this.Prontuario = drSelect.GetInt32(drSelect.GetOrdinal("P_PRONTUARIO"));
                                }

                                this.CPF = drSelect.GetString(drSelect.GetOrdinal("P_CPF"));
                                this.Endereco = drSelect.GetString(drSelect.GetOrdinal("P_ENDERECO"));
                                this.NumeroEndereco = drSelect.GetString(drSelect.GetOrdinal("P_NUMEROENDERECO"));
                                this.Bairro = drSelect.GetString(drSelect.GetOrdinal("P_BAIRRO"));
                                this.ComplementoEndereco = drSelect.GetString(drSelect.GetOrdinal("P_COMPENDERECO"));
                                this.RG = drSelect.GetString(drSelect.GetOrdinal("P_RG"));
                                this.TelefoneResidencial = drSelect.GetString(drSelect.GetOrdinal("P_TELRESIDENCIAL"));
                                this.TelefoneComercial = drSelect.GetString(drSelect.GetOrdinal("P_TELCOMERCIAL"));
                                this.Celular = drSelect.GetString(drSelect.GetOrdinal("P_CELULAR"));
                                this.Email = drSelect.GetString(drSelect.GetOrdinal("P_EMAIL"));
                                this.CodigoCidade = drSelect.GetInt32(drSelect.GetOrdinal("P_CIDADE"));

                                if (!drSelect.IsDBNull(drSelect.GetOrdinal("P_CONVENIO")))
                                {
                                    this.CodigoConvenio = drSelect.GetInt32(drSelect.GetOrdinal("P_CONVENIO"));
                                }
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

        public void Cadastrar(Paciente p, int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  INSERT
                                        INTO
                                        PACIENTE
                                        (
                                            P_CODIGO,
                                            P_NOME,
                                            P_SEXO,
                                            P_DATANASCIMENTO,
                                            P_PRONTUARIO,
                                            P_CPF,
                                            P_ENDERECO,
                                            P_NUMEROENDERECO,
                                            P_BAIRRO,
                                            P_COMPENDERECO,
                                            P_RG,
                                            P_TELRESIDENCIAL,
                                            P_TELCOMERCIAL,
                                            P_CELULAR,
                                            P_EMAIL,
                                            P_CIDADE,
                                            P_CONVENIO,
                                            P_CLINICA
                                        )
                                        VALUES
                                        (
                                            @P_CODIGO,
                                            @P_NOME,
                                            @P_SEXO,
                                            @P_DATANASCIMENTO,
                                            @P_PRONTUARIO,
                                            @P_CPF,
                                            @P_ENDERECO,
                                            @P_NUMEROENDERECO,
                                            @P_BAIRRO,
                                            @P_COMPENDERECO,
                                            @P_RG,
                                            @P_TELRESIDENCIAL,
                                            @P_TELCOMERCIAL,
                                            @P_CELULAR,
                                            @P_EMAIL,
                                            @P_CIDADE,
                                            @P_CONVENIO,
                                            @P_CLINICA
                                        )";

                    Conexao.Open();

                    using (FbCommand cmdInsert = new FbCommand(TxtSQL, Conexao))
                    {
                        this.Codigo = this.GerarCodigo();

                        cmdInsert.Parameters.AddWithValue("P_CODIGO", this.Codigo);
                        cmdInsert.Parameters.AddWithValue("P_NOME", p.Nome);
                        cmdInsert.Parameters.AddWithValue("P_SEXO", p.Sexo);
                        cmdInsert.Parameters.AddWithValue("P_DATANASCIMENTO", p.DataNascimento);

                        if (p.Prontuario != 0)
                        {
                            cmdInsert.Parameters.AddWithValue("P_PRONTUARIO", p.Prontuario);
                        }
                        else
                        {
                            cmdInsert.Parameters.AddWithValue("P_PRONTUARIO", null);
                        }

                        cmdInsert.Parameters.AddWithValue("P_CPF", p.CPF);
                        cmdInsert.Parameters.AddWithValue("P_ENDERECO", p.Endereco);
                        cmdInsert.Parameters.AddWithValue("P_NUMEROENDERECO", p.NumeroEndereco);
                        cmdInsert.Parameters.AddWithValue("P_BAIRRO", p.Bairro);
                        cmdInsert.Parameters.AddWithValue("P_COMPENDERECO", p.ComplementoEndereco);
                        cmdInsert.Parameters.AddWithValue("P_RG", p.RG);
                        cmdInsert.Parameters.AddWithValue("P_TELRESIDENCIAL", p.TelefoneResidencial);
                        cmdInsert.Parameters.AddWithValue("P_TELCOMERCIAL", p.TelefoneComercial);
                        cmdInsert.Parameters.AddWithValue("P_CELULAR", p.Celular);
                        cmdInsert.Parameters.AddWithValue("P_EMAIL", p.Email);

                        if (p.CodigoCidade != 0)
                        {
                            cmdInsert.Parameters.AddWithValue("P_CIDADE", p.CodigoCidade);
                        }
                        else
                        {
                            cmdInsert.Parameters.AddWithValue("P_CIDADE", null);
                        }

                        if (p.CodigoConvenio != 0)
                        {
                            cmdInsert.Parameters.AddWithValue("P_CONVENIO", p.CodigoConvenio);
                        }
                        else
                        {
                            cmdInsert.Parameters.AddWithValue("P_CONVENIO", null);
                        }

                        cmdInsert.Parameters.AddWithValue("P_CLINICA", CodigoClinica);

                        cmdInsert.ExecuteNonQuery();

                        Prontuario Prontuario = new Prontuario();

                        Prontuario.Cadastrar(this.Codigo, CodigoClinica);
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
        }

        public void Atualizar(Paciente p, int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(gestaoclinica.Models.Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  UPDATE
                                        PACIENTE
                                        SET
                                        P_CODIGO =@P_CODIGO,
                                        P_NOME =@P_NOME,
                                        P_SEXO =@P_SEXO,
                                        P_DATANASCIMENTO =@P_DATANASCIMENTO,
                                        P_PRONTUARIO =@P_PRONTUARIO,
                                        P_CPF =@P_CPF,
                                        P_ENDERECO =@P_ENDERECO,
                                        P_NUMEROENDERECO =@P_NUMEROENDERECO,
                                        P_BAIRRO =@P_BAIRRO,
                                        P_COMPENDERECO =@P_COMPENDERECO,
                                        P_RG =@P_RG,
                                        P_TELRESIDENCIAL =@P_TELRESIDENCIAL,
                                        P_TELCOMERCIAL =@P_TELCOMERCIAL,
                                        P_CELULAR =@P_CELULAR,
                                        P_EMAIL =@P_EMAIL,
                                        P_CIDADE =@P_CIDADE,
                                        P_CONVENIO =@P_CONVENIO
                                        WHERE
                                        P_CODIGO =@P_CODIGO AND
                                        P_CLINICA =@P_CLINICA"; 
                                        

                    Conexao.Open();

                    using (FbCommand cmdUpdate = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdUpdate.Parameters.AddWithValue("P_CODIGO", p.Codigo);
                        cmdUpdate.Parameters.AddWithValue("P_NOME", p.Nome);
                        cmdUpdate.Parameters.AddWithValue("P_SEXO", p.Sexo);
                        cmdUpdate.Parameters.AddWithValue("P_DATANASCIMENTO", p.DataNascimento);

                        if (p.Prontuario != 0)
                        {
                            cmdUpdate.Parameters.AddWithValue("P_PRONTUARIO", p.Prontuario);
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("P_PRONTUARIO", null);
                        }

                        cmdUpdate.Parameters.AddWithValue("P_CPF", p.CPF);
                        cmdUpdate.Parameters.AddWithValue("P_ENDERECO", p.Endereco);
                        cmdUpdate.Parameters.AddWithValue("P_NUMEROENDERECO", p.NumeroEndereco);
                        cmdUpdate.Parameters.AddWithValue("P_BAIRRO", p.Bairro);
                        cmdUpdate.Parameters.AddWithValue("P_COMPENDERECO", p.ComplementoEndereco);
                        cmdUpdate.Parameters.AddWithValue("P_RG", p.RG);
                        cmdUpdate.Parameters.AddWithValue("P_TELRESIDENCIAL", p.TelefoneResidencial);
                        cmdUpdate.Parameters.AddWithValue("P_TELCOMERCIAL", p.TelefoneComercial);
                        cmdUpdate.Parameters.AddWithValue("P_CELULAR", p.Celular);
                        cmdUpdate.Parameters.AddWithValue("P_EMAIL", p.Email);

                        if (p.CodigoCidade != 0)
                        {
                            cmdUpdate.Parameters.AddWithValue("P_CIDADE", p.CodigoCidade);
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("P_CIDADE", null);
                        }

                        if (p.CodigoConvenio != 0)
                        {
                            cmdUpdate.Parameters.AddWithValue("P_CONVENIO", p.CodigoConvenio);
                        }
                        else
                        {
                            cmdUpdate.Parameters.AddWithValue("P_CONVENIO", null);
                        }

                        cmdUpdate.Parameters.AddWithValue("P_CLINICA", CodigoClinica);

                        cmdUpdate.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Conexao.Close();
                }
            }
        }

        public void Excluir(Paciente p, int CodigoClinica)
        {
            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    Conexao.Open();

                    string TxtSQL = @"DELETE FROM PACIENTE WHERE P_CODIGO =@P_CODIGO AND P_CLINICA =@P_CLINICA";

                    using (FbCommand cmdDelete = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdDelete.Parameters.AddWithValue("P_CODIGO", p.Codigo);
                        cmdDelete.Parameters.AddWithValue("P_CLINICA", CodigoClinica);

                        cmdDelete.ExecuteNonQuery();
                    }
                }
                finally
                {
                    Conexao.Close();
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
                                        MAX(P_CODIGO) AS MAX_CODIGO
                                        FROM
                                        PACIENTE";

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

        public List<Paciente> ObterPacientes(int CodigoClinica, string Nome = "")
        {
            List<Paciente> Pacientes = new List<Paciente>();

            using (FbConnection Conexao = new FbConnection(Firebird.StringConexao))
            {
                try
                {
                    string TxtSQL = @"  SELECT
                                        *
                                        FROM
                                        PACIENTE
                                        WHERE
                                        P_CLINICA =@P_CLINICA ";

                    if (Nome != "")
                    {
                        TxtSQL = string.Concat(TxtSQL, "AND UPPER(", Firebird.CHARSET, "P_NOME) LIKE @P_NOME ");
                    }

                    Conexao.Open();

                    using (FbCommand cmdSelect = new FbCommand(TxtSQL, Conexao))
                    {
                        cmdSelect.Parameters.AddWithValue("P_CLINICA", CodigoClinica);

                        if (Nome != "")
                        {
                            cmdSelect.Parameters.AddWithValue("P_NOME", string.Concat("%", Nome.ToUpper(), "%"));
                        }

                        using (FbDataReader drSelect = cmdSelect.ExecuteReader())
                        {
                            while (drSelect.Read())
                            {
                                Pacientes.Add(new Paciente() 
                                { 
                                    Codigo = drSelect.GetInt32(drSelect.GetOrdinal("P_CODIGO")),
                                    Nome = drSelect.GetString(drSelect.GetOrdinal("P_NOME")),
                                    Sexo = ObterSexoVisaoUsuario(drSelect.GetString(drSelect.GetOrdinal("P_SEXO"))),
                                    DataNascimento = drSelect.GetDateTime(drSelect.GetOrdinal("P_DATANASCIMENTO")),
                                    CPF = drSelect.GetString(drSelect.GetOrdinal("P_CPF")),
                                    Email = drSelect.GetString(drSelect.GetOrdinal("P_EMAIL")),
                                    Celular = drSelect.GetString(drSelect.GetOrdinal("P_CELULAR"))
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

            return Pacientes;
        }

        private string ObterSexoVisaoUsuario(string Sexo)
        {
            if (Sexo.Equals("M"))
            {
                return "Masculino";
            }
            else
            {
                return "Feminino";
            }
        }

        public void CarregarAgendamentos(int CodigoClinica, int CodigoProfissional = 0)
        {
            Agenda a = new Agenda();

            this.Agendamentos = a.ObterAgendamentos(CodigoClinica, this.Codigo, CodigoProfissional);
        }

    }
}