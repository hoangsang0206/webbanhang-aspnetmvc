//--Lazy loading -------------------------------------------------
function loadImg(img) {
    const url = img.getAttribute('lazy-src');

    img.removeAttribute('lazy-src');
    img.setAttribute('src', url);
    img.classList.add('img-loaded');
    //img.parentNode.classList.remove('lazy-loading');
}

function lazyLoading() {
    if ('IntersectionObserver' in window) {
        var lazyImages = document.querySelectorAll('[lazy-src]');
        let observer = new IntersectionObserver((entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting && !entry.target.classList.contains('img-loaded')) {
                    loadImg(entry.target);
                }
            })
        }));

        lazyImages.forEach(img => {
            observer.observe(img);
        });
    }
}



//---------Add, update, detele product --------------------------------------
//Get product list ---------------------------------------------------
function appendProductList(responses) {
    if (responses != null) {
        $('.product-list').empty();
        if (responses.length > 14) {
            $('.product-list').css('height', '40rem');
        }
        else if (responses.length <= 0) {
            $('.product-list').css('height', '27rem');
        }
        else {
            $('.product-list').css('height', 'auto');
        }

        var str1 = "Tìm thấy: " + responses.length + " sản phẩm";
        $('.total-result').empty();
        $('.total-result').append(str1);

        for (var i = 0; i <= responses.length; i++) {
            if (responses[i] != null) {
                var str2 = `<div class="product-box-container">
                                            <div class="product-box position-relative">
                                                ${responses[i].Quantity <= 0 ? `<div class="product-oot">
                                                    <span>Hết hàng</span>
                                                </div>` : ""}
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-image lazy-loading">
                                                        <img lazy-src="${responses[i].ImgSrc != null ? responses[i].ImgSrc : '/images/no-image.jpg'}" class="product-img">
                                                    </div>
                                                </a>
                                                <a href="/admin/product/${responses[i].ProductID}" class="product-link">
                                                    <div class="product-name">
                                                        ${responses[i].ProductName}
                                                    </div>
                                                </a>
                                                <div class="product-original-price">
                                                    ${responses[i].Cost > responses[i].Price ? responses[i].Cost.toLocaleString("vi-VN") + 'đ' :
                        `<span style="visibility: hidden">0</span>`
                    }
                                                </div>
                                                <div class="product-price d-flex align-items-center">
                                                    ${responses[i].Price.toLocaleString("vi-VN") + 'đ'}
                                                </div>
                                                <button class="update-product-btn product-hidden-btn" onclick="window.location.href='/admin/product/${responses[i].ProductID}'">
                                                    <i class="fa-solid fa-screwdriver-wrench"></i>
                                                </button>
                                                <button class="delete-product-btn product-hidden-btn" data-product-id="${responses[i].ProductID}">
                                                    <i class="fa-solid fa-trash"></i>
                                                </button>
                                            </div>
                                        </div>`;

                $('.product-list').append(str2);
            }
        }

        lazyLoading();
    }
    else {

    }
}
//Reload ----------------------------
$('.reload-products').click(() => {
   showLoading();
    hideLoading();
    $('.product-list').empty();
    $('.total-result').empty();
    $('.product-list').css('height', '27rem');
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
})
//Search ----------------------------
$('.search-products').submit((e) => {
    e.preventDefault();
    var searchText = $('#search').val();

    if (searchText.length > 0) {
       showLoading();
        $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
        $.ajax({
            type: 'GET',
            url: '/api/products',
            data: {
                name: searchText
            },
            success: (responses) => {
                hideLoading();
                appendProductList(responses);
            },
            error: (err) => {
                hideLoading();
                console.log(err);
            }
        })
    }
})

//Get all product -------------------
$('.get-all-product').click(() => {
   showLoading();
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);
    $.ajax({
        type: 'GET',
        url: '/api/products',
        success: (responses) => {
            hideLoading();
            appendProductList(responses);
        },
        error: (err) => {
            hideLoading();
            console.log(err);
        }
    })
})

