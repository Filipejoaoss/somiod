using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SomiodAPI.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string NameApp { get; set; }
        public DateTime Creation_dt { get; set; }
        public string Res_type { get; set; }
    }
}