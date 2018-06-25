///<reference path="knockout.ts"/>
///<reference path="../lib/linq4js/dist/linq4js.d.ts"/>
$(function () {
    var vm = new PartOne.PartOneViewModel(azureSearchUri, azureAPIKey);
    ko.applyBindings(vm, document.getElementById('partOne'));
});
var PartOne;
(function (PartOne) {
    var PartOneViewModel = /** @class */ (function () {
        function PartOneViewModel(uri, apiKey) {
            var _this = this;
            this.uri = uri;
            this.apiKey = apiKey;
            this.QueryFields = [];
            this.QueryItems = ko.observableArray();
            this.PubDateSelectOptions = [];
            this.SelectedPubDateOption = ko.observable(DateCompareTypes.GreaterThan);
            this.PubDateFilter1 = ko.observable();
            this.PubDateFilter2 = ko.observable();
            this.NameSortBy = ko.observable(SortByTypes.Ascending);
            this.PubDateSortBy = ko.observable(SortByTypes.None);
            this.Results = ko.observableArray();
            this.QueryFields.push(new QueryField("All", []));
            this.QueryFields.push(new QueryField("Name", [{ Name: "Name", IsCollection: false }]));
            this.QueryFields.push(new QueryField("Description", [{ Name: "Description", IsCollection: false }]));
            this.QueryFields.push(new QueryField("Vision", [{ Name: "Vision", IsCollection: false }]));
            this.QueryFields.push(new QueryField("Mission", [{ Name: "Mission", IsCollection: false }]));
            this.QueryFields.push(new QueryField("Values", [{ Name: "valueDescriptionCollection", IsCollection: true }, { Name: "valueNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Value Name", [{ Name: "valueNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Value Description", [{ Name: "valueDescriptionCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Goals", [{ Name: "goalOtherInformationCollection", IsCollection: true }, { Name: "goalDescriptionCollection", IsCollection: true }, { Name: "goalNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Goal Name", [{ Name: "goalNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Goal Description", [{ Name: "goalDescriptionCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Goal Other Information", [{ Name: "goalOtherInformationCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Objectives", [{ Name: "objectiveOtherInformationCollection", IsCollection: true }, { Name: "objectiveDescriptionCollection", IsCollection: true }, { Name: "objectiveNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Objective Name", [{ Name: "objectiveNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Objective Description", [{ Name: "objectiveDescriptionCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Objective Other Information", [{ Name: "objectiveOtherInformationCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Other Information", [{ Name: "Other Information", IsCollection: false }]));
            this.QueryFields.push(new QueryField("Organizations", [{ Name: "organizationDescriptionCollection", IsCollection: true }, { Name: "organizationAcronymCollection", IsCollection: true }, { Name: "organizationNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Organization Name", [{ Name: "organizationNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Organization Description", [{ Name: "organizationDescriptionCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Organization Acronym", [{ Name: "organizationAcronymCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Stakeholders", [{ Name: "stakeholderDescriptionCollection", IsCollection: true }, { Name: "stakeholderNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Stakeholder Name", [{ Name: "stakeholderNameCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Stakeholder Description", [{ Name: "stakeholderDescriptionCollection", IsCollection: true }]));
            this.QueryFields.push(new QueryField("Submitter", [{ Name: "SubmitterGivenName", IsCollection: false }, { Name: "SubmitterSurname", IsCollection: false }, { Name: "SubmitterPhoneNumber", IsCollection: false }, { Name: "SubmitterEmailAddress", IsCollection: false }]));
            this.QueryItems.push(new QueryItem(this));
            this.PubDateSelectOptions.push(new DateOptions(">", DateCompareTypes.GreaterThan));
            this.PubDateSelectOptions.push(new DateOptions("Between", DateCompareTypes.Between));
            this.PubDateSelectOptions.push(new DateOptions("<", DateCompareTypes.LessThan));
            this.SelectedPubDateOption(DateCompareTypes.GreaterThan);
            this.IsPubDateFilterBetween = ko.computed(function () { return _this.SelectedPubDateOption() == DateCompareTypes.Between; });
            this.PubCss = ko.computed(function () {
                return "form-group row" + (_this.IsPubDateFilterBetween() && _this.PubDateFilter1() > _this.PubDateFilter2() ? " has-error" : "");
            });
            $("#pubDateFilter1").on("change.dp", function (e) {
                var d = $("#pubDateFilter1");
                _this.PubDateFilter1(d.datepicker('getDate'));
            });
            $("#pubDateFilter2").on("change.dp", function (e) {
                var d = $("#pubDateFilter2");
                _this.PubDateFilter2(d.datepicker('getDate'));
            });
            this.IsNameSorted = ko.computed(function () { return _this.NameSortBy() != SortByTypes.None; });
            this.NameSortedAscClass = ko.computed(function () { return "btn btn-sm" + (_this.NameSortBy() == SortByTypes.Ascending ? " btn-primary" : " btn-info"); });
            this.NameSortedDscClass = ko.computed(function () { return "btn btn-sm" + (_this.NameSortBy() == SortByTypes.Descending ? " btn-primary" : " btn-info"); });
            this.IsPubDateSorted = ko.computed(function () { return _this.PubDateSortBy() != SortByTypes.None; });
            this.PubDateSortedAscClass = ko.computed(function () { return "btn btn-sm" + (_this.PubDateSortBy() == SortByTypes.Ascending ? " btn-primary" : " btn-info"); });
            this.PubDateSortedDscClass = ko.computed(function () { return "btn btn-sm" + (_this.PubDateSortBy() == SortByTypes.Descending ? " btn-primary" : " btn-info"); });
        }
        PartOneViewModel.prototype.ToggleSortByName = function () {
            if (this.NameSortBy() != SortByTypes.Ascending)
                this.NameSortBy(SortByTypes.Ascending);
            else
                this.NameSortBy(SortByTypes.Descending);
            this.Search();
        };
        PartOneViewModel.prototype.RemoveSortByName = function () {
            this.NameSortBy(SortByTypes.None);
            this.Search();
        };
        PartOneViewModel.prototype.ToggleSortByPubDate = function () {
            if (this.PubDateSortBy() != SortByTypes.Ascending)
                this.PubDateSortBy(SortByTypes.Ascending);
            else
                this.PubDateSortBy(SortByTypes.Descending);
            this.Search();
        };
        PartOneViewModel.prototype.RemoveSortByPubDate = function () {
            this.PubDateSortBy(SortByTypes.None);
            this.Search();
        };
        PartOneViewModel.prototype.Search = function () {
            var _this = this;
            var queryStringBuilder = lucene.builder(function (data) {
                var _ = lucene;
                var groups = [];
                var items = _this.QueryItems();
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    var queryText = item.QueryText().trim();
                    if (queryText != "") {
                        var fields = item.SelectedQueryField().Fields;
                        var terms = [];
                        for (var j = 0; j < fields.length; j++) {
                            terms.push("(" + fields[j].Name + ":\"" + queryText + "\")");
                        }
                        var group = "(";
                        for (var j = 0; j < terms.length; j++) {
                            group += terms[j];
                            if (j < terms.length - 1)
                                group += " OR ";
                        }
                        group += ")";
                        groups.push(group);
                    }
                }
                var query = "(";
                for (var j = 0; j < groups.length; j++) {
                    query += groups[j];
                    if (j < groups.length - 1)
                        query += " AND ";
                }
                query += ")";
                //if (this.NameSortBy() != SortByTypes.None || this.PubDateSortBy() != SortByTypes.None) {
                //    query += "&$orderby=";
                //    var orderbys: string[] = [];
                //    if (this.NameSortBy() != SortByTypes.None) {
                //        orderbys.push("Name " + (this.NameSortBy() == SortByTypes.Ascending ? "asc" : "desc"));
                //    }
                //    if (this.PubDateSortBy() != SortByTypes.None) {
                //        orderbys.push("PublicationDate " + (this.PubDateSortBy() == SortByTypes.Ascending ? "asc" : "desc"));
                //    }
                //    for (var j = 0; j < orderbys.length; j++) {
                //        query += orderbys[j];
                //        if (j < orderbys.length - 1)
                //            query += ",";
                //    }
                //}
                return query;
            });
            var queryString = queryStringBuilder();
            $.ajax({
                headers: {
                    "api-key": this.apiKey,
                    "Content-Type": "application/json"
                },
                url: this.uri + "&search=" + queryStringBuilder() + "&searchMode=all&queryType=full&$count=true",
                success: function (data) {
                    var d = data.value;
                    _this.Results.removeAll();
                    for (var i = 0; i < d.length; i++) {
                        _this.Results.push(new QueryResultViewModel(d[i]));
                    }
                }
            });
        };
        return PartOneViewModel;
    }());
    PartOne.PartOneViewModel = PartOneViewModel;
    var QueryResultViewModel = /** @class */ (function () {
        function QueryResultViewModel(result) {
            this.result = result;
            this.Name = result.Name;
            this.Id = result.id;
            if (result.PublicationDate != null)
                this.PublicationDate = new Date(result.PublicationDate);
            this.Url = "http://stratml.services/api/PartOne/" + this.Id;
        }
        return QueryResultViewModel;
    }());
    PartOne.QueryResultViewModel = QueryResultViewModel;
    var DateCompareTypes;
    (function (DateCompareTypes) {
        DateCompareTypes[DateCompareTypes["LessThan"] = 0] = "LessThan";
        DateCompareTypes[DateCompareTypes["GreaterThan"] = 1] = "GreaterThan";
        DateCompareTypes[DateCompareTypes["Between"] = 2] = "Between";
    })(DateCompareTypes = PartOne.DateCompareTypes || (PartOne.DateCompareTypes = {}));
    var DateOptions = /** @class */ (function () {
        function DateOptions(Text, Value) {
            this.Text = Text;
            this.Value = Value;
        }
        return DateOptions;
    }());
    PartOne.DateOptions = DateOptions;
    var SortByTypes;
    (function (SortByTypes) {
        SortByTypes[SortByTypes["None"] = 0] = "None";
        SortByTypes[SortByTypes["Ascending"] = 1] = "Ascending";
        SortByTypes[SortByTypes["Descending"] = 2] = "Descending";
    })(SortByTypes = PartOne.SortByTypes || (PartOne.SortByTypes = {}));
    var QueryItem = /** @class */ (function () {
        function QueryItem(parent) {
            var _this = this;
            this.parent = parent;
            this.SelectedQueryField = ko.observable();
            this.QueryText = ko.observable();
            var me = this;
            this.IsLast = ko.computed(function () { return _this.parent.QueryItems().indexOf(me) == _this.parent.QueryItems().length - 1; });
            this.IsNotOnly = ko.computed(function () { return _this.parent.QueryItems().length > 1; });
        }
        QueryItem.prototype.Delete = function () {
            this.parent.QueryItems.remove(this);
        };
        QueryItem.prototype.Add = function () {
            this.parent.QueryItems.push(new QueryItem(this.parent));
        };
        return QueryItem;
    }());
    PartOne.QueryItem = QueryItem;
    var QueryField = /** @class */ (function () {
        function QueryField(DisplayName, Fields) {
            this.DisplayName = DisplayName;
            this.Fields = Fields;
        }
        return QueryField;
    }());
    PartOne.QueryField = QueryField;
})(PartOne || (PartOne = {}));
//# sourceMappingURL=PartOneViewModel.js.map