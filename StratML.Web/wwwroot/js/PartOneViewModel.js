///<reference path="jquery.ts"/>
///<reference path="../node_modules/@types/knockout/index.d.ts"/>
///<reference path="../node_modules/@types/o.js/index.d.ts"/>
///<reference path="../lib/linq4js/dist/linq4js.d.ts"/>
$(function () {
    var vm = new PartOne.PartOneViewModel(azureSearchUri, azureAPIKey);
    ko.applyBindings(vm, document.getElementById('partOne'));
});
var PartOne;
(function (PartOne) {
    var PartOneViewModel = /** @class */ (function () {
        function PartOneViewModel(uri, apiKey) {
            this.uri = uri;
            this.apiKey = apiKey;
            this.QueryFields = [];
            this.QueryItems = ko.observableArray();
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
        return PartOneViewModel;
    }());
    PartOne.PartOneViewModel = PartOneViewModel;
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