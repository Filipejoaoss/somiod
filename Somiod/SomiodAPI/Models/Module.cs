using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SomiodAPI.Models
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class Module
    {
        public int Id { get; set; }
        public string NameMod { get; set; }
        public string Creation_dt { get; set; }
        public int Parent { get; set; }
        public string Res_type { get; set; }
        public virtual List<DataSub> Data { get; set; }
    }
}