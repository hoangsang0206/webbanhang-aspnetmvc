
if (window.innerWidth < 768) {
    $('.sidebar').addClass('close');
}

//--------------------------------------------------------------------

$('.toggle').click(() => {
    $('.sidebar').toggleClass('close');
})

//-------------------------------------------------------------------
function checkInputValid(_input) {
    if (_input.val().length > 0) {
        _input.addClass('input-valid');
    }
    else {
        _input.removeClass('input-valid');
    }
}

const inputArr = $('.form-input').toArray();
inputArr.forEach((input) => {
    var _input = $(input);

    _input.on({
        focus: () => { checkInputValid(_input) },
        change: () => { checkInputValid(_input) }
    });
})

// --- Admin login with ajax ----------------------------------------
$('.admin-login-form').submit((e) => {
    e.preventDefault();
    var userName = $('#admin-username').val();
    var password = $('#admin-password').val();

    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            Username: userName,
            Password: password
        },
        success: (response) => {
            if (response.success) {
                $('.form-error').hide();
                $('.form-error').empty();
                $('.admin-login-form').unbind('submit').submit();
                window.location.href = response.redirectUrl;
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                    + response.error + `</span>`;

                $('.form-error').show();
                $('.form-error').empty();
                $('.form-error').append(str);
            }
        },
        error: (err) => { console.log(err) }
    })
})

//-------------------------------------------------------
$('.dashboard-categories').click(() => {
    window.location.href = '/admin/categories';
})

$('.dashboard-products').click(() => {
    window.location.href = '/admin/products';
})

$('.dashboard-orders').click(() => {
    window.location.href = '/admin/orders';
})
