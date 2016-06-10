using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.WebPages;
using Elasticsearch.Net;
using Microsoft.Ajax.Utilities;
using Nest;
using WebGrease.Css.Extensions;

namespace Elastico
{
    public class Constants
    {
        public class AnalyzerNames
        {
            public const string NgramAnalyzer = "ngramAnalyzer";
            public const string Keyword = "keyword";
            public const string Lowercase = "lowercase";
        }

        public class TokenizerNames
        {
            public const string NoWhitespaceNGram = "noWhitespaceNGram";
        }

        public class IndexNames
        {
            public const string Da = "da";
            public const string En = "en";
        }
    }

    public class ElasticManager
    {
        public Uri NodeUri { get; set; }
        public ConnectionSettings Settings { get; set; }
        public ElasticClient Client { get; set; }

        public ElasticManager(Uri node = null)
        {
            NodeUri = node ?? new Uri("http://192.168.99.100:9200");
            Settings = new ConnectionSettings(NodeUri);
            Client = new ElasticClient(Settings);
        }
        //first search - in lemma
        public ISearchResponse<Lemma> LemmaSearchByFullFormAndOrthography(string searchword, int from, string index)
        {

            var response = Client.Search<Lemma>(e => e
                .Index(index)
                .Type("lemmadocument")
                .From(from)
                .Size(10)
                .Query(q => q
                    .Nested(n => n
                        .Path(p => p.LemmaInflection.SearchableParadigms.First().LemmaInflectedForms)
                                .Query(q1 => q1
                                    .Bool(b1 => b1
                                        .Filter(fi => fi
                                            .Term(mp => mp.LemmaInflection.SearchableParadigms.First().LemmaInflectedForms.First().InflectedFormFullForm, searchword)
                                        )
                                    )
                                )
                   )

                   ||

                    q.Bool(b1 => b1
                        .Filter(fi => fi
                            .Term(ma => ma.LemmaOrtography, searchword)
                        )
                    )
                )


            );
            return response;
        }