//Get product by category or by brand
$('#cateID, #brandID').on('change', () => {
    var cateID = $('#cateID').val();
    var brandID = $('#brandID').val();
    $('#search').val('');
   showLoading();

    $.ajax({
        type: 'GET',
        url: '/api/products',
        data: {
            CateID: cateID,
            BrandID: brandID
        },
        success: (responses) => {
            hideLoading();
            appendProductList(responses);
        },
        error: (err) => {
            hideLoading();
            console.log(err);
        }
    })
})

//--Get product out of stock
$('.get-out-of-stock').click(() => {
   showLoading();
    $('#search').val('');
    $('.get-product-by-cate, .get-product-by-brand').prop('selectedIndex', 0);

    $.ajax({
        type: 'get',
        url: '/api/products',
        data: {
            type: 'OOT'
        },
        success: (responses) => {
            hideLoading();
            appendProductList(responses);
        },
        error: () => {
            hideLoading();
            console.log('Cannot get product list');
        }
    })

})



//Add product --------------------------------------------------------
//Check quantity > 0
$('.add-product #Quantity').keyup(() => {
    var inputVal = $('.add-product #Quantity').val();
    if (isNaN(inputVal)) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Số lượng không hợp lệ.
        </span>`;
        $('.add-product .form-error').empty();
        $('.add-product .form-error').show();
        $('.add-product .form-error').append(str);
    } else if (inputVal < 0) {
        var str = `<span>
            <i class="fa-solid fa-circle-exclamation error-icon"></i>
            Số lượng không được nhỏ hơn 0.
        </span>`;
        $('.add-product .form-error').empty();
        $('.add-product .form-error').show();
        $('.add-product .form-error').append(str);
    }
    else {
        $('.add-product .form-error').empty();
        $('.add-product .form-error').hide();
    }
})

//--Show/hide form product--------------------
$('.add-product-btn').click(() => {
    $('.add-product').css('visibility', 'visible');
    $('.product-form-container').addClass('showForm');
})

$('.close-product-form').click(() => {
    $('.add-product').css('visibility', 'hidden');
    $('.product-form-container').removeClass('showForm');
})

function showOkForm() {
    $('.complete-notice').css('visibility', 'visible');
    $('.complete-notice .complete-notice-box').addClass('showForm');

    $('.complete-notice-box button').click(() => {
        $('.complete-notice').css('visibility', 'hidden');
        $('.complete-notice .complete-notice-box').removeClass('showForm');
    })
}

function showUpdateOkForm() {
    $('.complete-update-notice').css('visibility', 'visible');
    $('.complete-update-notice .complete-notice-box').addClass('showForm');

    $('.complete-notice-box button').click(() => {
        $('.complete-update-notice').css('visibility', 'hidden');
        $('.complete-update-notice .complete-notice-box').removeClass('showForm');
    })
}

//Update image box
function updateImgBox(input, imgbox) {
    input.on('change mouseleave blur', () => {
        var imgVal = input.val();
        if (imgVal.length > 0) {
            imgbox.attr('src', imgVal);
        }
        else {
            imgbox.attr('src', '/images/no-image.jpg');
        }
    })
}

$(document).ready(() => {
    updateImgBox($('.add-product #ImgSrc'), $('.p-img-1'));
    updateImgBox($('.add-product #add-product-image-1'), $('.p-img-2'));
    updateImgBox($('.add-product #add-product-image-2'), $('.p-img-3'));
    updateImgBox($('.add-product #add-product-image-3'), $('.p-img-4'));
    updateImgBox($('.add-product #add-product-image-4'), $('.p-img-5'));
    updateImgBox($('.add-product #add-product-image-5'), $('.p-img-6'));
    updateImgBox($('.add-product #add-product-image-6'), $('.p-img-7'));
    updateImgBox($('.add-product #add-product-image-7'), $('.p-img-8'));
})

$(document).ready(() => {
    var imageList = $('.product-detail .product-detail-image img').toArray();
    updateImgBox($('.product-detail #ImgSrc'), $(imageList[0]));
    updateImgBox($('.product-detail #add-product-image-1'), $(imageList[1]));
    updateImgBox($('.product-detail #add-product-image-2'), $(imageList[2]));
    updateImgBox($('.product-detail #add-product-image-3'), $(imageList[3]));
    updateImgBox($('.product-detail #add-product-image-4'), $(imageList[4]));
    updateImgBox($('.product-detail #add-product-image-5'), $(imageList[5]));
    updateImgBox($('.product-detail #add-product-image-6'), $(imageList[6]));
    updateImgBox($('.product-detail #add-product-image-7'), $(imageList[7]));
})

//Add product --------------------------------
$('.add-product-form').submit((e) => {
    var productID = $('.add-product #ProductID').val();
    var productName = $('.add-product #ProductName').val();
    var productCost = $('.add-product #Cost').val();
    var productPrice = $('.add-product #Price').val();
    var cateID = $('.add-product #CateID').val();
    var brandID = $('.add-product #BrandID').val();
    var imgSrc = $('.add-product #ImgSrc').val();
    var warranty = $('.add-product #Warranty').val();
    var quantity = $('.add-product #Quantity').val();
    var imgSrc1 = $('.add-product #add-product-image-1').val();
    var imgSrc2 = $('.add-product #add-product-image-2').val();
    var imgSrc3 = $('.add-product #add-product-image-3').val();
    var imgSrc4 = $('.add-product #add-product-image-4').val();
    var imgSrc5 = $('.add-product #add-product-image-5').val();
    var imgSrc6 = $('.add-product #add-product-image-6').val();
    var imgSrc7 = $('.add-product #add-product-image-7').val();
    var mDate = $('.add-product #ManufacturingDate').val();
    var type = $('.add-product #Type').val();
    var description = $('.add-product #Description').val();
    e.preventDefault();

    //Clear input value
    function clearInputVal() {
        const inputFormArr = $('.add-product input').toArray();
        inputFormArr.forEach((input) => {
            $(input).val('');
            checkInputValid($(input));
        })

        const selectArr = $('.add-product select').toArray();
        selectArr.forEach((select) => {
            $(select).prop('selectedIndex', 0);
        })

        $('.add-product textarea').val('');
    }
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/products/addproduct',
        data: {
            'ProductID': productID,
            'ProductName': productName,
            'Cost': productCost,
            'Price': productPrice,
            'CateID': cateID,
            'BrandID': brandID,
            'ImgSrc': imgSrc,
            'Warranty': warranty,
            quantity: quantity,
            'ImgSrc1': imgSrc1,
            'ImgSrc2': imgSrc2,
            'ImgSrc3': imgSrc3,
            'ImgSrc4': imgSrc4,
            'ImgSrc5': imgSrc5,
            'ImgSrc6': imgSrc6,
            'ImgSrc7': imgSrc7,
            'ManufacturingDate': mDate,
            'Description': description,
            'Type': type
        },
        success: (responses) => {
            hideLoading();
            if (responses.success) {
                $('.add-product .form-error').empty();
                $('.add-product .form-error').hide();
                var interval = setInterval(() => {
                    clearInputVal();
                    showOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                        <i class="fa-solid fa-circle-exclamation error-icon"></i>
                        ${responses.error}
                    </span>`;
                $('.add-product .form-error').empty();
                $('.add-product .form-error').show();
                $('.add-product .form-error').append(str);

                var interval = setInterval(() => {
                    $('.add-product .form-error').empty();
                    $('.add-product .form-error').hide();
                    clearInterval(interval)
                }, 8000)
            }
        },
        error: (err) => {
            console.log(err);
            hideLoading();
        }
    })
})

