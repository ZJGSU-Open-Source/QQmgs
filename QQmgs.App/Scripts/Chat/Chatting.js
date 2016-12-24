$(function () {
    // clear mobile footer
    $("#mobile-footer").remove();

    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;

    function makeUserName() {
        var nameList = [
            "钱江湾", "金沙港", "金字塔", "字母楼", "保研路", "酱饼妹", "裸奔男", "墨湖", "碧湖", "教工路", "学正街", "二号大街", "中门", "球门", "鸟门",
            "信息楼", "经济楼", "管理楼", "食品楼", "环境楼", "二号田径场", "钱塘江", "金沙湖"
        ];

        var idx = Math.floor(Math.random() * (nameList.length - 1));
        return nameList[idx];
    }

    $("#message").keyup(function (event) {
        if (event.keyCode === 13) {
            $("#sendmessage").click();
        }
    });

    function updateCookie() {
        $.cookie('QQmgs-chat-userid', chat.state.id, { path: '/', expires: 30 });
    }

    // Auto generate name for user
    var userName = makeUserName();
    Materialize.toast('随机分配得到你的匿名昵称: ' + userName, 18000);

    // Get the user name and store it to prepend to messages.
    $('#displayname').val(userName);

    $('#title').append('<h6>当前昵称: ' + userName + '. 刷新页面更换新昵称.</h6>');

    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {

        // Get current time
        var currentdate = new Date().toLocaleTimeString('en-US',
        {
            hour12: false,
            hour: "numeric",
            minute: "numeric"
        });

        if (name === '【系统消息】') {
            // Add the message to the page.
            $('#discussion').append('<li class="collection-item" style="color: cornflowerblue; font-size: 12px; padding-top: 2px; padding-bottom: 1px; padding-left: 15px;">' + currentdate + ' <strong>' + htmlEncode(name)
                + '</strong>: ' + htmlEncode(message) + '</li>');
        } else {
            // Add the message to the page.
            $('#discussion').append('<li class="collection-item" style="padding-left: 15px">' + currentdate + ' <strong>' + htmlEncode(name)
                + '</strong>: ' + htmlEncode(message) + '</li>');
        }

        // Scroll to page bottom
        $("#discussion").scrollTop($("#discussion")[0].scrollHeight);
    };

    // Set initial focus to message input box.
    $('#message').focus();

    // Start the connection.
    $.connection.hub.start({ transport: window.activeTransport },
        function () {
            chat.server.join()
                .done(function () {
                    chat.server.send('【系统消息】', '欢迎“' + userName + '”加入群聊.');

                    $('#sendmessage')
                        .click(function () {

                            // Call the Send method on the hub.
                            chat.server.send(userName, $('#message').val());

                            // Clear text box and reset focus for next comment.
                            $('#message').val('').focus();
                        });
                });
        });

    // This optional function html-encodes messages for display in the page.
    function htmlEncode(value) {
        var encodedValue = $('<div />').text(value).html();
        return encodedValue;
    }
})
