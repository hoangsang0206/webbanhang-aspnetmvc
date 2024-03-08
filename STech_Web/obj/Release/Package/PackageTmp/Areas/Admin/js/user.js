//--Tạo khách hàng mới ----------------------------
$('.create-customer-box').submit((e) => {
    e.preventDefault();
    let name = $(e.target).find('#create-cusName').val();
    let phone = $(e.target).find('#create-cusPhone').val();
    let address = $(e.target).find('#create-cusAddress').val();
    let gender = $(e.target).find('input[name="create-cusGender"]:checked').val();
    let dob = $(e.target).find('#create-cusDOB').val();
    let email = $(e.target).find('#create-cusEmail').val();

    if (name.length > 0 && phone.length > 0 && address.length > 0) {
        showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/users/createcustomer',
            data: {
                'CustomerName': name,
                'Phone': phone,
                'Address': address,
                'Gender': gender,
                'DoB': dob,
                'Email': email
            },
            success: (response) => {
                hideLoading();
                if (response.success) {
                    $(e.target).find('.form-error').hide();
                    $(e.target).find('.form-error').empty();
                }
                else {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                    $(e.target).find('.form-error').show();
                    $(e.target).find('.form-error').empty();
                    $(e.target).find('.form-error').append(str);
                }

            },
            error: () => {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>Đã xảy ra lỗi.</span>`;

                $(e.target).find('.form-error').show();
                $(e.target).find('.form-error').empty();
                $(e.target).find('.form-error').append(str);
            }
        })
    }

    $(e.target).find('.form-submit-reset').click(() => {
        $(e.target).find('.form-error').hide();
        $(e.target).find('.form-error').empty();
    })
})

//-- Xem thông tin tài khoản
$(document).on('click', '.user-detail-btn', (e) => {
    var username = $(e.target).data('user-detail');
    if (username.length > 0) {
        showLoading();
        $.ajax({
            type: 'get',
            url: '/api/users',
            data: {
                username: username
            },
            success: (data) => {
                hideLoading();
                $('.users #userName').val(data.UserName);
                $('.users #UserFullName').val(data.UserFullName);
                $('.users #PhoneNumber').val(data.PhoneNumber);
                $('.users #Email').val(data.Email);
                $('.users #Address').val(data.Address);
                $('.users #DOB').val(data.DOB != null ? new Date(data.DOB).toLocaleDateString('en-CA') : '');
                $('.users #userimg').attr('src', data.ImgSrc != null ? data.ImgSrc : "/images/no-user-image-icon.png");
                data.Gender == 'Nam' ? $('#usergender-m').prop('checked', true) : data.Gender == 'Nữ' ? $('#usergender-fm').prop('checked', true) : '';

                var timeOut = setTimeout(() => {
                    $('.user-detail-wrapper').css('visibility', 'visible');
                    $('.user-detail-box').addClass('show');
                    clearTimeout(timeOut);
                }, 600)
            },
            error: () => {  }
        })
    }
})

$('.close-user-detail').click(() => {
    $('.user-detail-wrapper').css('visibility', 'hidden');
    $('.user-detail-box').removeClass('show');
})

//--Xóa tài khoản --------------------------------------
$(document).on('click', '.user-delete-btn', (e) => {
    var username = $(e.target).data('user-delete');
    $('.delete-user-confirm-box').css('visibility', 'visible');
    $('.delete-user-confirm').addClass('show');

    $('.delete-user-confirm-yes').off('click').click(() => {
        $('.delete-user-confirm-box').css('visibility', 'hidden');
        $('.delete-user-confirm').removeClass('show');

        showLoading();
        $.ajax({
            type: 'delete',
            url: `/api/users?username=${username}`,
            success: (response) => {
                hideLoading();
                var timeOut = setTimeout(() => {
                    if (response) {
                        $('.delete-user-confirm-box').css('visibility', 'hidden');
                        $('.delete-user-confirm').removeClass('show');

                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-notice-box').addClass('showForm');
                    }
                    else {
                        $('.delete-user-confirm-box').css('visibility', 'hidden');
                        $('.delete-user-confirm').removeClass('show');

                        $('.fail-delete-notice').css('visibility', 'visible');
                        $('.fail-notice-box').addClass('showForm');
                    }
                }, 600)
            },  
            error: () => {  }
        })
    })

    $('.delete-user-confirm-no').off('click').click(() => {
        $('.delete-user-confirm-box').css('visibility', 'hidden');
        $('.delete-user-confirm').removeClass('show');
    })
})

//--Tạo tài khoản ------------------------------------
$('.create-users-btn').click(() => {
    $('.create-account').css('visibility', 'visible');
    $('.create-account-box').addClass('show');
})

$('.close-create-account').click(() => {
    $('.create-account').css('visibility', 'hidden');
    $('.create-account-box').removeClass('show');
})

$('.create-account-box').submit((e) => {
    e.preventDefault();
    var userName = $('.create-account-box #create-userName').val();
    var password = $('.create-account-box #create-password').val();
    var confirmPassword = $('.create-account-box #create-repassword').val();
    var email = $('.create-account-box #create-Email').val();
    showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/users/createaccount',
        data: {
            ResUsername: userName,
            ResPassword: password,
            ResConfirmPassword: confirmPassword,
            Email: email
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                $('.create-account-box .form-error').hide();
                $('.create-account-box .form-error').empty();

                $('.create-account-box #create-userName').val('');
                $('.create-account-box #create-password').val('');
                $('.create-account-box #create-repassword').val('');
                $('.create-account-box #create-Email').val('');
                var timeOut = setTimeout(() => {
                    $('.complete-create-notice').css('visibility', 'visible');
                    $('.complete-create-notice .complete-notice-box ').addClass('showForm');
                    clearTimeout(timeOut)
                }, 600);
            }
            else {
                if (response.error.length > 0) {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                    $('.create-account-box .form-error').show();
                    $('.create-account-box .form-error').empty();
                    $('.create-account-box .form-error').append(str);
                }
            }
        },
        error: (err) => {  }
    })
})

$('.complete-create-notice button').click(() => {
    $('.complete-create-notice').css('visibility', 'hidden');
    $('.complete-create-notice .complete-notice-box ').removeClass('showForm');
})