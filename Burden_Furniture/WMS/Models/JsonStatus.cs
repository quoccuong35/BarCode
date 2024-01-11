using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WMS.Models
{
    public class JsonStatus
    {
       
            public int code { get; set; }
            public string text { get; set; }
            public string description { get; set; }
			public object data { get; set; }
        
    }
}