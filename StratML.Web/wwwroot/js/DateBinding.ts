/// <reference path="knockout.ts" />
interface KnockoutBindingHandlers {
    dateText: KnockoutBindingHandler
    dateValue: KnockoutBindingHandler
}
ko.bindingHandlers.dateText = {
    init: (element, valueAccessor, allBindingsAccessor, viewModel) => {
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
    update: (element, valueAccessor, allBindingsAccessor, viewModel) => {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
    }
};
ko.bindingHandlers.dateValue = {
    init: (element, valueAccessor, allBindingsAccessor, viewModel) => {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
        
    },
    update: (element, valueAccessor, allBindingsAccessor, viewModel) => {
        var val = valueAccessor();
        if (typeof val === "function")
            val(new Date($(element).text()));
    }
};