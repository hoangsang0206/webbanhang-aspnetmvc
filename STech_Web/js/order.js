//---------------------------------

//Kiểm tra thông tin người đặt hàng 
$('.order-info-summary').click(() => {
    var gender = $('input[name="gender"]:checked').val();
    var name = $('#UserFullName').val();
    var phone = $('#PhoneNumber').val();
    var shipMethod = $('input[name="shipmethod"]:checked').val();
    var address = $('#ship-address').val();
    var note = $('#cart-note').val();

    $.ajax({
        type: 'Post',
        url: '/order/checkorderinfo',
        data: {
            gender: gender,
            customerName: name,
            customerPhone: phone,
            shipMethod: shipMethod,
            address: address,
            note: note
        },
        success: (res) => {
            if (res.success) {       
                $.ajax({
                    type: 'POST',
                    url: '/account/update',
                    data: {
                        UserFullName: name,
                        Gender: gender,
                        PhoneNumber: phone,
                        Address: address
                    },
                    success: (response) => {
                        if (response.success) {
                            window.location.href = '/order/orderinfo';
                        }
                    },
                    error: () => { }
                })
            }
            else {
                $('.cart-info .form-error').empty();
                $('.cart-info .form-error').show();
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation"></i>`
                    + res.error + `</span>`;
                $('.cart-info .form-error').append(str);

                setTimeout(function () {
                    $('.cart-info .form-error').hide();
                }, 8000)
            }
        },
        error: () => {  }
    })
})

//Thanh toán qua thẻ VISA/MASTERCARD --------------------------------------------
$('.payment-action').click(() => {
    var paymentMethod = $('input[name="payment-method"]:checked').val();

    if (paymentMethod.length > 0) {
        $.ajax({
            type: 'Post',
            url: '/order/checkout',
            data: {
                paymentMethod: paymentMethod
            },
            success: (res) => {
                if (res.success == false) {
                    var str = ``;
                    $.each(res.error, (index, value) => {
                        console.log(value)
                        str += `<li><i class="fa-solid fa-circle-exclamation"></i><span>${value}</span></li>`;
                    })

                    showOrderNotice(str);
                }
                else {
                    window.location.href = res.url;
                }
            },
            error: () => { console.log("Payment Error") }
        })
    }

})

//--------------
function showOrderNotice(string) {
    $('.notice-list').empty();
    $('.checkout-notice-wrapper').css('visibility', 'visible');
    $('.checkout-notice').addClass('showNotice');

    $('.notice-list').append(string);

    $('.close-notice').click(() => {
        $('.notice-list').empty();
        $('.checkout-notice-wrapper').css('visibility', 'hidden');
        $('.checkout-notice').removeClass('showNotice');
    })
}