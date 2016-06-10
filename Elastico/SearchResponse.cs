using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elastico
{
    public class SearchResponse
    {
        public IList<Lemma> Lemmas { get; set; }
        public IList<Entry> Entries { get; set; }

        public IList<GetAccessData> GetAccessDatas { get; set; }
        public long TotalHits { get; set; }
        public List<string> Suggestions { get; set; }
    }
}