﻿$(function () {
    // clear mobile footer
    $(".page-footer").remove();

    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;
    var userName = "";

    function addMessage(content, type) {
        var e = $('<li/>').html(content).appendTo($('#discussion'));
        //refreshMessages();

        if (type) {
            e.addClass(type);
        }
        //updateUnread();
        e[0].scrollIntoView();

        return e;
    }

    function initUserName(name) {
        userName = name;

        // Get the user name and store it to prepend to messages.
        $('#displayname').val(userName);

        $('#title').append('<h6>当前昵称: <a style="color: #0000cd;font-weight: bold;">' + userName + '</a></h6>');

        Materialize.toast('随机分配匿名昵称: ' + userName, 3000);

        chat.server.send('system', '欢迎“' + userName + '”加入群聊.');
    }

    function updateCookie() {
        $.cookie('QQmgs-chat-userid', chat.state.id, { path: '/', expires: 30 });
    }

    chat.client.addUser = function (user, exists) {

        // DEBUG
        //$('#title').append('<h6>id: ' + user.Id + '</h6>');
        //$('#title').append('<h6>name: ' + user.Name + '</h6>');
        //$('#title').append('<h6>hash: ' + user.Hash + '</h6>');
        //$('#title').append('<h6>state name: ' + this.state.name + '</h6>');

        initUserName(user.Name);

        //var id = 'u-' + user.Name;
        //if (document.getElementById(id)) {
        //    return;
        //}

        //var data = {
        //    name: user.Name,
        //    hash: user.Hash
        //};

        //var e = $('#new-user-template').tmpl(data)
        //                               .appendTo($('#users'));
        //refreshUsers();

        //if (!exists && this.state.name != user.Name) {
        //    addMessage(user.Name + ' just entered ' + this.state.room, 'notification');
        //    e.hide().fadeIn('slow');
        //}

        updateCookie();
    };

    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {

        // Get current time
        var currentdate = new Date().toLocaleTimeString('en-US',
        {
            hour12: false,
            hour: "numeric",
            minute: "numeric"
        });

        if (name === 'system') {
            var chattingNotify = currentdate + '  ' + htmlEncode(message);

            addMessage(chattingNotify, 'notification');
        } else {
            var chattingMsg = currentdate + '  <a style="font-weight: bold; color: black">' + htmlEncode(name) + '</a>: ' + htmlEncode(message);

            addMessage(chattingMsg, 'msg');
        }

    };

    // Set initial focus to message input box.
    $('#message').focus();

    // Start the connection.
    $.connection.hub.start({ transport: window.activeTransport },
        function () {
            chat.server.join()
                .done(function (success) {

                    // DEBUG
                    // for (var i = 0; i < 3; ++i) chat.server.send('Dava', 'Try ' + i);

                    if (success === false) {
                        $.cookie('QQmgs-chat-userid', '');
                    }

                    $('#sendmessage')
                        .click(function () {
                            chat.server.send(userName, $('#message').val());
                            $('#message').val('').focus();
                        });

                });
        });

    function htmlEncode(value) {
        var encodedValue = $('<div />').text(value).html();
        return encodedValue;
    }

    $("#message").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#sendmessage").click();
        }
    });
})
