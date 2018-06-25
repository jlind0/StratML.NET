
///<reference path="knockout.ts"/>
///<reference path="../lib/linq4js/dist/linq4js.d.ts"/>
declare const azureSearchUri: string;
declare const azureAPIKey: string;
declare const lucene: any;
$(() => {
    var vm = new PartOne.PartOneViewModel(azureSearchUri, azureAPIKey);
    ko.applyBindings(vm, document.getElementById('partOne'));
});
namespace PartOne {
   

    export class PartOneViewModel {
        public QueryFields: QueryField[] = [];
        public QueryItems: KnockoutObservableArray<QueryItem> = ko.observableArray();
        public PubDateSelectOptions: DateOptions[] = [];
        public SelectedPubDateOption: KnockoutObservable<DateCompareTypes> = ko.observable(DateCompareTypes.GreaterThan);
        public PubDateFilter1: KnockoutObservable<Date> = ko.observable();
        public PubDateFilter2: KnockoutObservable<Date> = ko.observable();
        public IsPubDateFilterBetween: KnockoutComputed<boolean>;
        public PubCss: KnockoutComputed<string>;
        protected NameSortBy: KnockoutObservable<SortByTypes> = ko.observable(SortByTypes.Ascending);
        public NameSortedAscClass: KnockoutComputed<string>;
        public NameSortedDscClass: KnockoutComputed<string>;
        public IsNameSorted: KnockoutComputed<boolean>;
        protected PubDateSortBy: KnockoutObservable<SortByTypes> = ko.observable(SortByTypes.None);
        public PubDateSortedAscClass: KnockoutComputed<string>;
        public PubDateSortedDscClass: KnockoutComputed<string>;
        public IsPubDateSorted: KnockoutComputed<boolean>;
        public Results: KnockoutObservableArray<QueryResultViewModel> = ko.observableArray();
        constructor(protected uri: string, protected apiKey: string) {
            this.QueryFields.push(new QueryField("All", []));
            this.QueryFields.push(new QueryField("Name", [{ Name: "Name", IsCollection: false}]));
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
            
            this.IsPubDateFilterBetween = ko.computed(() => this.SelectedPubDateOption() == DateCompareTypes.Between);
            this.PubCss = ko.computed(() =>
                "form-group row" + (this.IsPubDateFilterBetween() && this.PubDateFilter1() > this.PubDateFilter2() ? " has-error" : "")
            );
            $("#pubDateFilter1").on("change.dp", e => {
                var d = <any>$("#pubDateFilter1");
                this.PubDateFilter1(d.datepicker('getDate'));
            });
            $("#pubDateFilter2").on("change.dp", e => {
                var d = <any>$("#pubDateFilter2");
                this.PubDateFilter2(d.datepicker('getDate'));
            });
            this.IsNameSorted = ko.computed(() => this.NameSortBy() != SortByTypes.None);
            this.NameSortedAscClass = ko.computed(() => "btn btn-sm" + (this.NameSortBy() == SortByTypes.Ascending ? " btn-primary" : " btn-info"));
            this.NameSortedDscClass = ko.computed(() => "btn btn-sm" + (this.NameSortBy() == SortByTypes.Descending ? " btn-primary" : " btn-info"));
            this.IsPubDateSorted = ko.computed(() => this.PubDateSortBy() != SortByTypes.None);
            this.PubDateSortedAscClass = ko.computed(() => "btn btn-sm" + (this.PubDateSortBy() == SortByTypes.Ascending ? " btn-primary" : " btn-info"));
            this.PubDateSortedDscClass = ko.computed(() => "btn btn-sm" + (this.PubDateSortBy() == SortByTypes.Descending ? " btn-primary" : " btn-info"));
        }
        public ToggleSortByName(): void {
            if (this.NameSortBy() != SortByTypes.Ascending)
                this.NameSortBy(SortByTypes.Ascending);
            else
                this.NameSortBy(SortByTypes.Descending);
            this.Search();
        }
        public RemoveSortByName(): void {
            this.NameSortBy(SortByTypes.None);
            this.Search();
        }
        public ToggleSortByPubDate(): void {
            if (this.PubDateSortBy() != SortByTypes.Ascending)
                this.PubDateSortBy(SortByTypes.Ascending);
            else
                this.PubDateSortBy(SortByTypes.Descending);
            this.Search();
        }
        public RemoveSortByPubDate(): void {
            this.PubDateSortBy(SortByTypes.None);
            this.Search();
        }
        public Search(): void {
            var queryStringBuilder = lucene.builder(data => {
                var _ = lucene;
                var groups: string[] = [];
                var items = this.QueryItems();
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];
                    var queryText = item.QueryText().trim();
                    if (queryText != "") {
                        var fields = item.SelectedQueryField().Fields;
                        var terms: string[] = [];
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
                success: data => {
                    var d = <QueryResult[]>data.value;
                    this.Results.removeAll();
                    for (var i = 0; i < d.length; i++) {
                        this.Results.push(new QueryResultViewModel(d[i]));
                    }
                }
            });
        }
    }
    export class QueryResultViewModel {
        public PublicationDate: Date;
        public Name: string;
        public Id: string;
        public Url: string;
        constructor(protected result: QueryResult) {
            this.Name = result.Name;
            this.Id = result.id;
            if (result.PublicationDate != null)
                this.PublicationDate = new Date(result.PublicationDate);
            this.Url = "http://stratml.services/api/PartOne/" + this.Id;
        }
    }
    export interface QueryResult {
        id: string,
        Name: string,
        PublicationDate: string
    }
    export enum DateCompareTypes {
        LessThan,
        GreaterThan,
        Between
    }
    export class DateOptions {
        constructor(public Text: string, public Value: DateCompareTypes) {

        }
    }
    export enum SortByTypes {
        None,
        Ascending,
        Descending
    }
    export class QueryItem {
        public SelectedQueryField: KnockoutObservable<QueryField> = ko.observable();
        public QueryText: KnockoutObservable<string> = ko.observable();
        public IsLast: KnockoutComputed<boolean>;
        public IsNotOnly: KnockoutComputed<boolean>;
        constructor(protected parent: PartOneViewModel) {
            var me = this;
            this.IsLast = ko.computed(() => this.parent.QueryItems().indexOf(me) == this.parent.QueryItems().length - 1);
            this.IsNotOnly = ko.computed(() => this.parent.QueryItems().length > 1);

        }
        public Delete(): void {
            this.parent.QueryItems.remove(this);
        }
        public Add(): void {
            this.parent.QueryItems.push(new QueryItem(this.parent));
        }
    }
    export interface Field {
        Name: string;
        IsCollection: boolean;
    }
    export class QueryField {
        constructor(public DisplayName, public Fields: Field[]) {

        }
    }
}