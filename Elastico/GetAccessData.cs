using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elastico
{
    public class GetAccessData
    {
        public string WordOtrhography { get; set; }
        public List<string> SynonymsTo { get; set; }
        public List<string> StartsWith { get; set; }
        public List<string> EndsWith { get; set; }
    }
}