﻿define(['ko'], function (ko) {

    function ComponentResolver() {
        this.components = ko.observableArray();
    };

    ko.components.register('missing', { require: 'app/components/missing/viewModel' });

    ko.components.register('one-component', { require: 'app/components/one-component/viewModel' });

    ComponentResolver.prototype = {
        register: function (name, location) {
            var self = this;

            require(['app/components/' + name + '/config'], function(Config) {
                ko.components.register(name, { require: 'app/components/' + name + '/viewModel' });

                self.components.push({
                    location: location,
                    name: name,
                    Config: Config
                });
            });
        },

        resolve: function (location) {
            var self = this;

            return ko.computed(function() {
                return self.components().filter(function(component) {
                    return component.location === location;
                });
            });
        },
    };

    return ComponentResolver;
});