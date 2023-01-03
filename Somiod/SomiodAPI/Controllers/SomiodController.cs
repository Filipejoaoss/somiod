using Newtonsoft.Json;
using SomiodAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace SomiodAPI.Controllers
{
    public class SomiodController : ApiController
    {
        string connectionString = Properties.Settings.Default.connStr;
        MqttClient mqttClient = null;

        /*------------------------------------------------ APPLICATIONS ------------------------------------------------*/

        //Get All Applications
        [Route("api/somiod")]
        public HttpResponseMessage GetAllApplications()
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Applications");
            xml.AppendChild(root);

            string sql = "SELECT * FROM Applications";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        
                    };
                }

                while (reader.Read())
                {
                    XmlElement applicationElement = xml.CreateElement("Application");
                    root.AppendChild(applicationElement);

                    XmlElement idElement = xml.CreateElement("Id");
                    idElement.InnerText = reader["Id"].ToString();
                    applicationElement.AppendChild(idElement);

                    XmlElement nameAppElement = xml.CreateElement("NameApp");
                    nameAppElement.InnerText = (string)reader["NameApp"];
                    applicationElement.AppendChild(nameAppElement);

                    XmlElement creationDtElement = xml.CreateElement("Creation_dt");
                    creationDtElement.InnerText = (string)reader["Creation_dt"];
                    applicationElement.AppendChild(creationDtElement);

                    XmlElement resTypeElement = xml.CreateElement("Res_type");
                    resTypeElement.InnerText = (string)reader["Res_type"];
                    applicationElement.AppendChild(resTypeElement);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            string xmlString = xml.OuterXml;

            return new HttpResponseMessage()
            {
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }

        // GET Application by Id
        [Route("api/somiod/{id:int}")]
        public HttpResponseMessage GetByApplicationById(int id)
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Applications");
            xml.AppendChild(root);

            string sql = "SELECT * FROM Applications WHERE Id=@id";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    XmlElement applicationElement = xml.CreateElement("Application");
                    root.AppendChild(applicationElement);

                    XmlElement idElement = xml.CreateElement("Id");
                    idElement.InnerText = reader["Id"].ToString();
                    applicationElement.AppendChild(idElement);

                    XmlElement nameAppElement = xml.CreateElement("NameApp");
                    nameAppElement.InnerText = (string)reader["NameApp"];
                    applicationElement.AppendChild(nameAppElement);

                    XmlElement creationDtElement = xml.CreateElement("Creation_dt");
                    creationDtElement.InnerText = (string)reader["Creation_dt"];
                    applicationElement.AppendChild(creationDtElement);

                    XmlElement resTypeElement = xml.CreateElement("Res_type");
                    resTypeElement.InnerText = (string)reader["Res_type"];
                    applicationElement.AppendChild(resTypeElement);
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }

            string xmlString = xml.OuterXml;

            return new HttpResponseMessage()
            {
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }


        // POST Application
        [Route("api/somiod")]
        public async Task<IHttpActionResult> PostApplication()
        {
            XmlDocument xmlDocument = new XmlDocument();

            using (var contentStream = await Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    xmlDocument.LoadXml(rawContent);
                }
            }

            XmlNode nameAppNode = xmlDocument.SelectSingleNode("/root/NameApp");
            string nameApp = nameAppNode.InnerText;
            XmlNode resTypeNode = xmlDocument.SelectSingleNode("/root/Res_type");
            string resType = resTypeNode.InnerText;

            string sql = "INSERT INTO Applications VALUES(@nameApp, @creation_dt, @res_type)";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);


                command.Parameters.AddWithValue("@nameApp", nameApp);
                command.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@res_type", resType);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok();
                }

                return InternalServerError();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return InternalServerError();
                }
            }

            return null;
        }

        // PUT Application 
        [Route("api/somiod/{id:int}")]
        public async Task<IHttpActionResult> PutApplicationAsync(int id)
        {
            XmlDocument xmlDocument = new XmlDocument();

            using (var contentStream = await Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    xmlDocument.LoadXml(rawContent);
                }
            }

            XmlNode nameAppNode = xmlDocument.SelectSingleNode("/root/NameApp");
            string nameApp = nameAppNode.InnerText;
            XmlNode resTypeNode = xmlDocument.SelectSingleNode("/root/Res_type");
            string resType = resTypeNode.InnerText;


            string sql = "UPDATE Applications SET NameApp=@nameApp, Res_type=@res_type WHERE Id=@id";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nameApp", nameApp);
                command.Parameters.AddWithValue("@res_type", resType);
                command.Parameters.AddWithValue("@id", id);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return InternalServerError();
                }
            }

            return null;
        }

        // DELETE Application
        [Route("api/somiod/{id:int}")]
        public IHttpActionResult DeleteApplication(int id)
        {
            List<Module> listModules = new List<Module>();

            listModules = GetModuleByApplication(id);

            for (int i = 0; i < listModules.Count(); i++)
            {
                int idMod = listModules[i].Id;
                DeleteModule(idMod);
            }

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string sql = "DELETE FROM Applications WHERE Id=@id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                return InternalServerError();
            }
        }

        /*------------------------------------------------ MODULES ------------------------------------------------*/

        // GET All Modules
        [Route("api/somiod/{nameApp:alpha}")]
        public HttpResponseMessage GetAllModules()
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Modules");
            xml.AppendChild(root);

            string sql = "SELECT * FROM Modules";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.NotFound,

                    };
                }

                while (reader.Read())
                {
                    XmlElement moduleElement = xml.CreateElement("Module");
                    root.AppendChild(moduleElement);

                    XmlElement idElement = xml.CreateElement("Id");
                    idElement.InnerText = reader["Id"].ToString();
                    moduleElement.AppendChild(idElement);

                    XmlElement nameModElement = xml.CreateElement("NameMod");
                    nameModElement.InnerText = (string)reader["NameMod"];
                    moduleElement.AppendChild(nameModElement);

                    XmlElement creationDtElement = xml.CreateElement("Creation_dt");
                    creationDtElement.InnerText = (string)reader["Creation_dt"];
                    moduleElement.AppendChild(creationDtElement);

                    XmlElement parentElement = xml.CreateElement("Parent");
                    parentElement.InnerText = reader["Parent"].ToString();
                    moduleElement.AppendChild(parentElement);

                    XmlElement resTypeElement = xml.CreateElement("Res_type");
                    resTypeElement.InnerText = (string)reader["Res_type"];
                    moduleElement.AppendChild(resTypeElement);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            string xmlString = xml.OuterXml;

            return new HttpResponseMessage()
            {
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }

        // GET Module By Id
        [Route("api/somiod/{nameApp}/{id}")]
        public HttpResponseMessage GetByModuleById(int id)
        {
            XmlDocument xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Modules");
            xml.AppendChild(root);

            string sql = "SELECT * FROM Modules WHERE Id=@id";
            string sqlData = "SELECT id, content, creation_dt, parent, res_type FROM Datas WHERE Parent=@id";

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    XmlElement moduleElement = xml.CreateElement("Module");
                    root.AppendChild(moduleElement);

                    XmlElement idElement = xml.CreateElement("Id");
                    idElement.InnerText = reader["Id"].ToString();
                    moduleElement.AppendChild(idElement);

                    XmlElement nameModElement = xml.CreateElement("NameMod");
                    nameModElement.InnerText = (string)reader["NameMod"];
                    moduleElement.AppendChild(nameModElement);

                    XmlElement creationDtElement = xml.CreateElement("Creation_dt");
                    creationDtElement.InnerText = (string)reader["Creation_dt"];
                    moduleElement.AppendChild(creationDtElement);

                    XmlElement parentElement = xml.CreateElement("Parent");
                    parentElement.InnerText = reader["Parent"].ToString();
                    moduleElement.AppendChild(parentElement);

                    XmlElement resTypeElement = xml.CreateElement("Res_type");
                    resTypeElement.InnerText = (string)reader["Res_type"];
                    moduleElement.AppendChild(resTypeElement);

                    reader.Close();
                    connection.Close();

                    connection.Open();

                    SqlCommand commandData = new SqlCommand(sqlData, connection);
                    commandData.Parameters.AddWithValue("@id", id);
                    SqlDataReader readerData = commandData.ExecuteReader();

                    while (readerData.Read())
                    {
                        XmlElement dataElement = xml.CreateElement("Data");
                        moduleElement.AppendChild(dataElement);

                        XmlElement idDataElement = xml.CreateElement("Id");
                        idDataElement.InnerText = readerData["Id"].ToString();
                        dataElement.AppendChild(idDataElement);

                        XmlElement contentElement = xml.CreateElement("Content");
                        contentElement.InnerText = (string)readerData["Content"];
                        dataElement.AppendChild(contentElement);

                        XmlElement creationDtDataElement = xml.CreateElement("Creation_dt");
                        creationDtDataElement.InnerText = (string)readerData["Creation_dt"];
                        dataElement.AppendChild(creationDtDataElement);

                        XmlElement parentDataElement = xml.CreateElement("Parent");
                        parentDataElement.InnerText = readerData["Parent"].ToString();
                        dataElement.AppendChild(parentDataElement);

                        XmlElement resTypeDataElement = xml.CreateElement("Res_type");
                        resTypeDataElement.InnerText = (string)readerData["Res_type"];
                        dataElement.AppendChild(resTypeDataElement);
                    }

                    readerData.Close();
                    connection.Close();
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
            }

            string xmlString = xml.OuterXml;

            return new HttpResponseMessage()
            {
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }

        // POST Module
        [Route("api/somiod/{nameApp}")]
        public async Task<IHttpActionResult> PostModule(string nameApp)
        {
            int parent = GetApplicationId(nameApp);

            if (parent == -1)
            {
                return InternalServerError();
            }

            XmlDocument xmlDocument = new XmlDocument();

            using (var contentStream = await Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    xmlDocument.LoadXml(rawContent);

                }
            }

            XmlNode nameModNode = xmlDocument.SelectSingleNode("/root/NameMod");
            string nameMod = nameModNode.InnerText;
            XmlNode resTypeNode = xmlDocument.SelectSingleNode("/root/Res_type");
            string resType = resTypeNode.InnerText;


            string sql = "INSERT INTO Modules VALUES(@nameMod, @creation_dt, @parent, @res_type)";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("nameMod", nameMod);
                command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("parent", parent);
                command.Parameters.AddWithValue("res_type", resType);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok();
                }

                return InternalServerError();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return InternalServerError();
                }
            }

            return null;
        }

        // PUT Module
        [Route("api/somiod/{nomeApp}/{id}")]
        public async Task<IHttpActionResult> PutModule(int id)
        {
            XmlDocument xmlDocument = new XmlDocument();

            using (var contentStream = await Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    xmlDocument.LoadXml(rawContent);
                }
            }

            XmlNode nameModNode = xmlDocument.SelectSingleNode("/root/NameMod");
            string nameMod = nameModNode.InnerText;
            XmlNode parentNode = xmlDocument.SelectSingleNode("/root/Parent");
            int parent = Convert.ToInt32(parentNode.InnerText);
            XmlNode resTypeNode = xmlDocument.SelectSingleNode("/root/Res_type");
            string resType = resTypeNode.InnerText;

            string sql = "UPDATE Modules SET NameMod=@nameMod, Parent=@parent ,Res_type=@res_type WHERE Id=@id";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nameMod", nameMod);
                command.Parameters.AddWithValue("@parent", parent);
                command.Parameters.AddWithValue("@res_type", resType);
                command.Parameters.AddWithValue("@id", id);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok();
                }
                return NotFound();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return InternalServerError();
                }
            }

            return null;
        }

        // DELETE Module
        [Route("api/somiod/{nameApp}/{id}")]
        public IHttpActionResult DeleteModule(int id)
        {
            List<DataSub> listDataSubs = new List<DataSub>();

            listDataSubs = GetDataSubs(id);

            for (int i = 0; i < listDataSubs.Count(); i++)
            {
                int idDataSub = listDataSubs[i].Id;
                string resTypeDataSub = listDataSubs[i].Res_type;

                DeleteDataSub(idDataSub, resTypeDataSub);
            }

            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                string sql = "DELETE FROM Modules WHERE Id=@id";
                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);

                int numRegistos = command.ExecuteNonQuery();

                connection.Close();

                if (numRegistos > 0)
                {
                    return Ok("Module Delete with success");
                }
                return NotFound();

            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                return InternalServerError();
            }
        }
        /*------------------------------------------------ DATA AND SUBSCRIPTION ------------------------------------------------*/
        // POST Data and Subscription 
        [Route("api/somiod/{nameApp}/{nameMod}")]
        public async Task<IHttpActionResult> PostDataSuB(string nameMod)
        {
            int parent = GetModuleId(nameMod);

            if (parent == -1)
            {
                return InternalServerError();
            }

            XmlDocument xmlDocument = new XmlDocument();

            using (var contentStream = await Request.Content.ReadAsStreamAsync())
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(contentStream))
                {
                    string rawContent = sr.ReadToEnd();
                    xmlDocument.LoadXml(rawContent);
                }
            }

            XmlNode resTypeNode = xmlDocument.SelectSingleNode("/root/Res_type");
            string resType = resTypeNode.InnerText;

            SqlConnection connection = null;

            if (resType == "data")
            {
                XmlNode contentNode = xmlDocument.SelectSingleNode("/root/Content");
                string content = contentNode.InnerText;

                string sql = "INSERT INTO Datas VALUES(@content, @creation_dt, @parent, @res_type)";

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("content", content);
                    command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("parent", parent);
                    command.Parameters.AddWithValue("res_type", resType);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        string eventData = "creation";
                        string endPoint = "127.0.0.1";

                        sendNotification(nameMod, eventData, endPoint, content);
                        return Ok();
                    }

                    return InternalServerError();
                }
                catch (Exception)
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        return InternalServerError();
                    }
                }
            }
            else if (resType == "subscription")
            {
                XmlNode nameSubNode = xmlDocument.SelectSingleNode("/root/NameSub");
                string nameSub = nameSubNode.InnerText;
                XmlNode eventNode = xmlDocument.SelectSingleNode("/root/Event");
                string eventSub = eventNode.InnerText;
                XmlNode endPointNode = xmlDocument.SelectSingleNode("/root/EndPoint");
                string endPoint = endPointNode.InnerText;

                string sql = "INSERT INTO Subscriptions VALUES(@nameSub, @creation_dt, @parent, @event, @endpoint, @res_type)";

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("nameSub", nameSub);
                    command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("parent", parent);
                    command.Parameters.AddWithValue("event", eventSub);
                    command.Parameters.AddWithValue("endpoint", endPoint);
                    command.Parameters.AddWithValue("res_type", resType);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        subChannel(nameMod, eventSub, endPoint);

                        return Ok();
                    }
                    return InternalServerError();
                }
                catch (Exception)
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                        return InternalServerError();
                    }
                }
            }
            return null;
        }

        // Get Data By Time
        [Route("api/somiod/{nameApp}/{nameMod}/{res_type}")]
        public HttpResponseMessage getLastData(string nameMod)
        {
            int parent = GetModuleId(nameMod);

            XmlDocument xml = new XmlDocument();

            XmlElement root = xml.CreateElement("Datas");
            xml.AppendChild(root);

            string sql = "SELECT TOP 1 * FROM Datas WHERE parent=@parent ORDER BY ID DESC";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@parent", parent);
                SqlDataReader reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    return new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.NotFound,

                    };
                }

                while (reader.Read())
                {
                    XmlElement dataElement = xml.CreateElement("Data");
                    root.AppendChild(dataElement);
                    
                    XmlElement idElement = xml.CreateElement("Id");
                    idElement.InnerText = reader["Id"].ToString();
                    dataElement.AppendChild(idElement);

                    XmlElement contentElement = xml.CreateElement("Content");
                    contentElement.InnerText = (string)reader["content"];
                    dataElement.AppendChild(contentElement);

                    XmlElement creationDtDataElement = xml.CreateElement("Creation_dt");
                    creationDtDataElement.InnerText = (string)reader["Creation_dt"];
                    dataElement.AppendChild(creationDtDataElement);

                    XmlElement parentDataElement = xml.CreateElement("Parent");
                    parentDataElement.InnerText = reader["Parent"].ToString();
                    dataElement.AppendChild(parentDataElement);

                    XmlElement resTypeDataElement = xml.CreateElement("Res_type");
                    resTypeDataElement.InnerText = (string)reader["Res_type"];
                    dataElement.AppendChild(resTypeDataElement);
                }

                reader.Close();
                connection.Close();
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            
            string xmlString = xml.OuterXml;

            return new HttpResponseMessage()
            {
                Content = new StringContent(xmlString, Encoding.UTF8, "application/xml")
            };
        }

        // DELETE Data and Subscription
        [Route("api/somiod/{nameApp}/{nameMod}/{id}/{res_type}")]
        public IHttpActionResult DeleteDataSub(int id, string res_type)
        {
            SqlConnection connection = null;

            if (res_type == "data")
            {
                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string sql = "DELETE FROM Datas WHERE Id=@id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", id);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        return Ok();
                    }
                    return NotFound();

                }
                catch (Exception)
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return InternalServerError();
                }
            }
            else if (res_type == "subscription")
            {
                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    string sql = "DELETE FROM Subscriptions WHERE Id=@id";
                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("@id", id);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        return Ok();
                    }
                    return NotFound();

                }
                catch (Exception)
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return InternalServerError();
                }
            }

            return null;
        }

        /*------------------------------------------------ AUXILIARY FUNCTIONS ------------------------------------------------*/

        private int GetApplicationId(string name)
        {
            int id = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT id FROM Applications WHERE nameApp = @nameApp", connection))
                {
                    command.Parameters.AddWithValue("@nameApp", name);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = (int)reader["Id"];
                        }
                    }

                    connection.Close();
                }
            }
            return id;
        }

        private int GetModuleId(string name)
        {
            int id = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT id FROM Modules WHERE NameMod = @nameMod", connection))
                {
                    command.Parameters.AddWithValue("@nameMod", name);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            id = (int)reader["Id"];
                        }
                    }

                    connection.Close();
                }
            }
            return id;
        }
        private List<Module> GetModuleByApplication(int id)
        {
            List<Module> listModules = new List<Module>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT id FROM Modules WHERE Parent = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listModules.Add(new Module
                            {
                                Id = (int)reader["Id"]
                            }
                            );
                        }
                    }

                    connection.Close();
                }
            }
            return listModules;

        }

        private List<DataSub> GetDataSubs(int id)
        {
            List<DataSub> listDataSubs = new List<DataSub>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT id, res_type FROM Datas WHERE parent = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listDataSubs.Add(new DataSub
                            {
                                Id = (int)reader["Id"],
                                Res_type = (string)reader["Res_type"]
                            }
                            );
                        }
                    }

                    connection.Close();
                }

                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT id, res_type FROM Subscriptions WHERE parent = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            listDataSubs.Add(new DataSub
                            {
                                Id = (int)reader["Id"],
                                Res_type = (string)reader["Res_type"]
                            }
                            );
                        }
                    }

                    connection.Close();
                }

                return listDataSubs;
            }
        }

        private void subChannel(string nameMod, string eventData, string endPoint)
        {
            string[] topics = { nameMod };

            mqttClient = new MqttClient(IPAddress.Parse(endPoint));
            mqttClient.Connect(Guid.NewGuid().ToString());

            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("Error connecting to message broker");
                return;
            }

            if (eventData == "creation")
            {
                mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceivedCreation;
            }
            else if (eventData == "deletion")
            {
                mqttClient.MqttMsgPublishReceived += client_MqttMsgPublishReceivedDeletion;
            }

            mqttClient.Subscribe(topics, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private void client_MqttMsgPublishReceivedCreation(object sender, MqttMsgPublishEventArgs e)
        {
            MessageBox.Show(e.Topic + "channel:\n  Data with " + Encoding.UTF8.GetString(e.Message) + " content was related to a creation event");
        }

        private void client_MqttMsgPublishReceivedDeletion(object sender, MqttMsgPublishEventArgs e)
        {
            MessageBox.Show(e.Topic + "channel:\n  Data with " + Encoding.UTF8.GetString(e.Message) + " content was related to a deletion event");
        }

        private void sendNotification(string nameMod, string eventData, string endPoint, string content)
        {
            mqttClient = new MqttClient(IPAddress.Parse(endPoint));
            mqttClient.Connect(Guid.NewGuid().ToString());

            if (!mqttClient.IsConnected)
            {
                Console.WriteLine("Error connecting to message broker");
                return;
            }

            mqttClient.Publish(nameMod, Encoding.UTF8.GetBytes(content));
        }
    }
}