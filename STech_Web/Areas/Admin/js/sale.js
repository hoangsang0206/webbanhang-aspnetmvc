//Add product to create order table
$(document).on('change', '.create-sale input[name="pro-search-id"]', (e) => {
    if ($(e.target).prop('checked') == true) {
        var proID = $(e.target).val();
        $('.pro-search-auto-complete').empty();
        $('.pro-search-auto-complete').hide();
        $('#order-search-p').val('');
        if (proID.length > 0) {
            showLoading();
            $.ajax({
                type: 'get',
                url: '/api/products',
                data: {
                    productID: proID
                },
                success: (data) => {
                    hideLoading();
                    var currentPro = $('.create-sale input[name="ProductID"]').toArray();
                    var exist = currentPro.some(function (el) {
                        return $(el).val() === data.ProductID;
                    });

                    if (exist === false) {
                        if (data.ProductID != null) {
                            var str = `<div class="create-sale-detail-item">
                                <div class="d-flex align-items-center justify-content-between gap-3 my-2">
                                    <label class="text-nowrap">Mã SP</label>
                                    <input type="text" name="ProductID" value="${data.ProductID}" class="form-add-input" required readonly />
                                </div>
                                <div class="d-flex align-items-center justify-content-between gap-3 my-2">
                                    <label class="text-nowrap">Giá gốc</label>
                                    <input type="text" name="OriginalPrice" value="${data.Price}" class="form-add-input" required readonly placeholder="Giá gốc"/>
                                </div>
                                <div class="d-flex align-items-center justify-content-between gap-3 my-2">
                                    <label class="text-nowrap">Giá KM</label>
                                    <input type="text" name="SalePrice" value="${data.Price}" class="form-add-input" required placeholder="Giá khuyến mãi"/>
                                    <button type="button" class="delete-sale-p">
                                        <i class='bx bx-trash'></i>
                                    </button>
                                </div>
                            </div>`;

                            $('.create-sale-detail-box').append(str);
                        }
                    }
                },
                error: () => { }
            })
        }
    }
})

//Delete product in create sale form
$(document).on('click', '.delete-sale-p', (e) => {
    $(e.target).closest('.create-sale-detail-item').remove();
})

//Create sale ------------------------------
$('.create-sale-frm').submit((e) => {
    e.preventDefault();
    var startTime = $('#StartTime').val();
    var endTime = $('#EndTime').val();
    var strSaleDetail = '';
    var saleDetailItems = $('.create-sale-detail-item').toArray();

    saleDetailItems.forEach((item) => {
        var proID = $(item).find('input[name="ProductID"]').val();
        var sPrice = $(item).find('input[name="SalePrice"]').val();

        strSaleDetail += proID + '+' + sPrice + ';;;;;;;;';
    })

    if (strSaleDetail.length > 0) {
        $.ajax({
            type: 'post',
            url: '/admin/sale/create',
            data: {
                'StartTime': startTime,
                'EndTime': endTime,
                saleDetails: strSaleDetail
            },
            success: (response) => {
                if (response.success) {
                    window.location.href = '';
                }
                else {
                    str = `<span>
                            <i class="fa-solid fa-circle-exclamation error-icon"></i>
                           ${response.error}
                        </span>`;
                    $('.create-sale .form-error').empty();
                    $('.create-sale .form-error').show();
                    $('.create-sale .form-error').append(str);
                }
            },
            error: () => {  }
        })
    }
})