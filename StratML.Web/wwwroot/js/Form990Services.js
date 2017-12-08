///<reference path="jquery.ts"/>
var Services;
(function (Services) {
    var Form990Service = (function () {
        function Form990Service(serviceLocation) {
            this.serviceLocation = serviceLocation;
        }
        Form990Service.prototype.GetOrganizations = function () {
            return $.ajax({
                type: "GET",
                url: this.serviceLocation + 'IRS990/Organizations',
                dataType: "json",
                headers: {
                    "accept": "application/json"
                }
            });
        };
        Form990Service.prototype.GetData = function (orgId) {
            return $.ajax({
                type: "GET",
                url: this.serviceLocation + 'IRS990/' + orgId,
                dataType: "json",
                headers: {
                    "accept": "application/json"
                }
            });
        };
        return Form990Service;
    }());
    Services.Form990Service = Form990Service;
})(Services || (Services = {}));
