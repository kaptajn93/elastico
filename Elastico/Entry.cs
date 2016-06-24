using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Elastico
{
    public class Entry
    {
        //id
        public string IdBook { get; set; }
        public string IdEntry { get; set; }
        public EntryIdLemma EntryIdLemma { get; set; }
        // /id
        public bool Unbound { get; set; }
        public IList<PrioritizeWhenLemma> PrioritizeWhenLemma { get; set; }
        //head
        public string HeadWord { get; set; }
        public string HeadWordExact { get; set; }

        public string HeadPosShortNameGyl { get; set; }
        //body
        public string Blob { get; set; }
        public int SenseCount { get; set; }
    }
    public class EntryIdLemma
    {
        public string IdLemmaPos { get; set; }
        public string IdLemmaRef { get; set; }
        public string IdLemmaDescriptionRef { get; set; }
        public string LemmaIdRef { get; set; }
    }
    public class PrioritizeWhenLemma
    {
        public string PrioritizeLemmaPos { get; set; }
        public string PrioritizeLemmaRef { get; set; }
        public string PrioritizeLemmaDescRef { get; set; }
        public string PrioritizeLemmaIfRef { get; set; }
    }
}