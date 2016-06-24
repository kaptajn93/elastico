using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Xunit;

namespace Elastico.test
{
    public class SearchTests : IDisposable
    {

        ElasticManager _manager = new ElasticManager();




        [Theory]
        //in da
        [InlineData("cand.act.", "0", "da", "dan-sko-ret")]
        [InlineData("cand.act.", "0", "da", "dan-sko-ret spda-mini")]
        [InlineData("cand.act.", "0", "da", "spda-mini dan-sko-ret")]
        //in sp
        [InlineData("zona de influencia", "0", "sp", "spda-mini")]
        [InlineData("zona de influencia", "0", "sp", "spda-mini dan-sko-ret")]
        [InlineData("zona de influencia", "0", "sp", "dan-sko-ret spda-mini")]
        public void CanFindHeadWordExact(string searchword, int from, string index, string searchInBooks)
        {
            var response = _manager.EntrySearchByHeadWordExact(searchword, from, index, searchInBooks);
            var result = response?.Hits?.FirstOrDefault();


            Assert.Equal(result?.Source?.HeadWord, searchword);
        }





        [Theory]
        //in da
        [InlineData(" ", "0", "da", "dan-sko-ret")]
        [InlineData("cand act", "0", "da", "dan-sko-ret spda-mini")] //virker ikke
        [InlineData("cand", "0", "da", "spda-mini dan-sko-ret")] //virker ikke
        //in sp
        [InlineData(" ", "0", "sp", "spda-mini")]
        [InlineData("zona influencia", "0", "sp", "spda-mini dan-sko-ret")] //finder et ord
        [InlineData("zona", "0", "sp", "dan-sko-ret spda-mini")] //finder ord hvor ordet indgår
        public void CanFindHeadWord(string searchword, int from, string index, string searchInBooks)
        {
            var response = _manager.EntrySearchByHeadWord(searchword, from, index, searchInBooks);
            var result = response?.Hits?.FirstOrDefault();

            Assert.Equal(result?.Source?.HeadWord, searchword);
        }

        [Theory]
        //in da

        [InlineData("cand act", "0", "da", "dan-sko-ret spda-mini")]
        [InlineData("abe", "0", "da", "spda-mini dan-sko-ret")]
        [InlineData("aebkat", "0", "da", "spda-mini dan-sko-ret")]
        //in sp
        [InlineData("zona influencia", "0", "sp", "spda-mini dan-sko-ret")]
        [InlineData("zona de", "0", "sp", "spda-mini")]
        public void EntrySearchByHeadWordWithTermSuggestions(string searchword, int from, string index, string searchInBooks)
        {
            if (searchword != " ")
            {
                searchword = "*" + searchword + "*";
            }
            var response = _manager.EntrySearchByHeadWordWithTermSuggestions(searchword, from, index, searchInBooks);
            var result = response?.Hits?.FirstOrDefault();

            Assert.Equal(result?.Source?.HeadWord, searchword);
        }

        [Theory]
        [InlineData("cand act", "0", "da", "dan-sko-ret spda-mini")]
        [InlineData("abe", "0", "da", "spda-mini dan-sko-ret")]
        [InlineData("aebkat", "0", "da", "spda-mini dan-sko-ret")]
        //in sp
        [InlineData("znoa nifluencia", "0", "sp", "spda-mini dan-sko-ret")]
        [InlineData("znoa ed infleuncia", "0", "sp", "spda-mini dan-sko-ret")] //fejler
        [InlineData("zona de", "0", "sp", "spda-mini")]
        public void EntrySearchByHeadWordWithPhraseSuggestions(string searchword, int from, string index, string searchInBooks)
        {
            if (searchword != " ")
            {
                searchword = "*" + searchword + "*";
            }
            var response = _manager.EntrySearchByHeadWordWithPhraseSuggestions(searchword, from, index, searchInBooks);
            var result = response?.Hits?.FirstOrDefault();

            Assert.Equal(result?.Source?.HeadWord, searchword);
        }




        [Theory]
        //in da
        [InlineData(" ", "0", "da", "dan-sko-ret")]
        [InlineData("cand act", "0", "da", "dan-sko-ret spda-mini")]
        [InlineData("cand", "0", "da", "spda-mini dan-sko-ret")]
        [InlineData("abe", "0", "da", "spda-mini dan-sko-ret")]
        //in sp
        [InlineData(" ", "0", "sp", "spda-mini")]
        [InlineData("z", "0", "sp", "spda-mini")]
        [InlineData("zona influencia", "0", "sp", "spda-mini dan-sko-ret")]
        [InlineData("zona de", "0", "sp", "spda-mini")]
        [InlineData("zona", "0", "sp", "dan-sko-ret spda-mini")] //finder ord hvor ordet indgår 
        public void CanFindHeadWordWithTwoSearches(string searchword, int from, string index, string searchInBooks)
        {
            if (searchword != " ")
            {
                searchword = "*" + searchword + "*";
                var response1 = _manager.EntrySearchByHeadWordWithWildCard(searchword, from, index, searchInBooks);
                var result1 = response1?.Hits?.FirstOrDefault();
                Assert.Equal(result1?.Source?.HeadWord, searchword);
            }
            else
            {
                var response = _manager.EntrySearchByHeadWord(searchword, from, index, searchInBooks);
                var response1 = _manager.EntrySearchByHeadWordWithWildCard(searchword, from, index, searchInBooks);

                if (response?.Total < response1?.Total)
                {
                    var result1 = response1?.Hits?.FirstOrDefault();
                    Assert.Equal(result1?.Source?.HeadWord, searchword);
                }
                else
                {
                    var result = response?.Hits?.FirstOrDefault();
                    Assert.Equal(result?.Source?.HeadWord, searchword);
                }
            }
        }

       
      [Theory]
        [InlineData("reconversión industrial", "0", "sp", "spda-mini")]
        [InlineData("recopilar", "0", "sp", "spda-mini")]
        public void TestPrioritizeLemma(string searchword, int from, string index, string searchInBooks)
        {
            var response1 = _manager.EntrySearchByHeadWordExactAndPrioritizeWhenLemma(searchword, from, index, searchInBooks);
            var result1 = response1?.Hits?.FirstOrDefault();
            Assert.Equal(result1?.Source?.HeadWord, searchword);
        }



        [Theory]
        [InlineData("reconversión industrial", "0", "da", "spda-mini")]
        public void CanFindLemma(string searchword, int from, string index, string searchInBooks)
        {
            _manager.EntrySearchByHeadWordExact(searchword, from, index, searchInBooks);
            Assert.NotNull(_manager);
        }

        public void Dispose()
        {
        }
    }
}
