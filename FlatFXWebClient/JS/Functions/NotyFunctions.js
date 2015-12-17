
//type: 'alert', 'information', 'error', 'warning', 'notification', 'success'

notyWrapper = {
    generateAlertPlusButton: function generate(container, type, text) {

        var n = $(container).noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: 'topCenter',
            theme: 'defaultTheme',
            maxVisible: 10

            //layout: 'top',
            //theme: 'defaultTheme',
            //type: 'alert',
            //text: '',
            //dismissQueue: true,
            //template: '<div class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>',
            //animation: {
            //    open: { height: 'toggle' },
            //    close: { height: 'toggle' },
            //    easing: 'swing',
            //    speed: 500
            //},
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
    }
}