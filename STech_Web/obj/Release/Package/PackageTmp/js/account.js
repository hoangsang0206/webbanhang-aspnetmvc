//--Show button loading -------------------------------------------
function showBtnLoading(button) {
    var btnText = button.text();
    button.empty();
    var loadingStr = `<div class="loadingio-spinner-dual-ring-ekj0ol56kwc">
                        <div class="ldio-gmrbyawnrc">
                            <div></div><div><div></div></div>
                        </div>
                    </div>`;
    button.append(loadingStr);

    return btnText;
}

function resetBtn(button, btnText) {
    button.empty();
    button.text(btnText);
}

//--Register with Ajax --------------------------------------------
$('#re-enter-password').keyup(() => {
    var password = $('#register-password').val();
    var confirmPassword = $('#re-enter-password').val();

    if (password !== confirmPassword) {
        $('#re-enter-password').parent('.form-input-box')
            .css('border', '1px solid #e30019');
    }
    else {
        $('#re-enter-password').parent('.form-input-box')
            .css('border', '1px solid #cfcfcf');
    }
})

$('.register-form').submit((e) => {
    e.preventDefault();
    var userName = $('.register #ResUsername').val();
    var password = $('.register #ResPassword').val();
    var confirmPassword = $('.register #ResConfirmPassword').val();
    var email = $('#Email').val();

    var submitBtn = $(e.target).find('.form-submit-btn');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/register',
        data: {
            ResUsername: userName,
            ResPassword: password,
            ResConfirmPassword: confirmPassword,
            Email: email
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                $('.register .form-error').hide();
                $('.register .form-error').empty();

                window.location.href = response.redirectUrl;
            }
            else {
                if (response.error.length > 0) {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                    $('.register .form-error').show();
                    $('.register .form-error').empty();
                    $('.register .form-error').append(str);
                }
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})



//--Login with Ajax -----------------------------------------------
$('.login-form').submit((e) => {
    e.preventDefault();
    var userName = $('.login #Username').val();
    var password = $('.login #Password').val();

    var submitBtn = $(e.target).find('.form-submit-btn');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/login',
        data: {
            Username: userName,
            Password: password
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                $('.login .form-error').hide();
                $('.login .form-error').empty();

                window.location.href = response.redirectUrl;
            }
            else {
                if (response.error.length > 0) {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                        + response.error + `</span>`;

                    $('.login .form-error').show();
                    $('.login .form-error').empty();
                    $('.login .form-error').append(str);
                }
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})

//--Logout ---------------------------------
$('.logout-confirm-yes').click((e) => {
    var btnText = showBtnLoading($(e.target));
    window.location.href = '/account/logout';
})


//--Update with AJAX ----------------------------------------------
function closeUpdateErr() {
    $('.update-error').empty();
}

$('.user-update-form').submit((e) => {
    e.preventDefault();
    var fullName = $('#UserFulName').val();
    var gender = $('input[name="Gender"]:checked').val();
    var phone = $('#PhoneNumber').val();
    var email = $('#Email').val();
    var dob = $('#DOB').val();
    var address = $('#Address').val();

    var submitBtn = $(e.target).find('.user-form-submit');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/update',
        data: {
            UserFullName: fullName,
            Gender: gender,
            PhoneNumber: phone,
            Email: email,
            DOB: dob,
            Address: address
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                var str = '<span>Cập nhật thành công.</span>';
                $('.update-error').empty();
                $('.update-error').append(str);

                var timeout = setTimeout(() => {
                    closeUpdateErr();
                    clearTimeout(timeout);
                }, 6000)
            }
            else {
                $('.update-error').empty();
                var str = '<span>' + response.error +'</span>'
               
                $('.update-error').append(str);

                var timeout = setTimeout(() => {
                    closeUpdateErr();
                    clearTimeout(timeout);
                }, 6000)
            }
        },
        error: (err) => { resetBtn(submitBtn, btnText); }
    })
})

//-Change password -----------------------------------
$('.change-password-form').submit((e) => {
    e.preventDefault();
    var oldPassword = $('#OldPassword').val();
    var newPassword = $('#NewPassword').val();
    var confirmNewPassword = $('#ConfirmNewPassword').val();

    var submitBtn = $(e.target).find('.user-form-submit');
    var btnText = showBtnLoading(submitBtn);
    $.ajax({
        type: 'POST',
        url: '/account/changepassword',
        data: {
            oldPassword: oldPassword,
            newPassword: newPassword,
            confirmNewPassword: confirmNewPassword
        },
        success: (response) => {
            resetBtn(submitBtn, btnText);
            if (response.success) {
                var str = '<span>Đổi mật khẩu thành công.</span>';
                $('.update-error').empty();
                $('.update-error').append(str);

                $('#OldPassword').val('');
                $('#NewPassword').val('');
                $('#ConfirmNewPassword').val('');

                var timeout = setTimeout(() => {
                    closeUpdateErr();
                    clearTimeout(timeout);
                }, 6000)
            }
            else {
                $('.update-error').empty();
                var str = `<span>${response.error}</span>`;

                $('.update-error').append(str);

                var timeout = setTimeout(() => {
                    closeUpdateErr();
                    clearTimeout(timeout);
                }, 6000)
            }

        },
        error: () => { resetBtn(submitBtn, btnText); }
    })
})

