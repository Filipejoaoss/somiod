using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Application_B
{
    public partial class Form1 : Form
    {
        string baseUrl = @"http://localhost:53497/";
        RestClient client = null;

        public Form1()
        {
            InitializeComponent();
            client = new RestClient(baseUrl);
        }

        private void buttonOn_Click(object sender, EventArgs e)
        {
            string xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <Content>xml(on)</Content>\r\n  <Res_type>data</Res_type>\r\n</root>";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlData);
                writer.Flush();

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    byte[] payload = Encoding.UTF8.GetBytes(xmlData);
                    var request = new RestRequest("api/somiod/lighting/light_bulb", Method.POST);
                    request.AddParameter("application/xml", payload, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }
        }

        private void buttonOff_Click(object sender, EventArgs e)
        {
            string xmlData = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <Content>xml(off)</Content>\r\n  <Res_type>data</Res_type>\r\n</root>";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlData);
                writer.Flush();

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    byte[] payload = Encoding.UTF8.GetBytes(xmlData);
                    var request = new RestRequest("api/somiod/lighting/light_bulb", Method.POST);
                    request.AddParameter("application/xml", payload, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bool appExist = false;

            while(appExist == false) 
            {
                RestRequest getApplicationName = new RestRequest("api/somiod", Method.GET);
                var response = client.Execute(getApplicationName);

                if (response.Content.Contains("lighting"))
                {
                    appExist = true;
                }
            }

            string xmlModule = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <NameMod>light_command</NameMod>\r\n  <Res_type>Module</Res_type>\r\n</root>";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlModule);
                writer.Flush();

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    byte[] payload = Encoding.UTF8.GetBytes(xmlModule);
                    var request = new RestRequest("api/somiod/lighting", Method.POST);
                    request.AddParameter("application/xml", payload, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }

            MessageBox.Show("Module created with success!");
        }

        
    }
}
