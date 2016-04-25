angular.module('indexApp')
    .factory('alertFactory', ['$window', '$compile', '$http', '$q', '$resource', function ($window, $compile, $http, $q, $resource) {
        var alertFactory = {};



        alertFactory.popupNotification = function (options) {
            //Bootstrap modal settings
            var modalSettings = {
                backdrop: 'static', //set to 'static' to dismiss close modal by clicking on overlay background
                keyboard: true,
                show: true,
                remote: false
            };
            //Default popup content
            var popupSettings = {
                popupType: 'alert',  //confirm, alert, open
                popupStatus: '', //success, info, warning, danger
                title: '',
                message: '',
                htmlTemplate: '', //insert html template
                iframe: '',
                callback: ''
            };

            if (options && Object.keys(options).length > 0) {
                options = angular.fromJson(options);

                angular.extend(popupSettings, options);

                var popupType = popupSettings.popupType,
                    popupStatus = popupSettings.popupStatus,
                    title = popupSettings.title,
                    message = popupSettings.message,
                    htmlTemplate = popupSettings.htmlTemplate,
                    iframe = popupSettings.iframe;

                if (!popupType || popupType.length === 0)
                    popupType = 2;

                var template = '';

                switch (popupType) {
                    case 'confirm':
                        template = '<div class="modal fade" id="alertModal" tabindex="-1" role="dialog" aria-labelledby="alertModalLabel" aria-hidden="true">'
                                  + '<div class="modal-dialog">'
                                    + '<div class="modal-content">'
                                      + '<div class="modal-header alert-' + popupStatus + '">'
                                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
                                        + '<h4 class="modal-title" id="alertModalLabel">' + title + '</h4>'
                                      + '</div>'
                                      + '<div class="modal-body">'
                                             + '<p style="display:inline;"> ' + message + '</p>'
                                      + '</div>'
                                      + '<div class="modal-footer">'
                                        + '<button type="button" ng-click="' + popupSettings.callback + '()" class="btn btn-' + popupStatus + '">OK</button>'
                                        + '<button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>'
                                      + '</div>'
                                    + '</div>'
                                  + '</div>'
                                + '</div>';
                        console.log("template: confirm");
                        break;

                    case 'alert':
                        template = '<div class="modal fade" id="alertModal" tabindex="-1" role="dialog" aria-labelledby="alertModalLabel" aria-hidden="true">'
                                  + '<div class="modal-dialog">'
                                    + '<div class="modal-content">'
                                      + '<div class="modal-header alert-' + popupStatus + '">'
                                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
                                        + '<h4 class="modal-title" id="alertModalLabel">' + title + '</h4>'
                                      + '</div>'
                                      + '<div class="modal-body">'
                                             + '<p style="display:inline;"> ' + message + '</p>'
                                             + '<div class="overflow">' + htmlTemplate + ' </div>'
                                      + '</div>'
                                      + '<div class="modal-footer">'
                                        + '<button type="button" class="btn btn-' + popupStatus + '" data-dismiss="modal">OK</button>'
                                      + '</div>'
                                    + '</div>'
                                  + '</div>'
                                + '</div>';
                        console.log("template: alert");
                        break;

                    case 'open':
                        template = '<div class="modal fade" id="alertModal" tabindex="-1" role="dialog" aria-labelledby="alertModalLabel" aria-hidden="true">'
                                  + '<div class="modal-dialog">'
                                    + '<div class="modal-content">'
                                      + '<div class="modal-header alert-' + popupStatus + '">'
                                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
                                        + '<h4 class="modal-title" id="alertModalLabel">' + title + '</h4>'
                                      + '</div>'
                                      + '<div class="modal-body">'
                                            + '<p style="display:inline;"> ' + message + '</p>'
                                             + '<div class="overflow">' + htmlTemplate + ' </div>'
                                             + '<div class="overflow"><iframe src=' + iframe + ' style="width:100%"></iframe></div>'
                                      + '</div>'
                                    + '</div>'
                                  + '</div>'
                                + '</div>';
                        console.log("template: open");
                        break;

                    default:
                        template = '<div class="modal fade" id="alertModal" tabindex="-1" role="dialog" aria-labelledby="alertModalLabel" aria-hidden="true">'
                                  + '<div class="modal-dialog">'
                                    + '<div class="modal-content">'
                                      + '<div class="modal-header alert-' + popupStatus + '">'
                                        + '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>'
                                        + '<h4 class="modal-title" id="alertModalLabel">Modal title</h4>'
                                      + '</div>'
                                      + '<div class="modal-body">'
                                             + message
                                      + '</div>'
                                      + '<div class="modal-footer">'
                                        + '<button type="button" class="btn btn-' + popupStatus + '">OK</button>'
                                        + '<button type="button" class="btn btn-default" data-dismiss="modal">Cancel</button>'
                                      + '</div>'
                                    + '</div>'
                                  + '</div>'
                                + '</div>';
                        break;


                }
                var $body = angular.element(document.querySelector('body'));
                $body.find('#alertModal').remove();
                $body.append(template);
                $compile(template)($body.scope());
            }//end If
            $('#alertModal').modal(modalSettings);
        }

        return alertFactory;
    }])