//--Upload user image ----------
$('.upload-img-btn').click(() => {
    $('.upload-user-image').addClass('show');
    $('.upload-form-box').addClass('show');

})

$('.close-upload-frm').click(() => {
    $('.upload-user-image').removeClass('show');
    $('.upload-form-box').removeClass('show');

    $('.upload-user-img').empty();
    $('.upload-user-img').removeClass('img-uploaded');
    $('.upload-user-img').append(`<i class="fa-solid fa-cloud-arrow-up"></i><span>Tải tên hoặc kéo thả hình ảnh vào đây</span>`);
    $('.upload-frm-notice').empty();
    $('.upload-frm-notice').removeClass('success')
    $('.upload-frm-notice').removeClass('failed')
    $('.upload-frm-notice').css('display', 'none !important');
})
//------------
function updateUploadFormNotice(string, status) {
    $('.upload-frm-notice').empty();
    $('.upload-frm-notice').show();
  
    if (status) {
        $('.upload-frm-notice').addClass('success')
        $('.upload-frm-notice').removeClass('failed')
    }
    else {
        $('.upload-frm-notice').addClass('failed')
        $('.upload-frm-notice').removeClass('success')
    }

    $('.upload-frm-notice').append(string);
}
//------------

$(document).ready(() => {
    function uploadUserImage(file) {
        if (file) {
            let formData = new FormData();
            formData.append('file', file);

            $('.upload-progress-bar').css('display', 'flex');
            $('.upload-progress-bar').empty();
            $.ajax({
                type: 'POST',
                url: '/account/uploadimage',
                data: formData,
                processData: false,
                contentType: false,
                xhr: () => {
                    var xhr = $.ajaxSettings.xhr();
                    xhr.upload.onprogress = (event) => { //Upload progress
                        var percentComplete = Math.floor((event.loaded / event.total) * 100);
                        var progressHtml = `<i class='bx bxs-file-image'></i>
                                <div class="progress-bar d-flex flex-row align-items-center justify-content-start">
                                    <div class="bar" style="width: ${percentComplete + '%'}"></div>
                                </div>
                                <div class="progress-percent">${percentComplete}%</div>`;
                        $('.upload-progress-bar').empty();
                        $('.upload-progress-bar').append(progressHtml);
                    }
                    return xhr;
                },
                success: (res) => {
                    $('.upload-progress-bar').empty();
                    $('.upload-progress-bar').hide();

                    if (res.success) {
                        var str = `<i class='bx bxs-check-circle'></i>
                            <span>Tải lên thành công.</span>`;
                        var strImg = `<img src="${res.src}" alt="" />`
                        $('.upload-user-img').empty();
                        $('.upload-user-img').addClass('img-uploaded');
                        $('.upload-user-img').append(strImg);

                        updateUploadFormNotice(str, res.success);
                        $('.user-img').attr('src', res.src);
                    }
                    else {
                        var str = `<i class='bx bxs-x-circle'></i>
                            <span>${res.error}</span>`;
                        var strElement = ` <i class="fa-solid fa-cloud-arrow-up"></i><span>Tải hình ảnh lên</span>`
                        $('.upload-user-img').empty();
                        $('.upload-user-img').removeClass('img-uploaded');
                        $('.upload-user-img').append(strElement);
                        updateUploadFormNotice(str, res.success);
                    }
                },
                error: (err) => {
                    var str = `<i class='bx bxs-x-circle'></i>
                            <span>Tải lên thất bại.</span>`;
                    updateUploadFormNotice(str, false);
                    var strElement = ` <i class="fa-solid fa-cloud-arrow-up"></i><span>Tải hình ảnh lên</span>`
                    $('.upload-user-img').empty();
                    $('.upload-user-img').removeClass('img-uploaded');
                    $('.upload-user-img').append(strElement);
                    console.log(err);
                }
            })
        }
    }

    $('.upload-user-img').on('dragover dragenter', () => {
        $('.upload-user-img').addClass('dragenter');
    })

    $('.upload-user-img').on('dragleave dragend drop', () => {
        $('.upload-user-img').removeClass('dragenter');
    })

    $('.upload-user-img').click(() => {
        $('.file-input').click();
    })

    $('.file-input').on('change', ({target}) => {
        let file = target.files[0];
        uploadUserImage(file);
    });

    $('.upload-user-img').on('dragover', (e) => {
        e.preventDefault();
    })

    $('.upload-user-img').on('drop', (e) => {
        e.preventDefault();
        var files = e.originalEvent.dataTransfer.files;
        uploadUserImage(files[0]);
    })
});

