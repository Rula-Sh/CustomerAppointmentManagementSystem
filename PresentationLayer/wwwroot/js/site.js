// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    $('[data-toggle="popover"]').popover({
        placement: 'bottom',
        content: function () {
            return $("#notification-content").html();
        },
        html: true,
    }); 

    $('body').append('<div id="notification-content" class="hide"> notification-content </div>')

    function getNotification() {
        var res = "<ul class='list-group'>"
        $.ajax({
            url: "/Notification/Index",
            method: "GET",
            success: function (result) {
                if (Object.keys(result).length > 0) {
                    $('#notificationAlert').show();
                    if (Object.keys(result).length >= 99) {
                        $('#notificationCount').html(99).show();
                        $('#notificationCount').addClass('notifAbove10');
                        $('#notificationCount').removeClass('notifbelow10');
                    } else {
                        $('#notificationCount').html(Object.keys(result).length).show();
                        $('#notificationCount').addClass('notifbelow10');
                        $('#notificationCount').removeClass('notifAbove10');
                    }
                    var notificationsList = result;
                    console.log(notificationsList);
                    notificationsList.forEach(element => {
                        console.log(element.message);
                        res = res + `<li class='list-group-item '>${element.message}</li>`;
                    });
                    res = res + "</ul>";
                    $("#notification-content").html(res);
                } else {
                    $("notificationAlert").hide();
                }
                //console.log(result);
                //console.log(Object.keys(result).length);
            },
            error: function (err) {
                console.log(err);
            }
        })
    }

    getNotification();
});