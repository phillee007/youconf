$(document).ready(function () {

    $('#name').focus(function () {
        $(this).removeClass('error_class');
    });

    $('#email').focus(function () {
        $(this).removeClass('error_class');
    });

    $('#message').focus(function () {
        $(this).removeClass('error_class');
    });

    $('.contact_form').submit(function () {

        hasError = false;

        if ($('#name').val() == '') {
            $('#name').addClass('error_class');
            hasError = true;
        }

        var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;

        var emailaddressVal = $('#email').val();
        if (emailaddressVal == '') {
            $('#email').addClass('error_class');
            hasError = true;
        }
        else if (!emailReg.test(emailaddressVal)) {
            $('#email').addClass('error_class');
            hasError = true;
        }

        if ($('#email').val() == '') {
            $('#email').addClass('error_class');
            hasError = true;
        }

        if ($('#message').val() == '') {
            $('#message').addClass('error_class');
            hasError = true;
        }

        if (hasError == true) {
            $('.info_box').hide();
            $('.error_box').show();
        }
        else {
            $.ajax({
                type: 'POST',
                url: 'contact.php',
                cache: false,
                data: $(".contact_form").serialize(),
                success: function (data) {
                    if (data == "error") {
                        $('.success_box').hide();
                        $('.error_box').show();
                    }
                    else {
                        $('#name').val('');
                        $('#email').val('');
                        $('#message').val('');
                        $('#website').val('');
                        $('.error_box').hide();
                        $('.success_box').show();
                    }
                }
            });
        }

        return false;
    });
});