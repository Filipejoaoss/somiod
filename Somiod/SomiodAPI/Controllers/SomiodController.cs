using SomiodAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Web.Http;
using System.Windows.Forms;
using System.Xml.Linq;
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
        public IEnumerable<Models.Application> GetAllApplications()
        {
            List<Models.Application> listApplications = new List<Models.Application>();
            string sql = "SELECT * FROM Applications";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Models.Application application = new Models.Application
                    {
                        Id = (int)reader["ID"],
                        NameApp = (string)reader["NameApp"],
                        Creation_dt = (string)reader["Creation_dt"],
                        Res_type = (string)reader["Res_type"]
                    };

                    listApplications.Add(application);
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

            return listApplications;
        }

        // GET Application by Id
        [Route("api/somiod/{id:int}")]
        public IHttpActionResult GetByApplicationById(int id)
        {
            string sql = "SELECT * FROM Applications WHERE Id=@id";
            SqlConnection connection = null;
            Models.Application application = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    application = new Models.Application
                    {
                        Id = (int)reader["Id"],
                        NameApp = (string)reader["NameApp"],
                        Creation_dt = (string)reader["Creation_dt"],
                        Res_type = (string)reader["Res_type"]
                    };
                }

                reader.Close();
                connection.Close();

                if (application == null)
                {
                    return NotFound();
                }

                return Ok(application);
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return NotFound();
                }
            }

            return null;
        }

        // POST Application
        [Route("api/somiod")]
        public IHttpActionResult PostApplication([FromBody] Models.Application application)
        {
            string sql = "INSERT INTO Applications VALUES(@nameApp, @creation_dt, @res_type)";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nameApp", application.NameApp);
                command.Parameters.AddWithValue("@creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@res_type", application.Res_type);

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
        public IHttpActionResult PutApplication(int id, [FromBody] Models.Application application)
        {
            string sql = "UPDATE Applications SET NameApp=@nameApp, Res_type=@res_type WHERE Id=@id";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nameApp", application.NameApp);
                command.Parameters.AddWithValue("@res_type", application.Res_type);
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
        public IEnumerable<Module> GetAllModules()
        {
            List<Module> listModules = new List<Module>();
            string sql = "SELECT Id, NameMod, Creation_dt, Parent, Res_type FROM Modules";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Module module = new Module
                    {
                        Id = (int)reader["ID"],
                        NameMod = (string)reader["NameMod"],
                        Creation_dt = (string)reader["Creation_dt"],
                        Parent = (int)reader["Parent"],
                        Res_type = (string)reader["Res_type"]
                    };

                    listModules.Add(module);
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

            return listModules;
        }

        // GET Module By Id
        [Route("api/somiod/{nameApp}/{id}")]
        public IHttpActionResult GetModuleById(int id)
        {
            string sql = "SELECT * FROM Modules WHERE Id=@id";
            string sqlData = "SELECT id, content, creation_dt, parent, res_type FROM Datas WHERE Parent=@id";
            
            SqlConnection connection = null;
            
            Module module = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand commandData = new SqlCommand(sqlData, connection);
                commandData.Parameters.AddWithValue("@id", id);
                SqlDataReader readerData = commandData.ExecuteReader();

                List<DataSub> listData = new List<DataSub>();

                while (readerData.Read())
                {
                    DataSub dataSub = new DataSub
                    {
                        Id = (int)readerData["Id"],
                        Content = (string)readerData["Content"],
                        Creation_dt = (string)readerData["Creation_dt"],
                        Parent = (int)readerData["Parent"],
                        Res_type = (string)readerData["Res_type"]
                    };

                    listData.Add(dataSub); 
                }

                readerData.Close();
                connection.Close();

                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    module = new Module
                    {
                        Id = (int)reader["Id"],
                        NameMod = (string)reader["NameMod"],
                        Creation_dt = (string)reader["Creation_dt"],
                        Parent = (int)reader["Parent"],
                        Res_type = (string)reader["Res_type"],
                        Data = listData
                    };
                }

                reader.Close();
                connection.Close();

                if (module == null)
                {
                    return NotFound();
                }

                return Ok(module);
            }
            catch (Exception)
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    return NotFound();
                }
            }

            return null;
        }

        // POST Module
        [Route("api/somiod/{nameApp}")]
        public IHttpActionResult PostModule(string nameApp, [FromBody] Module module)
        {
            int parent = GetApplicationId(nameApp);

            if (parent == -1)
            {
                return InternalServerError();
            }

            string sql = "INSERT INTO Modules VALUES(@nameMod, @creation_dt, @parent, @res_type)";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("nameMod", module.NameMod);
                command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("parent", parent);
                command.Parameters.AddWithValue("res_type", module.Res_type);

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
        public IHttpActionResult PutModule(int id, [FromBody] Module module)
        {
            string sql = "UPDATE Modules SET NameMod=@nameMod, Parent=@parent ,Res_type=@res_type WHERE Id=@id";
            SqlConnection connection = null;

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nameMod", module.NameMod);
                command.Parameters.AddWithValue("@parent", module.Parent);
                command.Parameters.AddWithValue("@res_type", module.Res_type);
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
        public IHttpActionResult PostDataSub(string nameMod, [FromBody] DataSub dataSub)
        {
            int parent = GetModuleId(nameMod);

            if (parent == -1)
            {
                return InternalServerError();
            }

            SqlConnection connection = null;

            if (dataSub.Res_type == "data")
            {
                string sql = "INSERT INTO Datas VALUES(@content, @creation_dt, @parent, @res_type)";

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("content", dataSub.Content);
                    command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("parent", parent);
                    command.Parameters.AddWithValue("res_type", dataSub.Res_type);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        string content = dataSub.Content;
                        string eventData = "creation";
                        string endPoint = GetEndPoint(parent);

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
            else if (dataSub.Res_type == "subscription")
            {
                string sql = "INSERT INTO Subscriptions VALUES(@nameSub, @creation_dt, @parent, @event, @endpoint, @res_type)";

                try
                {
                    connection = new SqlConnection(connectionString);
                    connection.Open();

                    SqlCommand command = new SqlCommand(sql, connection);
                    command.Parameters.AddWithValue("nameSub", dataSub.NameSub);
                    command.Parameters.AddWithValue("creation_dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    command.Parameters.AddWithValue("parent", parent);
                    command.Parameters.AddWithValue("event", dataSub.Event);
                    command.Parameters.AddWithValue("endpoint", dataSub.EndPoint);
                    command.Parameters.AddWithValue("res_type", dataSub.Res_type);

                    int numRegistos = command.ExecuteNonQuery();

                    connection.Close();

                    if (numRegistos > 0)
                    {
                        string eventData = dataSub.Event;
                        string endPoint = dataSub.EndPoint;

                        subChannel(nameMod, eventData, endPoint);
                        
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

        // DELETE Data and Subscription
        [Route("api/somiod/{nameApp}/{nameMod}/{id}/{res_type}")]
        public IHttpActionResult DeleteDataSub(int id, string res_type)
        {
            string eventData = "deletion";
            int parent = GetParent(id);

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
                        string endPoint = GetEndPoint(parent);
                        string content = null;
                        string nameMod = GetModuleName(parent);

                        //sendNotification(nameMod, eventData, endPoint, content);

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

        private string GetEndPoint(int parent)
        {
            string endPoint = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT EndPoint FROM Subscriptions WHERE parent = @parent", connection))
                {
                    command.Parameters.AddWithValue("@parent", parent);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            endPoint = (string)reader["EndPoint"];
                        }
                    }

                    connection.Close();
                }
            }

            return endPoint;
        }

        private string GetModuleName(int parent)
        {
            string name = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT nameMod FROM Modules WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", parent);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            name = (string)reader["NameMod"];
                        }
                    }

                    connection.Close();
                }
            }

            return name;
        }

        private int GetParent(int id)
        {
            int parent = -1;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT parent FROM Datas WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            parent = (int)reader["Parent"];
                        }
                    }

                    connection.Close();
                }
            }

            return parent;
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
            mqttClient  = new MqttClient(IPAddress.Parse(endPoint));
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