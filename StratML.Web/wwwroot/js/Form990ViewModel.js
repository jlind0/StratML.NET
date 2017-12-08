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
            this.Organizations = ko.observableArray([{ id: "", name: " All" }]);
            this.SelectedId = ko.observable(null);
            this.Assets = ko.observableArray();
            this.Income = ko.observableArray();
            this.Revenue = ko.observableArray();
            this.IncomeOverAssets = ko.observableArray();
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
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Asset
                    }); });
                    var income = data[i].income.OrderByDescending(function (d) { return d.asOfDate; });
                    income.forEach(function (val) { return _this.Income.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Income
                    }); });
                    var revenue = data[i].revenue.OrderByDescending(function (d) { return d.asOfDate; });
                    revenue.forEach(function (val) { return _this.Revenue.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Revenue
                    }); });
                    _this.CalculauteIncomeOverAssets();
                }
            });
        };
        Form990VM.prototype.CalculauteIncomeOverAssets = function () {
            var _this = this;
            this.IncomeOverAssets.removeAll();
            this.Assets().forEach(function (a) {
                var income = _this.Income().Where(function (i) { return i.OrgId == a.OrgId && i.AsOfDate == a.AsOfDate; }).FirstOrDefault();
                if (income != null) {
                    var assets = accounting.unformat(a.Amount);
                    var i = accounting.unformat(income.Amount);
                    if (assets > 0)
                        _this.IncomeOverAssets.push({
                            OrgId: income.OrgId,
                            Name: income.Name,
                            Type: DataPointType.IncomeOverAsset,
                            AsOfDate: income.AsOfDate,
                            Amount: accounting.formatNumber(i / assets, 4)
                        });
                }
            });
        };
        Form990VM.prototype.LoadOrganizations = function () {
            var _this = this;
            this.Service.GetOrganizations().done(function (orgs) {
                _this.Organizations.removeAll();
                _this.Organizations.push({ id: "", name: "All" });
                for (var i = 0; i < orgs.length; i++)
                    _this.Organizations.push(orgs[i]);
            });
        };
        return Form990VM;
    }());
    ViewModels.Form990VM = Form990VM;
    var DataPointType;
    (function (DataPointType) {
        DataPointType[DataPointType["Asset"] = 0] = "Asset";
        DataPointType[DataPointType["Income"] = 1] = "Income";
        DataPointType[DataPointType["Revenue"] = 2] = "Revenue";
        DataPointType[DataPointType["IncomeOverAsset"] = 3] = "IncomeOverAsset";
    })(DataPointType = ViewModels.DataPointType || (ViewModels.DataPointType = {}));
})(ViewModels = exports.ViewModels || (exports.ViewModels = {}));
