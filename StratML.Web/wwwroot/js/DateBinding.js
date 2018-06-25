/// <reference path="knockout.ts" />
ko.bindingHandlers.dateText = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        try {
            var jsonDate = ko.utils.unwrapObservable(valueAccessor());
            var value = new Date(jsonDate);
            if (value != null) {
                var strDate = value.toLocaleDateString();
                $(element).text(strDate);
            }
        }
        catch (ex) { }
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
    }
};
ko.bindingHandlers.dateValue = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
    }
};
//# sourceMappingURL=DateBinding.js.map