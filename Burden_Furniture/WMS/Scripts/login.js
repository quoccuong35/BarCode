$(function () {
    "use strict";
    $(document).ready(function () {

        $('input').on('keydown', function (e) {
            $('#iAlert').addClass('hide');
            if (e.which == 13) {
                Login();
            }
        });
        $('input').focus(function () {
            $('#iAlert').addClass('hide');
        });
        $('#btnlogin').click(function () {
            Login();
        });
    });

    function Login() {
        var formData = new FormData();
        $('#iAlert').addClass('hide');
        if ($('#username').val()=='') {
            $('#username').focus();
            $('.error_message').text("Tài khoản không được trống");
            $('#iAlert').removeClass('hide');

        } else if ($('#password').val()=='') {
            $('#password').focus();
            $('.error_message').text("Mật khẩu không được trống");
            $('#iAlert').removeClass('hide');
        }
        else {
            formData.append("Username", $('#username').val());
            formData.append("Password", $('#password').val());
            formData.append("__RequestVerificationToken", $('input[name="__RequestVerificationToken"]').val());
            $.ajax({
                url: '/Account/GetLogin',
                type: "POST",
                enctype: 'multipart/form-data',
                contentType: false,
                processData: false,
                data: formData,
                success: function (data) {
                    if (Number(data[0]) == 1) {
                        window.location.href = data[1];
                    } else {

                        $('.error_message').text(data[1]);
                        $('#iAlert').removeClass('hide');
                    }
                },
                error: function () {

                }
            });
        }
    }

});