        //second search - in entry
        public ISearchResponse<Entry> EntrySearchByHeadWordExact(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);

            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .Size(10)
                .Query(q1 => q1
                   .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.Match(ma => ma.Field(fi => fi.HeadWordExact).Query(searchword))
                                &&
                            mu.Term(ma => ma.Unbound, true)
                        )
                   )
                ));
            return response;
        }


        //third search - in entry --- not done !
        public ISearchResponse<Entry> EntrySearchByHeadWord(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);
            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .Size(10)
                .Query(q1 => q1
                    .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.MatchPhrase(ma => ma.Field(fi => fi.HeadWord).Query(searchword).Slop(25))
                         && mu.Term(ma => ma.Unbound, true)
                        )
                    )
                 )
                 .Suggest(ss => ss
                    .Phrase("my-phrase-suggest", t => t
                        .Text(searchword)
                        .Size(3)
                        .Field(f => f.HeadWord)
                        .RealWordErrorLikelihood(0.9)
                        .MaxErrors(2)
                        .GramSize(1)
                        .DirectGenerator(d => d
                            .Field(f => f.HeadWord)
                            .SuggestMode(SuggestMode.Always)
                            .MinWordLength(1)
                        )
                    )
                )
            );

            return response;
        }
        //wildcard on headword
        public ISearchResponse<Entry> EntrySearchByHeadWordWithWildCard(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);
            string[] searchwords = searchword.Split(' ');
            string searchWithWords = string.Join("*", searchwords);
            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .LowercaseExpandedTerms(false)
                .Size(10)
                .Query(q1 => q1
                    .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.Wildcard(ma => ma.HeadWord, searchWithWords)
                            && mu.Term(ma => ma.Unbound, true)
                        )
                    )
                )
             .Suggest(ss => ss
                    .Phrase("my-phrase-suggest", t => t
                        .Text(searchword)
                        .Size(3)
                        .Field(f => f.HeadWord)
                        .RealWordErrorLikelihood(0.9)
                        .MaxErrors(2)
                        .GramSize(1)
                        .DirectGenerator(d => d
                            .Field(f => f.HeadWord)
                            .SuggestMode(SuggestMode.Always)
                            .MinWordLength(1)
                        )
                    )
                )
            );
            return response;
        }
        //with suggestions -- TEST ----
        public ISearchResponse<Entry> EntrySearchByHeadWordWithTermSuggestions(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);
            string[] searchwords = searchword.Split(' ');
            string searchWithWords = string.Join("*", searchwords);
            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .LowercaseExpandedTerms(false)
                .Size(10)
                .Query(q1 => q1
                    .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.Wildcard(ma => ma.HeadWord, searchWithWords)
                            && mu.Term(ma => ma.Unbound, true)
                        )
                    )
                )
                .Suggest(ss => ss
                    .Term("my-term-suggest", t => t
                    .Text(searchword)
                    .Size(1)
                    .Field(f => f.HeadWord)
                    .MinWordLength(1)
                    )
                )
            );
            return response;
        }
        public ISearchResponse<Entry> EntrySearchByHeadWordWithPhraseSuggestions(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);
            string[] searchwords = searchword.Split(' ');
            string searchWithWords = string.Join("*", searchwords);
            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .LowercaseExpandedTerms(false)
                .Size(10)
                .Query(q1 => q1
                    .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.Wildcard(ma => ma.HeadWord, searchWithWords)
                            && mu.Term(ma => ma.Unbound, true)
                        )
                    )
                )
                .Suggest(ss => ss
                    .Phrase("my-phrase-suggest", t => t
                        .Text(searchword)
                        .Size(3)
                        .Field(f => f.HeadWord)
                        .RealWordErrorLikelihood(0.9)
                        .MaxErrors(2)
                        .GramSize(1)
                        .DirectGenerator(d => d
                            .Field(f => f.HeadWord)
                            .SuggestMode(SuggestMode.Always)
                            .MinWordLength(1)
                        )
                    )
                )
            );
            return response;
        }

        public ISearchResponse<Entry> EntrySearchByHeadWordExactAndPrioritizeWhenLemma(string searchword, int from, string index, string searchInBooks)
        {
            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);

            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(from)
                .Size(10)
                .Query(q1 => q1
                   .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(mu =>
                            mu.Match(ma => ma.Field(fi => fi.HeadWordExact).Query(searchword))
                               &&
                            mu.Term(ma => ma.PrioritizeWhenLemma.PrioritizeLemmaIfRef, null)

                        )
                   )
                ));
            return response;
        }

        //--------/TEST-------


            //first results
        public ISearchResponse<Entry> EnrtrySearchInEntryByLemmaId(List<string> lemmaId, string index, string searchInBooks)
        {
            string ids = string.Join(" || ", lemmaId.ToArray());

            string[] booksFromUser = searchInBooks.Split(' ');
            string books = string.Join(" || ", booksFromUser);

            if (ids.IsEmpty())
            {
                return null;
            }
            var response = Client.Search<Entry>(e => e
                .Index(index)
                .Type("entrydocument")
                .From(0)
                .Size(100)
                .Query(q => q
                     .Bool(b => b
                        .Filter(f => f.Match(m => m.Field(fi => fi.IdBook).Query(books)))
                        .Must(m => m
                        .Nested(n => n
                                .Path(p => p.EntryIdLemma)
                                .Query(q1 => q1
                                    .Bool(bo => bo
                                        .Must(mu => mu
                                            .Match(ma => ma.Field(fi => fi.EntryIdLemma.LemmaIdRef).Query(ids))
                                        )
                                    )
                                )
                            )
                       )
                   )
               ));
            return response;
        }

        //get Lemmas from Entry
        public ISearchResponse<Lemma> EnrtrySearchInLemmaByIdFromEntry(List<string> lemmaId, string index)
        {
            string ids = string.Join(" || ", lemmaId.ToArray());

            if (ids.IsEmpty())
            {
                return null;
            }
            var response = Client.Search<Lemma>(e => e
                .Index(index)
                .Type("lemmadocument")
                .From(0)
                .Size(100)
                .Query(q => q
                    .Bool(bo => bo
                        .Must(mu => mu
                            .Match(ma => ma.Field(fi => fi.LemmaId).Query(ids))
                        )
                    )
                ));
            return response;
        }

        //secondary results


        //word connections


        //reverse results


    }
}

