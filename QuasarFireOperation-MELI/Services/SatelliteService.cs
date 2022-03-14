using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QuasarFireOperation_MELI.Models;
using QuasarFireOperation_MELI.Utils;
using QuasarFireOperation_MELI.Repository;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace QuasarFireOperation_MELI.Services
{
    public class SatelliteService
    {
        #region Public Functions
        /// <summary>
        /// Genera la respuesta para el controlador TopSecret
        /// </summary>
        /// <param name="JSON">JSON con datos del body del Controlador.</param>
        /// <returns>Respuesta del Controlador.</returns>
        public dynamic getTopSecretResponse(dynamic JSON)
        {
            SatelliteRepository repo = new SatelliteRepository();
            List<Satellite> SatelliteList = deserializeTopSecret(JSON);

            if (SatelliteList == null || SatelliteList.Count != 3)
                return null;

            if (!validateSatelliteList(SatelliteList))
                return null;

            foreach (Satellite s in SatelliteList)
            {
                Satellite sat = repo.getSatellite(s.Name.ToUpper());
                sat.Distance = s.Distance;
                sat.Message = s.Message;
                repo.updateSatellite(sat);
            }

            SatelliteList.Clear();
            SatelliteList = repo.getAllSatellites();

            Coordinate coordResponse = getLocation(SatelliteList[0], SatelliteList[1], SatelliteList[2]);
            string messageResponse = getMessage(SatelliteList[0].Message, SatelliteList[1].Message, SatelliteList[2].Message);

            if (coordResponse != null && messageResponse != null)
            {
                TopSecretResponse response = new TopSecretResponse();
                response.message = messageResponse;
                response.position = coordResponse;
                dynamic jsonResponse = JsonConvert.SerializeObject(response);
                return jsonResponse;
            }
            else
                return null;
        }
        /// <summary>
        /// Genera la respuesta para el controlador TopSecretSplit [GET]
        /// </summary>
        /// <param name="satellite_name">Nombre del satélite pasado como parámetro en el Controlador.</param>
        /// <returns>Respuesta del Controlador.</returns>
        public dynamic getTopSecretSplitResponse(string satellite_name)
        {
            SatelliteRepository repo = new SatelliteRepository();

            Satellite sat = repo.getSatellite(satellite_name.ToUpper());

            if (sat != null)
            {
                TopSecretResponse response = new TopSecretResponse();
                response.message = sat.Message == null || sat.Message.Length <= 0 ? "No hay suficiente información para revelar el mensaje" : String.Join(" ", sat.Message);
                response.position = sat.Location;
                dynamic jsonResponse = JsonConvert.SerializeObject(response);
                return jsonResponse;
            }
            else
                return null;
        }
        /// <summary>
        /// Genera la respuesta para el controlador TopSecretSplit [POST]
        /// </summary>
        /// <param name="satellite_name">Nombre del satélite pasado en el endpoint.</param>
        /// <param name="JSON">JSON con datos del body del Controlador.</param>
        /// <returns>True en caso de actualizar los datos de forma correcta. False en caso contrario.</returns>
        public bool getTopSecretSplitResponse(string satellite_name, dynamic JSON)
        {
            SatelliteRepository repo = new SatelliteRepository();

            float Distance = 0;
            string[] Message = null;

            dynamic reader = JsonConvert.DeserializeObject(Convert.ToString(JSON));
            Distance = reader.distance != null ? reader.distance : 0;
            Message = reader.message != null ? reader.message.ToObject<string[]>() : null;

            if (Distance > 0 && Message != null && Message.Length > 0)
            {
                Satellite sat = repo.getSatellite(satellite_name.ToUpper());
                sat.Distance = Distance;
                sat.Message = Message;
                return repo.updateSatellite(sat);
            }
            else return false;
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Calcula la Posición del Emisor.
        /// </summary>
        /// <param name="DistanceKenobi">Distancia del Satélite Kenobi al Emisor.</param>
        /// <param name="DistanceSkywalker">Distancia del Satélite Skywalker al Emisor.</param>
        /// <param name="DistanceSato">Distancia del Satélite Sato al Emisor.</param>
        /// <returns>Objeto Coordenada con la posición X e Y del Emisor.</returns>
        private Coordinate getLocation(float DistanceKenobi, float DistanceSkywalker, float DistanceSato)
        {
            SatelliteRepository repo = new SatelliteRepository();
            List<Satellite> SatelliteList = repo.getAllSatellites();

            if (SatelliteList.Count != 3)
                return null;

            List<Point> PointList = new List<Point>();

            Dictionary<string, float> Dict = new Dictionary<string, float>() { { "KENOBI", DistanceKenobi }, { "SKYWALKER", DistanceSkywalker }, { "SATO", DistanceSato } };
            foreach(Satellite s in SatelliteList)
                PointList.Add(new Point(s.Location.X, s.Location.Y, Dict[s.Name.ToUpper()]));

            if (PointList.Count != 3)
                return null;

            double[] Location = Trilateration.Compute(PointList[0], PointList[1], PointList[2]);

            Coordinate coordReturn = new Coordinate();
            if (Location.Length == 2)
            {
                coordReturn.X = float.Parse(Location[0].ToString());
                coordReturn.Y = float.Parse(Location[1].ToString());
            }
            else
                coordReturn = null;

            return coordReturn;
        }
        /// <summary>
        /// Calcula la Posición del Emisor.
        /// </summary>
        /// <param name="Kenobi">Objeto Satellite Kenobi.</param>
        /// <param name="Skywalker">Objeto Satellite Skywalker.</param>
        /// <param name="Sato">Objeto Satellite Sato.</param>
        /// <returns>Objeto Coordenada con la posición X e Y del Emisor.</returns>
        private Coordinate getLocation(Satellite Kenobi, Satellite Skywalker, Satellite Sato)
        {
            if (Kenobi == null || Skywalker == null || Sato == null)
                return null;

            List<Point> PointList = new List<Point>();
            List<Satellite> SatelliteList = new List<Satellite>() { Kenobi, Skywalker, Sato };
            foreach (Satellite s in SatelliteList)
                PointList.Add(new Point(s.Location.X, s.Location.Y, Convert.ToDouble(s.Distance)));

            double[] Location = Trilateration.Compute(PointList[0], PointList[1], PointList[2]);

            Coordinate coordReturn = new Coordinate();
            if (Location != null && Location.Length >= 2)
            {
                coordReturn.X = float.Parse(Math.Round(Location[0],2).ToString());
                coordReturn.Y = float.Parse(Math.Round(Location[1],2).ToString());
            }
            else
                coordReturn = null;

            return coordReturn;
        }
        /// <summary>
        /// Descifra el mensaje del Emisor
        /// </summary>
        /// <param name="MessageKenobi">Mensaje recibido en el satélite Kenobi.</param>
        /// <param name="MessageSkywalker">Mensaje recibido en el satélite Skywalker.</param>
        /// <param name="MessageSato">Mensaje recibido en el satélite Sato.</param>
        /// <returns>Mensaje Descifrado. Si no es posible descifrar, retorna null.</returns>
        private string getMessage(string[] MessageKenobi, string[] MessageSkywalker, string[] MessageSato)
        {
            if (MessageKenobi == null || MessageSkywalker == null || MessageSato == null)
                return null;

            int MaxWords = 0;
            MaxWords = MessageKenobi.Length > MessageSkywalker.Length ? MessageKenobi.Length : MessageSkywalker.Length;
            MaxWords = MaxWords > MessageSato.Length ? MaxWords : MessageSato.Length;

            string[] retMessage = new string[MaxWords];

            for (int i = 0; i < MessageKenobi.Length; i++)
                retMessage[i] = MessageKenobi[i].Trim() != "" ? MessageKenobi[i] : retMessage[i];

            for (int i = 0; i < MessageSkywalker.Length; i++)
                retMessage[i] = MessageSkywalker[i].Trim() != "" ? MessageSkywalker[i] : retMessage[i];

            for (int i = 0; i < MessageSato.Length; i++)
                retMessage[i] = MessageSato[i].Trim() != "" ? MessageSato[i] : retMessage[i];

            bool messageDecrypted = true;
            for (int i = 0; i < retMessage.Length && messageDecrypted; i++)
                if (retMessage[i] == null || retMessage[i].Trim() == "")
                    messageDecrypted = false;

            return messageDecrypted? String.Join(" ", retMessage) : null;
        }
        /// <summary>
        /// Deserializa un JSON en objetos Satellite
        /// </summary>
        /// <param name="JSON">JSON con los datos a deserializar.</param>
        /// <param name="SplitResponse">Parámetro que indica si es para el controlador TopSecret o TopSecret_Split</param>
        /// <returns>Lista de Satélites que posee el JSON.</returns>
        private List<Satellite> deserializeTopSecret(dynamic JSON, bool SplitResponse = false)
        {
            JObject jsonParse = JObject.Parse(Convert.ToString(JSON));
            List<JToken> jsonResults = null;
            if (!SplitResponse)
                jsonResults = jsonParse["satellites"].Children().ToList();
            else
                jsonResults = jsonParse.Children().ToList();

            List<Satellite> SatelliteList = new List<Satellite>();

            foreach (JToken r in jsonResults)
            {
                Satellite s = JsonConvert.DeserializeObject<Satellite>(r.ToString());
                SatelliteList.Add(s);
            }

            return SatelliteList.Count() > 0 ? SatelliteList : null;
        }
        /// <summary>
        /// Valida los Datos de un satélite
        /// </summary>
        /// <param name="sat">Objeto Satellite a validar.</param>
        /// <returns>True en caso correcto. False en caso contrario.</returns>
        private bool validateSatellite(Satellite sat)
        {
            return (sat.Distance != null && sat.Distance > 0) && (sat.Message != null && sat.Message.Length > 0) && (sat.Name != null && sat.Name != "") ? true : false;
        }
        /// <summary>
        /// Valida los Datos de una Lista de Satélites
        /// </summary>
        /// <param name="list">Lista de Objetos Satellite.</param>
        /// <returns>True en caso correcto. False en caso contrario.</returns>
        private bool validateSatelliteList(List<Satellite> list)
        {
            bool Flag = true;
            foreach (Satellite s in list)
                if (Flag) Flag = validateSatellite(s);
            return Flag;
        }
        #endregion
    }
}
