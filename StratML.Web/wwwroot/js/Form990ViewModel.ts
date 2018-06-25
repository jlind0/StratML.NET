
///<reference path="knockout.ts"/>
///<reference path="Form990Services.ts"/>
///<reference path="accounting.ts"/>
///<reference path="../lib/linq4js/dist/linq4js.d.ts"/>
type NameId = Services.NameId;
type DataElement = Services.IRSDollarPoints;
type DollarPoint = Services.DollarPoint;
type Form990Service = Services.Form990Service;
declare const serviceLocation: string;
$(() => {
    var vm = new ViewModels.Form990VM();
    ko.applyBindings(vm, document.getElementById('form990'));
});
export namespace ViewModels {
    
    export class Form990VM {
        private Service: Form990Service;
        constructor() {
            this.SelectedId.subscribe(val => this.GetData());
            this.Service = new Services.Form990Service(serviceLocation);
            //this.GetData();
            this.LoadOrganizations();
        }
        Organizations: KnockoutObservableArray<NameId> = ko.observableArray<NameId>([{ id: "", name: " All" }]);
        SelectedId: KnockoutObservable<string> = ko.observable(null);
        Assets: KnockoutObservableArray<IFlattenedDataPoint> = ko.observableArray<IFlattenedDataPoint>();
        Income: KnockoutObservableArray<IFlattenedDataPoint> = ko.observableArray<IFlattenedDataPoint>();
        Revenue: KnockoutObservableArray<IFlattenedDataPoint> = ko.observableArray<IFlattenedDataPoint>();
        public GetData(): void {
            this.Service.GetData(this.SelectedId()).done(data => {
                this.Assets.removeAll();
                this.Income.removeAll();
                this.Revenue.removeAll();
                for (var i = 0; i < data.length; i++) {
                    var assets = data[i].assets.OrderByDescending(d => d.asOfDate);
                    assets.forEach(val => this.Assets.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Asset
                    }));
                    var income = data[i].income.OrderByDescending(d => d.asOfDate);
                    income.forEach(val => this.Income.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Income
                    }));
                    var revenue = data[i].revenue.OrderByDescending(d => d.asOfDate);
                    revenue.forEach(val => this.Revenue.push({
                        OrgId: data[i].orgId,
                        Name: data[i].name,
                        Amount: accounting.formatMoney(val.amount),
                        AsOfDate: val.asOfDate,
                        Type: DataPointType.Revenue
                    }));

                    this.CalculauteIncomeOverAssets();
                }
            });
        }
        public IncomeOverAssets: KnockoutObservableArray<IFlattenedDataPoint> = ko.observableArray<IFlattenedDataPoint>();
        public CalculauteIncomeOverAssets(): void {
            this.IncomeOverAssets.removeAll();
            this.Assets().forEach(a => {
                var income = this.Income().Where(i => i.OrgId == a.OrgId && i.AsOfDate == a.AsOfDate).FirstOrDefault();
                if (income != null) {
                    var assets = accounting.unformat(a.Amount);
                    var i = accounting.unformat(income.Amount);
                    if (assets > 0)
                        this.IncomeOverAssets.push({
                            OrgId: income.OrgId,
                            Name: income.Name,
                            Type: DataPointType.IncomeOverAsset,
                            AsOfDate: income.AsOfDate,
                            Amount: accounting.formatNumber(i / assets, 4)
                        })
                }
            });
        }
        public LoadOrganizations(): void {

            this.Service.GetOrganizations().done(orgs => {
                this.Organizations.removeAll();
                this.Organizations.push({ id: "", name: "All" });
                for (var i = 0; i < orgs.length; i++)
                    this.Organizations.push(orgs[i]);
            });
        }
    }
    export enum DataPointType {
        Asset,
        Income,
        Revenue,
        IncomeOverAsset
    }
    export interface IFlattenedDataPoint {
        OrgId: string,
        Name: string,
        Amount?: string,
        AsOfDate?: string,
        Type: DataPointType
    }
}