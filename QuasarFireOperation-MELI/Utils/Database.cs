using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Resources;

namespace QuasarFireOperation_MELI.Utils
{
    public class Database
    {
        private static string connectionString = "";
        public Database()
        {
            connectionString = Properties.Resources.connectionString;
        }
        /// <summary>
        /// Permite realizar una consulta en la Base de Datos.
        /// </summary>
        /// <param name="SQL">Código SQL de la Consulta</param>
        /// <returns>Objeto DataTable que contiene el resultset. Null en caso de error.</returns>
        public DataTable Read(string SQL)
        {
            SqlConnection con = new SqlConnection(connectionString);
            DataTable Read = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = con;
                cmd.CommandText = SQL;

                SqlDataReader dataReader = cmd.ExecuteReader();
                Read.Load(dataReader);

                con.Close();
            }
            catch
            {
                Read = null;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return Read;
        }
        /// <summary>
        /// Permite realizar una ejecución en la Base de Datos.
        /// </summary>
        /// <param name="SQL">Código SQL de la Ejecución</param>
        /// <param name="SQLConn">Opcional. Conexión a Utilizar (Múltiples ejecuciones)</param>
        /// <param name="Transac">Opcional. Debe existir una Conexión múltiple. Permite manejar una transacción de forma manual.</param>
        /// <returns>True en caso de ejecución correcta. False en caso incorrecto.</returns>
        public bool Query(string SQL, SqlConnection SQLConn = null, SqlTransaction Transac = null)
        {
            SqlConnection con;
            if (SQLConn == null)
                con = new SqlConnection(connectionString);
            else
                con = SQLConn;

            int Query = 0;
            try
            {
                if(SQLConn == null)
                    con.Open();

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = con;
                cmd.CommandText = SQL;

                if (Transac != null && SQLConn != null)
                    cmd.Transaction = Transac;

                Query = cmd.ExecuteNonQuery();
            }
            catch(Exception ex)
            {
                Query = 0;
            }
            finally
            {
                if (con.State == ConnectionState.Open && (SQLConn == null || Transac == null))
                    con.Close();
            }

            return Query > 0 ? true : false;
        }
        /// <summary>
        /// Crea y genera una nueva conexión SQL.
        /// </summary>
        /// <returns>Nueva conexión SQL</returns>
        public SqlConnection getSQLConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
