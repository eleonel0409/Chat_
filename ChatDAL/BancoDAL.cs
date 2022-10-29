using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace SCA.DAL.Login
{
    public class BancoDAL
    {

        string connectionString = "Password=GLAP8Le9mb;Persist Security Info=True;User ID=SCA;Initial Catalog=SCA;Data Source=ws1832;connection timeout=300";

        private SqlConnection connection;
        private SqlParameter[] parameters;
        private SqlCommand command;
        public SqlParameter[] Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        private SqlParameter[] parameters_;

        public SqlCommand Command
        {
            get
            {
                command.CommandTimeout = 30000;
                return command;
            }
            set { command = value; }
        }
        protected Typ PegarItem<Typ>(DataRow registro)
        {
            Type temp = typeof(Typ);
            Typ obj = Activator.CreateInstance<Typ>();

            foreach (DataColumn coluna in registro.Table.Columns)
            {
                foreach (PropertyInfo propriedade in temp.GetProperties())
                {
                    if (propriedade.Name == coluna.ColumnName)
                    {
                        if (registro[coluna.ColumnName] != DBNull.Value)
                        {
                            propriedade.SetValue(obj, registro[coluna.ColumnName], null);
                        }

                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return obj;
        }

        protected List<Typ> ConverterParaLista<Typ>(DataTable dt)
        {
            List<Typ> lst = new List<Typ>();
            foreach (DataRow row in dt.Rows)
            {
                Typ item = PegarItem<Typ>(row);
                lst.Add(item);
            }
            return lst;
        }
        public DataTable ExecuteDataTable(string pSqlCommand, System.Data.CommandType pCommandType)
        {
            parameters = new SqlParameter[0];
            return ExecuteDataTable(pSqlCommand, pCommandType, parameters);
        }

        public DataTable ExecuteDataTable(string pSqlCommand, System.Data.CommandType pCommandType, SqlParameter[] pParameters)
        {
            try
            {
                using (Connection)
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();

                    Command = new SqlCommand(pSqlCommand, connection);
                    Command.CommandType = pCommandType;
                    Command.CommandTimeout = 60;

                    if (pParameters != null)
                        Command.Parameters.AddRange(pParameters);

                    SqlDataAdapter sda = new SqlDataAdapter(Command);

                    DataTable dt = new DataTable();

                    sda.Fill(dt);

                    return dt;
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //if (Connection.State == ConnectionState.Open)
                //    Connection.Close();
            }
        }

        public SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();
                }
                else
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Dispose();
                        connection = new SqlConnection(connectionString);
                        connection.Open();
                    }
                }
                return connection;
            }
            set { connection = value; }
        }

        public object ExecuteScalar(string pSqlCommand, System.Data.CommandType pCommandType, SqlParameter[] pParameters)
        {
            try
            {
                using (connection = new SqlConnection(connectionString))
                {
                    if (connection.State == System.Data.ConnectionState.Closed)
                        connection.Open();

                    Command = new SqlCommand(pSqlCommand, Connection);
                    Command.CommandType = pCommandType;

                    Command.Parameters.AddRange(pParameters);

                    return Command.ExecuteScalar();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int ExecuteNonQuery(string pSqlCommand, System.Data.CommandType pCommandType, SqlParameter[] pParameters)
        {
            try
            {
                using (Connection)
                {

                    Command = new SqlCommand(pSqlCommand, connection);
                    Command.CommandType = pCommandType;

                    Command.Parameters.AddRange(pParameters);

                    return Command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                //connection.Close();
            }
        }

        public int Validar(int codigo, string senha)
        {
            var valido = 0;

            try
            {
                //LoginInfo objUsuario = new LoginInfo();
                using (Connection)
                {
                    Parameters = new SqlParameter[2];
                    Parameters[0] = new SqlParameter("@login", codigo);
                    Parameters[1] = new SqlParameter("@senha", senha);
                    valido = Convert.ToInt32(ExecuteScalar("usp_ValidarLogin", System.Data.CommandType.StoredProcedure, Parameters));
                     
                }

                return valido;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /*public List<ObjetoInfo> ObterFolhaRosto(string pTipo)
        {
            List<ObjetoInfo> lstFolhaRosto = new List<ObjetoInfo>();
            try
            {
                DataTable dt;

                using (Connection)
                {
                    Parameters = new SqlParameter[1];
                    Parameters[0] = new SqlParameter("@TIPO", pTipo);
                    dt = ExecuteDataTable("usp_ObterListaColeta", CommandType.StoredProcedure, Parameters);
                }
                if (dt.Rows.Count > 0)
                {
                    lstFolhaRosto = ConverterParaLista<ObjetoInfo>(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstFolhaRosto;
        }*/

       /* public List<EdicaoInfo> ObterTipos()
        {
            List<EdicaoInfo> lstTipos = new List<EdicaoInfo>();
            try
            {
                DataTable dt;

                using (Connection)
                {
                    //Parameters = new SqlParameter[1];
                    //Parameters[0] = new SqlParameter("@TIPO", pTipo);
                    dt = ExecuteDataTable("usp_ObterTiposEdicao", CommandType.StoredProcedure);
                }
                if (dt.Rows.Count > 0)
                {
                    lstTipos = ConverterParaLista<EdicaoInfo>(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstTipos;
        }*/

      /*public void InserirLog(LogInfo objLog)
        {
            try
            {
                using (Connection)
                {
                    Parameters = new SqlParameter[6];
                    Parameters[0] = new SqlParameter("@COD_FOLHAROSTO", objLog.COD_FOLHAROSTO);
                    Parameters[1] = new SqlParameter("@COD_MIOLO", objLog.COD_MIOLO);
                    Parameters[2] = new SqlParameter("@DESTINATARIO", objLog.DESTINATARIO);
                    Parameters[3] = new SqlParameter("@TIPO", objLog.TIPO);
                    Parameters[4] = new SqlParameter("@USUARIO", objLog.USUARIO);
                    Parameters[5] = new SqlParameter("@SUCESSO", objLog.SUCESSO);

                    ExecuteNonQuery("usp_InserirLogColeta", System.Data.CommandType.StoredProcedure, Parameters);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }*/

        /*public List<LogInfo> ObterLog(string pTipo)
        {
            List<LogInfo> lstLog = new List<LogInfo>();
            try
            {
                DataTable dt;

                using (Connection)
                {
                    Parameters = new SqlParameter[1];
                    Parameters[0] = new SqlParameter("@TIPO", pTipo);
                    dt = ExecuteDataTable("usp_ObterLogColeta", CommandType.StoredProcedure, Parameters);
                }
                if (dt.Rows.Count > 0)
                {
                    lstLog = ConverterParaLista<LogInfo>(dt);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return lstLog;
        }*/
    }
}
