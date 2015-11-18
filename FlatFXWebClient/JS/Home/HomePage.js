$(document).ready(function () {
    $('#full-page').fullpage({
        'verticalCentered': false,
        'css3': true,
        'sectionsColor': ['#F0F2F4', '#fff', '#fff', '#fff'],
        'navigation': true,
        'navigationPosition': 'right',
        'navigationTooltips': ['Home', 'Movie', 'How it works', 'About'],
        anchors: ['firstPage', 'secondPage', '3rdPage', '4thPage'],
        menu: '#menu',
        scrollingSpeed: 1000,

        'afterLoad': function (anchorLink, index) {
            //if (index == 2) {
            //    $('#iphone3, #iphone2, #iphone4').addClass('active');
            //}
        },
        afterRender: function () {
            //playing the video
            $('video').get(0).play();
        },

        'onLeave': function (index, nextIndex, direction) {
            if (index == 3 && direction == 'down') {
                $('.section').eq(index - 1).removeClass('moveDown').addClass('moveUp');
            }
            else if (index == 3 && direction == 'up') {
                $('.section').eq(index - 1).removeClass('moveUp').addClass('moveDown');
            }

            $('#staticImg').toggleClass('active', (index == 2 && direction == 'down') || (index == 4 && direction == 'up'));
            $('#staticImg').toggleClass('moveDown', nextIndex == 4);
            $('#staticImg').toggleClass('moveUp', index == 4 && direction == 'up');
            
        }
    });

});