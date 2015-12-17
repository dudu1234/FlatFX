
//type: 'alert', 'information', 'error', 'warning', 'notification', 'success'

notyWrapper = {
    generateAlertPlusButton : function generate(container, type, text) {
        
        var n = $(container).noty({
            text: text,
            type: type,
            dismissQueue: true,
            layout: 'topCenter',
            theme: 'defaultTheme',
            maxVisible: 10
        });
    }
}