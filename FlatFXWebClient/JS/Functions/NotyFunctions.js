
//type: 'alert', 'information', 'error', 'warning', 'notification', 'success'

//if (isDemo == 'True')
//    notyWrapper.generateAlertPlusButton($('#ddddd'), 'warning', 'bla bla bla');

//.noty({ text: 'tsts', theme: 'defaultTheme', type: 'alert', template: '<div style="backgroud:blue" class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>' });
//$scope.showNotification = function () {
//    noty.show('Test message', "success")
//};
//$scope.close = function () {
//    noty.closeAll()
//}

notyWrapper = {
    generateAlertPlusButton: function generate(container, type, text) {

        var n = $(container).noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: 'topCenter',
            theme: 'defaultTheme',
            maxVisible: 10,
            animation: {
                open: { height: 'toggle' },
                close: { height: 'toggle' },
                easing: 'swing',
                speed: 0
            }
            //layout: 'top',
            //theme: 'defaultTheme',
            //type: 'alert',
            //text: '',
            //dismissQueue: true,
            //template: '<div class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>',
            //timeout: false,
            //force: false,
            //modal: false,
            //maxVisible: 5,
            //killer: false,
            //closeWith: ['click'],
            //callback: {
            //    onShow: function () {
            //    },
            //    afterShow: function () {
            //    },
            //    onClose: function () {
            //    },
            //    afterClose: function () {
            //    },
            //    onCloseClick: function () {
            //    }
            //},
            //buttons: false
        });
    },
    //
    generateResultMessage: function generate(container, type, text) {
        var n = $(container).noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: 'topCenter',
            theme: 'defaultTheme',
            maxVisible: 10,
            animation: {
                open: { height: 'toggle' },
                close: { height: 'toggle' },
                easing: 'swing',
                speed: 300
            }
            //timeout: 5000
        });
    }
};