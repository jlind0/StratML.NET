"use strict";
exports.__esModule = true;
$(function () {
    var vm = new ViewModels.Form990VM();
    ko.applyBindings(vm, document.getElementById('form990'));
});
var ViewModels;
(function (ViewModels) {
    var Form990VM = (function () {
        function Form990VM() {
            var _this = this;
            this.Organizations = ko.observableArray([{ id: null, name: " All" }]);
            this.SelectedId = ko.observable(null);
            this.Assets = ko.observableArray();
            this.Income = ko.observableArray();
            this.Revenue = ko.observableArray();
            this.SelectedId.subscribe(function (val) { return _this.GetData(); });
            this.Service = new Services.Form990Service(serviceLocation);
            //this.GetData();
            this.LoadOrganizations();
        }
        Form990VM.prototype.GetData = function () {
            var _this = this;
            this.Service.GetData(this.SelectedId()).done(function (data) {
                _this.Assets.removeAll();
                _this.Income.removeAll();
                _this.Revenue.removeAll();
                for (var i = 0; i < data.length; i++) {
                    var assets = data[i].assets.OrderByDescending(function (d) { return d.asOfDate; });
                    assets.forEach(function (val) { return _this.Assets.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate.toString(),
                        Type: DataPointType.Asset
                    }); });
                    var income = data[i].income.OrderByDescending(function (d) { return d.asOfDate; });
                    income.forEach(function (val) { return _this.Income.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate.toString(),
                        Type: DataPointType.Income
                    }); });
                    var revenue = data[i].revenue.OrderByDescending(function (d) { return d.asOfDate; });
                    revenue.forEach(function (val) { return _this.Revenue.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate.toString(),
                        Type: DataPointType.Revenue
                    }); });
                    //this.Assets().Union(this.Income()).Union(this.Revenue()).GroupBy(
                    //    g => { g.AsOfDate, g.OrgId }).Select(g =>
                    //        g.sel)
                }
            }).fail(function (err, msg) {
                return alert(msg);
            });
        };
        Form990VM.prototype.LoadOrganizations = function () {
            var _this = this;
            this.Service.GetOrganizations().done(function (orgs) {
                _this.Organizations.removeAll();
                _this.Organizations.push({ id: null, name: "All" });
                for (var i = 0; i < orgs.length; i++)
                    _this.Organizations.push(orgs[i]);
            }).fail(function (err, msg) {
                return alert(msg);
            });
            ;
        };
        return Form990VM;
    }());
    ViewModels.Form990VM = Form990VM;
    var DataPointType;
    (function (DataPointType) {
        DataPointType[DataPointType["Asset"] = 0] = "Asset";
        DataPointType[DataPointType["Income"] = 1] = "Income";
        DataPointType[DataPointType["Revenue"] = 2] = "Revenue";
    })(DataPointType = ViewModels.DataPointType || (ViewModels.DataPointType = {}));
})(ViewModels = exports.ViewModels || (exports.ViewModels = {}));
