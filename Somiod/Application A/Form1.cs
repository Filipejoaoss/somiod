using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application_A
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

        private void Form1_Load(object sender, EventArgs e)
        {
            string xmlApplication = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <NameApp>lighting</NameApp>\r\n  <Res_type>Application</Res_type>\r\n</root>";
            
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlApplication);
                writer.Flush();

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    byte[] payload = Encoding.UTF8.GetBytes(xmlApplication);
                    var request = new RestRequest("api/somiod", Method.POST);
                    request.AddParameter("application/xml", payload, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }

            MessageBox.Show("Application created with success!");

            string xmlModule = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <NameMod>light_bulb</NameMod>\r\n  <Res_type>Module</Res_type>\r\n</root>";
            
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

            string xmlSub = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\r\n<root>\r\n  <NameSub>sub2</NameSub>\r\n  <Event>creation</Event>\r\n  <EndPoint>127.0.0.1</EndPoint>\r\n  <Res_type>subscription</Res_type>\r\n</root>";

            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(xmlSub);
                writer.Flush();

                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    byte[] payload = Encoding.UTF8.GetBytes(xmlSub);
                    var request = new RestRequest("api/somiod/Lighting/light_bulb", Method.POST);
                    request.AddParameter("application/xml", payload, ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);
                }
            }

            MessageBox.Show("Subscription created with success!");

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RestRequest getData = new RestRequest("api/somiod/lighting/light_bulb/data", Method.GET);
            var response = client.Execute(getData);
            if (response != null)
            {
                if (response.Content.Contains("xml(on)"))
                {
                    pictureBox.Image = Properties.Resources.light_bulb_on;
                }
                else
                {
                    pictureBox.Image = Properties.Resources.light_bulb_off;
                }
            }
        }
    }
}
