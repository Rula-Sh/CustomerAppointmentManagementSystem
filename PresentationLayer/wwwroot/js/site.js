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

    $('body').append('<div id="notification-content" class="d-none"> No Notification Yet</div>')

    function getNotification() {
        var res = "<ul class='list-group'>"
        $.ajax({
            url: "/Notification/Index",
            method: "GET",
            success: function (result) {
                // was Object.keys(result).length  before adding notifications.Count on index return
                if (result.notifscount > 0) {
                    $('#newNotification').removeClass('d-none');
                    if (result.notifscount >= 99) {
                        $('#notificationCount').html(99).show();
                    }
                    if (result.notifscount >= 10) {
                        $('#notificationCount').html(result.notifscount).show();
                        $('#notificationCount').addClass('notifAbove10');
                        $('#notificationCount').removeClass('notifbelow10');
                    } else {
                        $('#notificationCount').html(result.notifscount).show();
                        $('#notificationCount').addClass('notifbelow10');
                        $('#notificationCount').removeClass('notifAbove10');
                    }
                    var notificationsList = result.notifs;
                    console.log(notificationsList);
                    notificationsList.forEach(element => {
                        //console.log(element.id,element.message);
                        res = res + `<li class='list-group-item notification-message' id='${element.id}'>${element.message}</li>`;
                    });
                    res = res + "</ul>";
                    $("#notification-content").html(res);
                } else {
                    $("#notificationCount").html();
                    $("#notificationCount").hide('slow');
                    $("#notification").popover('hide');
                    $('#newNotification').addClass('d-none');
                }
                //console.log(result);
                //console.log(Object.keys(result).length);
            },
            error: function (err) {
                console.log(err);
            }
        })
    }
    $('body').on('click', '.popover .notification-message', function (e) {
        var target=e.target
        const id = $(target).attr('id');
        //was const id = $(this).attr('id');
        console.log("Notification ID:", id);

        readNotification(id, target);
    });

    function readNotification(id, target) {
        $.ajax({
            url: '/Notification/ReadNotification',
            method: 'GET',
            data: { notificationId: id },
            success: function (result) {
                getNotification();
                $(target).fadeOut('slow');
            },
            error: function (err) {
                console.log(err);
            }
        })
    }

    getNotification();

    // adding SignalR
    let connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();
    connection.on("displayNotification", () => {
        getNotification();
    });

    connection.start();

});