﻿define(['ko', 'app/events', 'app/dataService', 'text!app/components/percent/template.html'], function (ko, events, dataService, htmlString) {
    
    function Percent(model) {
        var self = this;

        self.percentRemaining = ko.computed(function() {
            return (ko.unwrap(model.percentRemaining) * 100).toFixed(1);
        });
        self.levelCssClass = ko.computed(function () {
            return ko.unwrap(model.isLow) ? "low" : "high";
        });
    };

    return {
        viewModel: Percent,
        template: htmlString
    };
});