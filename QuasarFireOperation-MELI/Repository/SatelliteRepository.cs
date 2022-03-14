using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using QuasarFireOperation_MELI.Utils;
using QuasarFireOperation_MELI.Models;
using System.Data.SqlClient;

namespace QuasarFireOperation_MELI.Repository
{
    public class SatelliteRepository
    {
        /// <summary>
        /// Consulta y Genera todos los satélites existenes en la Base de Datos.
        /// </summary>
        /// <returns>Lista que contiene los Satelites.</returns>
        public List<Satellite> getAllSatellites()
        {
            List<Satellite> SatelliteList = new List<Satellite>();

            Database DB = new Database();
            string SQL = "SELECT ID_Satellite FROM Satellite";

            DataTable Tabla = DB.Read(SQL);

            if (Tabla != null && Tabla.Rows.Count > 0)
            {
                foreach (DataRow DR in Tabla.Rows)
                {
                    int IDSat = Convert.ToInt32(DR["ID_Satellite"].ToString());
                    Satellite sat = getSatellite(IDSat);
                    if (sat != null)
                        SatelliteList.Add(sat);
                }
            }
            else
                SatelliteList = null;

            return SatelliteList;
        }
        /// <summary>
        /// Consulta y genera el satélite existente en la Base de Datos con el ID indicado.
        /// </summary>
        /// <param name="ID">ID del Satélite</param>
        /// <returns>Objeto Satellite que contiene los datos del mismo.</returns>
        public Satellite getSatellite(int ID)
        {
            if (ID <= 0)
                return null;

            Satellite sat = new Satellite();
            Database DB = new Database();
            string SQL = "SELECT s.ID_Satellite, s.Name_Satellite, sl.X_Coord, sl.Y_Coord, sh.Distance, sh.Message FROM Satellite s";
            SQL = SQL + " INNER JOIN SatelliteLocation sl ON sl.ID_Satellite = s.ID_Satellite AND sl.Until_Location IS NULL";
            SQL = SQL + " LEFT JOIN SatelliteHistory sh ON sh.ID_Satellite = s.ID_Satellite AND sh.Until_History IS NULL";
            SQL = SQL + " WHERE s.ID_Satellite = " + ID.ToString();
            DataTable Tabla = DB.Read(SQL);

            if (Tabla != null && Tabla.Rows.Count == 1)
            {
                var DR = Tabla.Rows[0];
                sat.ID = Convert.ToInt32(DR["ID_Satellite"].ToString());
                sat.Name = DR["Name_Satellite"].ToString();
                sat.Location = new Coordinate() { X = float.Parse(DR["X_Coord"].ToString()), Y = float.Parse(DR["Y_Coord"].ToString()) };
                
                if (!DR.IsNull("Distance"))
                    sat.Distance = float.Parse(DR["Distance"].ToString());
                else
                    sat.Distance = null;

                if (!DR.IsNull("Message"))
                    sat.Message = DR["Message"].ToString().Split(";");
                else
                    sat.Message = null;
            }
            else
                sat = null;

            return sat;
        }
        /// <summary>
        /// Consulta y genera el satélite existente en la Base de Datos con el nombre indicado.
        /// </summary>
        /// <param name="satName">Nombre del Satélite</param>
        /// <returns>Objeto Satellite que contiene los datos del mismo.</returns>
        public Satellite getSatellite(string satName)
        {
            if (satName == null || satName == "")
                return null;

            Database DB = new Database();
            string SQL = "SELECT ID_Satellite FROM Satellite WHERE Name_Satellite LIKE '" + satName.ToUpper() + "'";

            int satID = 0;
            DataTable Tabla = DB.Read(SQL);
            if (Tabla != null && Tabla.Rows.Count == 1)
                satID = Convert.ToInt32(Tabla.Rows[0]["ID_Satellite"]);

            return getSatellite(satID);
        }
        /// <summary>
        /// Actualización de los Datos de un Satélite.
        /// </summary>
        /// <param name="sat">Objeto Satellite a modificar.</param>
        /// <returns>True en caso de actualización correcta y False si falla.</returns>
        public bool updateSatellite(Satellite sat)
        {
            bool Flag = false;

            if (verificateSatellite(sat))
            {
                Database DB = new Database();
                DateTime ActDate = DateTime.Now;

                string SQL_Update = "UPDATE SatelliteHistory SET Until_History = '" + ActDate.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                SQL_Update = SQL_Update + " WHERE Until_History IS NULL AND ID_Satellite = " + sat.ID.ToString();

                string SQL_Insert = "INSERT INTO SatelliteHistory (ID_Satellite, Distance, Message, From_History) VALUES (";
                SQL_Insert = SQL_Insert + sat.ID.ToString() + "," + sat.Distance.ToString().Replace(",",".") + ", '" + String.Join(";", sat.Message) + "', '";
                SQL_Insert = SQL_Insert + ActDate.ToString("yyyy-MM-dd HH:mm:ss") + "')";

                SqlConnection conn = DB.getSQLConnection();
                conn.Open();
                SqlTransaction Tran = null;

                try
                {
                    Tran = conn.BeginTransaction();
                    DB.Query(SQL_Update, conn, Tran);
                    DB.Query(SQL_Insert, conn, Tran);
                    Tran.Commit();
                    Flag = true;
                }
                catch(Exception ex)
                {
                    if(Tran != null)
                        Tran.Rollback();
                }
                finally
                {
                    conn.Close();
                }
            }

            return Flag;
        }
        /// <summary>
        /// Controla que un objeto Satellite posea todos los datos necesarios.
        /// </summary>
        /// <param name="sat">Objeto Satellite a controlar.</param>
        /// <returns>True si posee todos los datos correctos. False en caso contrario.</returns>
        private bool verificateSatellite(Satellite sat)
        {
            bool Flag = true;

            if (Flag) Flag = sat.ID > 0 ? true : false;
            if (Flag) Flag = sat.Distance > 0 ? true : false;
            if (Flag) Flag = sat.Location != null ? true : false;
            if (Flag) Flag = sat.Message != null && sat.Message.Length > 0 ? true : false;
            if (Flag) Flag = sat.Name != "" ? true : false;

            return Flag;
        }
    }
}