//-----------------------------------------------
function setParentHeight() {
    var childHeight = $('.account-right-box.current').outerHeight(true);
    $('.account-right-side').css('height', childHeight + 'px');
}

function showCard() {
    var idFromUrl = window.location.hash.substring(1);
    if (idFromUrl.length > 0) {
        $('.account-right-box').removeClass('current');
        $('.account-nav-list-item a').removeClass('activeNav');
        $('[data-account-side="' + idFromUrl + '"').addClass('current');
        $('a[href="#' + idFromUrl +'"').addClass('activeNav');
    } 
    setParentHeight();
}

showCard();

$(window).on('hashchange' ,() => {
    showCard();
})

// Format datetime ASP.NET to "dd/MM/yyyy"
function formatDateFromAspNet(jsonDate) {
    var ticks = /\/Date\((\d+)\)\//.exec(jsonDate);
    if (ticks) {
        var milliseconds = parseInt(ticks[1]);
        var date = new Date(milliseconds);
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        return day + '/' + month + '/' + year;
    }
    return jsonDate;
}

//Order search ----------------------------------------------
$('.order-search-form').submit((e) => {
    e.preventDefault();
    var orderID = $('#order-search').val();

    showWebLoader();
    $.ajax({
        type: 'POST',
        url: '/order/searchorder',
        data: {
            orderID: orderID
        },
        success: (data) => {
            setTimeout(hideWebLoader, 500);
            var str = ` <tr>
                    <th>Mã ĐH</th>
                    <th>Ngày đặt</th>
                    <th>Tổng tiền</th>
                    <th>Trạng thái thanh toán</th>
                    <th>Trạng thái</th>
                    <th></th>
                </tr>`;
            $('.order-list table tbody').empty();
            $.each(data.orders, (index, order) => {
                var statusClass = "order-success";
                if (order.PaymentStatus == "Thanh toán thất bại") { statusClass = "order-failed"; }
                else if (order.PaymentStatus == "Chờ thanh toán") { statusClass = "order-waiting"; }
                str += `<tr>
                    <td class="order-id">${order.OrderID}</td>
                    <td class="order-date">${formatDateFromAspNet(order.OrderDate)}</td>
                    <td class="order-total">${order.TotalPaymentAmout.toLocaleString("vi-VN")}đ</td>
                    <td>
                        <div class="order-status ${statusClass}">${order.PaymentStatus}</div>
                    </td>
                     <td>
                        <div class="order-status ${order.Status == 'Đã xác nhận' ? 'order-success' : 'order-waiting'}">${order.Status}</div>
                    </td>
                    <td> <a href="/order/detail/${order.OrderID}">Chi tiết</a></td>
                </tr>`;
            })

            $('.order-list table tbody').append(str);
            setParentHeight();
        },
        error: () => { hideWebLoader(); }
    })
})

//Get order list by Status
$('.order-header-list li').click((e) => {
    $('.order-header-list li').removeClass('active');
    $(e.target).addClass('active');

    var value = $(e.target).data('get-order');
    if (value.length > 0) {
        showWebLoader();
        $.ajax({
            type: 'POST',
            url: '/order/getorder',
            data: {
                status: value
            },
            success: (data) => {
                setTimeout(hideWebLoader, 500);
                var str = ` <tr>
                    <th>Mã ĐH</th>
                    <th>Ngày đặt</th>
                    <th>Tổng tiền</th>
                    <th>Trạng thái thanh toán</th>
                    <th>Trạng thái</th>
                    <th></th>
                </tr>`;
                $('.order-list table tbody').empty();
                $.each(data.orders, (index, order) => {
                    var statusClass = "order-success";
                    if (order.PaymentStatus == "Thanh toán thất bại") { statusClass = "order-failed"; }
                    else if (order.PaymentStatus == "Chờ thanh toán") { statusClass = "order-waiting"; }
                    str += `<tr>
                    <td class="order-id">${order.OrderID}</td>
                    <td class="order-date">${formatDateFromAspNet(order.OrderDate)}</td>
                    <td class="order-total">${order.TotalPaymentAmout.toLocaleString("vi-VN")}đ</td>
                    <td>
                        <div class="order-pstatus ${statusClass}">${order.PaymentStatus}</div>
                    </td>
                    <td>
                        <div class="order-status ${order.Status == 'Đã xác nhận' ? 'order-success' : 'order-waiting'}">${order.Status}</div>
                    </td>
                    <td> <a href="/order/detail/${order.OrderID}">Chi tiết</a></td>
                </tr>`;
                })

                $('.order-list table tbody').append(str);
                setParentHeight();
            },
            error: () => { hideWebLoader(); }
        })
    }
})