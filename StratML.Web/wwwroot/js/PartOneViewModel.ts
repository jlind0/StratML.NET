///<reference path="jquery.ts"/>
///<reference path="../node_modules/@types/knockout/index.d.ts"/>
///<reference path="../node_modules/@types/o.js/index.d.ts"/>
///<reference path="../lib/linq4js/dist/linq4js.d.ts"/>
declare const azureSearchUri: string;
declare const azureAPIKey: string;
$(() => {
    var vm = new PartOne.PartOneViewModel(azureSearchUri, azureAPIKey);
    ko.applyBindings(vm, document.getElementById('partOne'));
});
namespace PartOne {
   

    export class PartOneViewModel {
        public QueryFields: QueryField[] = [];
        public QueryItems: KnockoutObservableArray<QueryItem> = ko.observableArray();
        constructor(protected uri: string, protected apiKey: string) {
            this.QueryFields.push(new QueryField("All", null));
            this.QueryFields.push(new QueryField("Name", ["Name"]));
            this.QueryFields.push(new QueryField("Description", ["Description"]));
            this.QueryFields.push(new QueryField("Vision", ["Vision"]));
            this.QueryFields.push(new QueryField("Mission", ["Mission"]));
            this.QueryFields.push(new QueryField("Values", ["organizationDescriptionCollection", "valueDescriptionCollection", "valueNameCollection"]));
            this.QueryFields.push(new QueryField("Goals", ["goalOtherInformationCollection", "goalDescriptionCollection", "goalNameCollection"]));
            this.QueryFields.push(new QueryField("Objectives", ["objectiveOtherInformationCollection", "objectiveDescriptionCollection", "objectiveNameCollection"]));
            this.QueryFields.push(new QueryField("Other Information", ["Other Information"]));
            this.QueryFields.push(new QueryField("Organizations", ["organizationDescriptionCollection", "organizationAcronymCollection", "organizationNameCollection"]));
            this.QueryFields.push(new QueryField("Stakeholders", ["organizationDescriptionCollection", "stakeholderDescriptionCollection", "stakeholderNameCollection"]));
            this.QueryFields.push(new QueryField("Submitter", ["SubmitterGivenName", "SubmitterSurname", "SubmitterPhoneNumber", "SubmitterEmailAddress"]));
            this.QueryItems.push(new QueryItem(this));
        }
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
    export class QueryField {
        constructor(public DisplayName, public Fields: string[]) {

        }
    }
}