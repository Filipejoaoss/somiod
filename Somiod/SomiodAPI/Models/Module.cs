using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SomiodAPI.Models
{
    public class Module
    {
        public int Id { get; set; }
        public string NameMod { get; set; }
        public DateTime Creation_dt { get; set; }
        public int Parent { get; set; }
        public string Res_type { get; set; }
    }
}