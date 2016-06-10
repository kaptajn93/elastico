using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Nest;
using WebGrease.Css.Extensions;
using System.IO;
using Newtonsoft.Json;
using System.Web.Http.Results;
using System.Web.Helpers;
using Newtonsoft.Json.Linq;

namespace Elastico.Controllers
{
    public class SearchController : ApiController
    {

        public IHttpActionResult Post([FromBody]string data)
        {
            string[] values = data.Split(',');
            var searchword = values[0];
            var from = int.Parse(values[1]);
            var index = values[2];
            var searchInBooks = values[3];

            var _elastic = new ElasticManager();
            var lemmas = new List<Lemma>();
            var entries = new List<Entry>();
            var accessFromLemma = new List<GetAccessData>();
            var lemmaIds = new List<string>();
            var totalHits = new long();
            var suggestions = new List<string>();

            #region ---First Search---
            var lemmaResponse = _elastic.LemmaSearchByFullFormAndOrthography(searchword, from, index);
            lemmaResponse.Hits?.ForEach(x => lemmas.Add(x.Source));
            lemmas.ForEach(x => lemmaIds.Add(x.LemmaId));
            //get entries for first search
            if (lemmaResponse.Hits.Any())
            {
                var entryResponse = _elastic.EnrtrySearchInEntryByLemmaId(lemmaIds, index, searchInBooks);
                entryResponse.Hits?.ForEach(y => entries.Add(y.Source));

                totalHits = lemmaResponse.Total;
            }
            #endregion

            #region ---Second and third Search---

            else
            {
                var searchHeadWordExact = _elastic.EntrySearchByHeadWordExact(searchword, from, index, searchInBooks);
                searchHeadWordExact.Hits?.ForEach(x => entries.Add(x.Source));
                searchHeadWordExact.Hits?.ForEach(x => lemmaIds.Add(x.Source.EntryIdLemma.LemmaIdRef));

                if (!searchHeadWordExact.Hits.Any())
                {
                    //third search
                    var searchwordWild = "*" + searchword + "*";
                    //make 2 search - phrase and wildcard
                    var headWordWithPhrase = _elastic.EntrySearchByHeadWord(searchword, from, index, searchInBooks);
                    var headWordWithWildcard = _elastic.EntrySearchByHeadWordWithWildCard(searchwordWild, from, index,
                        searchInBooks);

                    //most hits win
                    if (headWordWithPhrase?.Total < headWordWithWildcard?.Total)
                    {
                        headWordWithWildcard?.Hits?.ForEach(x => entries.Add(x.Source));
                        headWordWithWildcard?.Hits?.ForEach(x => lemmaIds.Add(x.Source.EntryIdLemma.LemmaIdRef));

                        var first = headWordWithWildcard?.Suggest?.FirstOrDefault();
                        if (first != null)
                        {
                            var options = first.Value.Value.SelectMany(x => x.Options);
                            var suggestions2 = options?.Select(x => x.Text);

                            suggestions.AddRange(suggestions2);
                        }
                    }
                    else
                    {
                        headWordWithPhrase?.Hits?.ForEach(x => entries.Add(x.Source));
                        headWordWithPhrase?.Hits?.ForEach(x => lemmaIds.Add(x.Source.EntryIdLemma.LemmaIdRef));

                        var first = headWordWithPhrase?.Suggest?.FirstOrDefault();
                        if (first != null)
                        {
                            var options = first.Value.Value.SelectMany(x => x.Options);
                            var suggestions2 = options?.Select(x => x.Text);

                            suggestions.AddRange(suggestions2);
                        }
                    }
                }
                //getting lemmas from Entries:
                var getLemmasFromEntries = _elastic.EnrtrySearchInLemmaByIdFromEntry(lemmaIds, index);
                getLemmasFromEntries?.Hits?.ForEach(f => lemmas.Add(f.Source));

                if (getLemmasFromEntries != null)
                    totalHits = getLemmasFromEntries.Total;
            }
            #endregion

            #region ----getting info from accessoryData----
            if (lemmas.Any())
            {
                foreach (var lemma in lemmas)
                {
                    var lemmaFromAccess = new GetAccessData
                    {
                        WordOtrhography = lemma.LemmaOrtography,
                        SynonymsTo = new List<string>(),
                        StartsWith = new List<string>(),
                        EndsWith = new List<string>()
                    };
                    foreach (var accessData in lemma.LemmaAccessoryDatas)
                    {
                        switch (accessData.CategoryDan)
                        {
                            case "synonymer til ¤":
                                foreach (var refs in accessData.LemmaAccessDataReferencesRefs)
                                {
                                    lemmaFromAccess.SynonymsTo.Add(refs.LemmaRef + ", " + refs.LemmaPos);
                                }
                                break;
                            case "ord der begynder med ¤":
                                foreach (var refs in accessData.LemmaAccessDataReferencesRefs)
                                {
                                    lemmaFromAccess.StartsWith.Add(refs.LemmaRef + ", " + refs.LemmaPos);
                                }
                                break;
                            case "ord der ender på ¤":
                                foreach (var refs in accessData.LemmaAccessDataReferencesRefs)
                                {
                                    lemmaFromAccess.EndsWith.Add(refs.LemmaRef + ", " + refs.LemmaPos);
                                }
                                break;
                            default:
                                Console.WriteLine("No AccessoryData found to " + lemma.LemmaOrtography);
                                break;
                        }
                    }
                    accessFromLemma.Add(lemmaFromAccess);
                }
            }
            #endregion


            //set search response
            var searchResponse = new SearchResponse
            {
                Entries = entries,
                Lemmas = lemmas,
                GetAccessDatas = accessFromLemma,
                TotalHits = totalHits,
                Suggestions = suggestions

            };

            return Ok(searchResponse);

        }
    }
}

