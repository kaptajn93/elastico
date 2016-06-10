
function Search() {
    var self = this;
    self.searchWord = ko.observable();
    //self.searchFilter = ko.observable();
    //self.searchIndex = ko.observable();
    self.entries = ko.observableArray(result);
    self.lemmas = ko.observableArray(result);
    self.orthAndAccessData = ko.observableArray(result);
    self.totalHits = ko.observable(result || 0);
    self.indexes = ko.observableArray(['da', 'sp']);
    self.selectedIndex = ko.observable(self.selectedIndex || "Choose Index...");
    self.searchInBooks = ko.observable();
    self.suggestions = ko.observableArray(result);

    var result;
    var from = 0;
    var increaseFrom = false;

    self.anySynonyms = ko.computed(function () {
        var any = ko.utils.arrayFirst(self.orthAndAccessData(),
             function (item) {
                 if (item.SynonymsTo.length) {
                     return true;
                 } else {
                     return false;
                 }
             });
        return any;
    }, this);

    self.anyStartsWith = ko.computed(function () {
        var any = ko.utils.arrayFirst(self.orthAndAccessData(),
            function (item) {
                if (item.StartsWith.length)
                    return true;
                else
                    return false;
            });
        return any;
    });


    self.anyEndsWith = ko.computed(function () {
        var any = ko.utils.arrayFirst(self.orthAndAccessData(),
             function (item) {
                 if (item.EndsWith.length)
                     return true;
                 else
                     return false;
             });
        return any;
    });

    self.searchSuggestion = function (suggestedWord) {

        self.searchWord (suggestedWord);
        self.getLemmas();
    }

    self.showMore = function () {
        increaseFrom = true;
        self.getLemmas();
    }

    self.getLemmas = function() {
        if (self.selectedIndex() == undefined) {
            alert("a man must choose a index to search in !");
        } else {
            var text = self.searchWord();
            if (increaseFrom) {
                from += 10;
                increaseFrom = false;
            } else {
                from = 0;
            }
            setFrom = from.toString();
            data = text + "," + setFrom + "," + self.selectedIndex() + "," + self.searchInBooks();
            //load data into site (from controller)
            $.ajax({
                    method: "POST",
                    url: "/api/search",
                    data: JSON.stringify(data),
                    contentType: "application/json",
                    dataType: "json"
                })
                .done(function(data, textStatus, jqXHR) {
                    console.debug("lemmas result", data.Lemmas);
                    console.debug("entries result", data.Entries);
                    result = data;
                    if (from === 0) {
                        self.lemmas(result.Lemmas);
                        self.entries(result.Entries);
                        self.orthAndAccessData(result.GetAccessDatas);
                        self.totalHits(result.TotalHits);
                        self.suggestions(result.Suggestions);
                       
                    } else {
                        ko.utils.arrayForEach(result.Lemmas,
                            function(item) { self.lemmas.push(item) });
                        ko.utils.arrayForEach(result.Entries,
                            function(item) { self.entries.push(item) });
                        ko.utils.arrayForEach(result.GetAccessDatas,
                            function (item) { self.orthAndAccessData.push(item) });

                    }
                })
                .fail(function(jqXHR, textStatus, errorThrown) {
                    alert("error");
                });
        }
    };



}

var vm = new Search();
ko.applyBindings(vm);