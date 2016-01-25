//$(document).ready(function () {
//    $('#full-page').fullpage({
//        'verticalCentered': false,
//        'css3': true,
//        //'sectionsColor': ['#F0F2F4', '#fff', '#fff', '#fff'],
//        'navigation': true,
//        'navigationPosition': 'right',
//        'navigationTooltips': ['Home', 'Movie', 'How it works', 'About'],
//        anchors: ['firstPage', 'Movie', 'HowItWorks', 'About'],
//        menu: '#menu',
//        scrollingSpeed: 300,

//        'afterLoad': function (anchorLink, index) {
//            if (index > 1) {
//                $('nav').addClass('shrink');
            
//            }
//            if (index == 4) {
//                $('#footer').css({ position: "fixed"});
//               $('#iphone3, #iphone2, #iphone4').addClass('active');
//            }
//            else {
//                $('#footer').css({ position: "inherit" });
//            }
//        },
//        afterRender: function () {
//            //playing the video
//            $('video').get(0).play();
//        },

//        'onLeave': function (index, nextIndex, direction) {
//            if (index == 3 && direction == 'down') {
//                $('.section').eq(index - 1).removeClass('moveDown').addClass('moveUp');
//            }
//            else if (index == 3 && direction == 'up') {
//                $('.section').eq(index - 1).removeClass('moveUp').addClass('moveDown');
//            }
//        }
//    });

//});