//----
$('.add-product-form').on('reset', () => {
    const inputFormArr = $('.add-product input').toArray();
    inputFormArr.forEach((input) => {
        $(input).val('');
        checkInputValid($(input));
    })

    const selectArr = $('.add-product select').toArray();
    selectArr.forEach((select) => {
        $(select).prop('selectedIndex', 0);
    })
})

//-------
$(document).ready(() => {
    var productDetailInput = $('.product-detail-form input').toArray();
    productDetailInput.forEach((input) => {
        checkInputValid($(input));
    })
})

//Update product --------------------------------
$('.product-detail-form #ProductID').on('keydown', (e) => {
    e.preventDefault();
})

$('.product-detail-form').submit((e) => {
    e.preventDefault();
    var productID = $('.product-detail-form #ProductID').val();
    var productName = $('.product-detail-form #ProductName').val();
    var productCost = $('.product-detail-form #Cost').val();
    var productPrice = $('.product-detail-form #Price').val();
    var cateID = $('.product-detail-form #CateID').val();
    var brandID = $('.product-detail-form #BrandID').val();
    var imgSrc = $('.product-detail-form #ImgSrc').val();
    var warranty = $('.product-detail-form #Warranty').val();
    var quantity = $('.product-detail-form #add-product-quantity').val();
    var imgSrc1 = $('.product-detail-form #add-product-image-1').val();
    var imgSrc2 = $('.product-detail-form #add-product-image-2').val();
    var imgSrc3 = $('.product-detail-form #add-product-image-3').val();
    var imgSrc4 = $('.product-detail-form #add-product-image-4').val();
    var imgSrc5 = $('.product-detail-form #add-product-image-5').val();
    var imgSrc6 = $('.product-detail-form #add-product-image-6').val();
    var imgSrc7 = $('.product-detail-form #add-product-image-7').val();
    var mDate = $('.product-detail-form #ManufacturingDate').val();
    var type = $('.product-detail-form #Type').val();
    var description = $('.product-detail-form #Description').val();
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/products/updateproduct',
        data: {
            'ProductID': productID,
            'ProductName': productName,
            'Cost': productCost,
            'Price': productPrice,
            'CateID': cateID,
            'BrandID': brandID,
            'ImgSrc': imgSrc,
            'Warranty': warranty,
            quantity: quantity,
            'ImgSrc1': imgSrc1,
            'ImgSrc2': imgSrc2,
            'ImgSrc3': imgSrc3,
            'ImgSrc4': imgSrc4,
            'ImgSrc5': imgSrc5,
            'ImgSrc6': imgSrc6,
            'ImgSrc7': imgSrc7,
            'ManufacturingDate': mDate,
            'Description': description,
            'Type': type
        },
        success: (responses) => {
            hideLoading();
            if (responses.success) {
                $('.product-detail .form-error').empty();
                $('.product-detail .form-error').hide();
                var interval = setInterval(() => {
                    showUpdateOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                        <i class="fa-solid fa-circle-exclamation error-icon"></i>
                        ${responses.error}
                    </span>`;
                $('.product-detail .form-error').empty();
                $('.product-detail .form-error').show();
                $('.product-detail .form-error').append(str);

                var interval = setInterval(() => {
                    $('.add-product .form-error').empty();
                    $('.add-product .form-error').hide();
                    clearInterval(interval)
                }, 8000)
            }
        },
        error: () => {
            hideLoading();
        }
    })
})

//Delete product --------------------------------
$(document).on('click ', '.delete-product-btn', (e) => {
    const productID = $(e.target).data('product-id');
    //----------
    $('.delete-product-confirm').css('visibility', 'visible');
    $('.delete-product-confirm .delete-confirm-box').addClass('show');
    //----------
    $('.cancel-delete').off('click').click(() => {
        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    })
    //----------
    $('.confirm-delete').off('click').click(() => {
        //----------
       showLoading();
        $.ajax({
            type: 'Delete',
            url: `/api/products?productID=${productID}`,
            success: (res) => {
                hideLoading();
                if (res) {
                    var interval = setInterval(() => {
                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-delete-notice .complete-notice-box').addClass('showForm');
                        clearInterval(interval);
                    }, 620);
                }
                else {
                    var interval = setInterval(() => {
                        $('.fail-delete-notice').css('visibility', 'visible')
                        $('.fail-delete-notice .fail-notice-box').addClass('showForm');
                        clearInterval(interval);
                    }, 620);
                    
                }
            },
            error: () => {
                hideLoading();
            }
        })

        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    })
})

//-----
$('.product-delete-frm-btn').click(() => {
    var productID = $('#ProductID').val();
    //----------
    $('.pro-detail-del').css('visibility', 'visible');
    $('.pro-detail-del .delete-confirm-box').addClass('show');
    //----------
    $('.pro-detail-del .cancel-delete').off('click').click(() => {
        $('.pro-detail-del').css('visibility', 'hidden');
        $('.pro-detail-del .delete-confirm-box').removeClass('show');
    })
    //----------
    $('.pro-detail-del .confirm-delete').off('click').click(() => {
        $.ajax({
            type: 'Delete',
            url: `/api/products?productID=${productID}`,
            success: (res) => {
                if (res) {
                    window.location.href = '/admin/products'
                }
            },
            error: () => {
            }
        })
    })
})
//-----

$('.delete-product-confirm').click((e) => {
    if ($(e.target).closest('.delete-confirm-box').length <= 0) {
        $('.delete-product-confirm').css('visibility', 'hidden');
        $('.delete-product-confirm .delete-confirm-box').removeClass('show');
    }
})

$('.complete-delete-notice .complete-notice-box button').click(() => {
    $('.complete-delete-notice').css('visibility', 'hidden');
    $('.complete-delete-notice .complete-notice-box').removeClass('showForm');
})

$('.complete-delete-notice').click((e) => {
    if ($(e.target).closest('.complete-notice-box').length <= 0) {
        $('.complete-delete-notice').css('visibility', 'hidden');
        $('.complete-delete-notice .complete-notice-box').removeClass('showForm');
    }
})

$('.fail-delete-notice .fail-notice-box button').click(() => {
    $('.fail-delete-notice').css('visibility', 'hidden')
    $('.fail-delete-notice .fail-notice-box').removeClass('showForm');
})


//---Thêm quà tặng kèm của sản phẩm -----------------------------------
$(document).on('change', '.add-product-gifts input[name="pro-search-id"]', (e) => {
    if ($(e.target).prop('checked') == true) {
        var proID = $(e.target).val();
        $('.add-product-gifts .pro-search-auto-complete').empty();
        $('.add-product-gifts .pro-search-auto-complete').hide();
        $('.add-product-gifts #order-search-p').val('');
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
                    var currentPro = $('.add-gift-box input[name="p-gift-id"]').toArray();
                    var exist = currentPro.some(function (el) {
                        return $(el).val() === data.ProductID;
                    });

                    if (exist === false) {
                        if (data.ProductID != null) {
                            var str = `<div class="p-gift-item">
                                <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                                    <label class="text-nowrap">Mã SP tặng kèm</label>
                                    <input type="text" name="p-gift-id" value="${data.ProductID}" class="form-add-input" required readonly />
                                </div>
                                <div class="d-flex align-items-center justify-content-between gap-3 ps-5 my-3">
                                    <label>Tên SP tặng kèm</label>
                                    <input type="text" name="p-gift-name" value="${data.ProductName}" class="form-add-input" readonly />
                                    <button type="button" class="delete-p-gift">
                                        <i class='bx bx-trash'></i>
                                    </button>
                                </div>
                            </div>`;

                            $('.add-gift-box').append(str);
                        }
                    }
                },
                error: () => { }
            })
        }
    }
})

// -----------------
$(document).on('click', '.delete-p-gift', (e) => {
    $(e.target).closest('.p-gift-item').remove();
})

$('.add-gift-btn').click(() => {
    var proID = $('.product-detail #ProductID').val();
    var strGifts = '';
    var giftList = $('.add-gift-box input[name="p-gift-id"]').toArray();
    giftList.forEach(item => {
        strGifts += $(item).val() + ';;;;;;;;';
    })

    if (strGifts.length > 0) {
        showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/products/addgifts',
            data: {
                productID: proID,
                strGifts: strGifts
            },
            success: (res) => {
                hideLoading();
                if (res.success) {
                    var interval = setInterval(() => {
                        showUpdateOkForm();
                        clearInterval(interval);
                    }, 620);
                }
            },
            error: () => {  }
        })
    }
})

//-- Add new specification
$(document).on('click', '.delete-p-spec', (e) => {
    $(e.target).closest('.p-spec-item').remove();
}) 

$('.add-new-spec').click(() => {
    var str = `<div class="p-spec-item">
                    <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                        <label class="text-nowrap">Tên thông số</label>
                        <input type="text" name="p-spec-name" value="" class="form-add-input" required />
                    </div>
                    <div class="d-flex align-items-center justify-content-between gap-3 ps-5 my-3">
                        <label>Nội dung</label>
                        <input type="text" name="p-spec-content" value="" class="form-add-input" required />
                        <button type="button" class="delete-p-spec">
                            <i class='bx bx-trash'></i>
                        </button>
                    </div>
                </div>`;

    $('.spec-list').append(str);
})

$('.add-spec-btn').click(() => {
    var proID = $('.product-detail #ProductID').val();
    var strSpecs = '';
    var specList = $('.propduct-specifications .p-spec-item').toArray();
    specList.forEach(item => {
        var specName = $(item).find('input[name="p-spec-name"]').val();
        var specContent = $(item).find('input[name="p-spec-content"]').val();

        strSpecs += specName + '++++++++' + specContent + ';;;;;;;;';
    })

    if (strSpecs.length > 0) {
        showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/products/addspecification',
            data: {
                productID: proID,
                specifications: strSpecs
            },
            success: (res) => {
                hideLoading();
                if (res.success) {
                    var interval = setInterval(() => {
                        showUpdateOkForm();
                        clearInterval(interval);
                    }, 620);
                }
            },
            error: () => {  }
        })
    }
})


// --Add new content
$(document).on('click', '.delete-p-content', (e) => {
    $(e.target).closest('.p-content-item').remove();
})

$('.add-new-content').click(() => {
    var str = `<div class="p-content-item">
                    <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                        <label class="text-nowrap">Tiêu đề đoạn</label>
                        <input type="text" name="p-content-title" value="" class="form-add-input" />
                    </div>
                    <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                        <label class="text-nowrap">Nội dung</label>
                        <textarea class="form-add-input" name="p-content-main" style="overflow: hidden"></textarea>
                    </div>
                    <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                        <label class="text-nowrap">Hình ảnh</label>
                        <input type="text" name="p-content-img" value="" class="form-add-input" />
                    </div>
                    <div class="d-flex align-items-center justify-content-between gap-3 my-3">
                        <label class="text-nowrap">Video (nhúng iframe ytb)</label>
                        <textarea name="p-content-video" class="form-add-input" style="overflow: hidden"></textarea>
                    </div>
                    <button type="button" class="delete-p-content">
                        <i class='bx bx-trash'></i>
                    </button>
                    <hr />
                </div>`;


    $('.content-list').append(str);
})

$('.add-content-btn').click(() => {
    var proID = $('.product-detail #ProductID').val();
    var strContents = '';
    var contentList = $('.p-content-item').toArray();

    contentList.forEach(item => {
        var title = $(item).find('input[name="p-content-title"]').val();
        var content = $(item).find('textarea[name="p-content-main"]').val();
        var img = $(item).find('input[name="p-content-img"]').val();
        var video = $(item).find('textarea[name="p-content-video"]').val();

        video = encodeURIComponent(video);
        strContents += title + '++++++++'
            + content + '++++++++' + img
            + '++++++++' + video + ';;;;;;;;';
    })

    if (strContents.length > 0) {
        showLoading();
        $.ajax({
            type: 'post',
            url: '/admin/products/addproductcontent',
            data: {
                productID: proID,
                contents: strContents
            },
            success: (res) => {
                hideLoading();
                if (res.success) {
                    var interval = setInterval(() => {
                        showUpdateOkForm();
                        clearInterval(interval);
                    }, 620);
                }
            },
            error: () => {  }
        })
    }
})