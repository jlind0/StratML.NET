///<reference path="jquery.ts"/>

namespace Services {
        export class Form990Service {
            constructor(public serviceLocation: string) { }
            GetOrganizations(): JQuery.jqXHR<NameId[]> {
                return <JQuery.jqXHR<NameId[]>>$.ajax({
                    type: "GET",
                    url: this.serviceLocation + 'IRS990/Organizations',
                    dataType: "json",
                    headers: {
                        "accept": "application/json"
                    }
                });
            }
            GetData(orgId?: string): JQuery.jqXHR<IRSDollarPoints[]> {
                return <JQuery.jqXHR<IRSDollarPoints[]>>$.ajax({
                    type: "GET",
                    url: this.serviceLocation + 'IRS990/',
                    dataType: "json",
                    headers: {
                        "accept": "application/json"
                    }
                });
            }

        }
        export interface NameId {
            id: string,
            name: string
        }
        export interface IRSDollarPoints {
            orgId: string,
            name: string,
            assets: DollarPoint[],
            income: DollarPoint[],
            revenue: DollarPoint[]
        }
        export interface DollarPoint {
            asOfDate?: Date,
            amount: number
        }
}