
//--AJAX get list category --------------------
function reloadCategories() {
   showLoading();
    $.ajax({
        type: 'GET',
        url: '/api/categories',
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.categories-list table tbody').empty();
                $('.categories-list table tbody').append(`<tr>
                            <th>STT</th>
                            <th>Mã danh mục</th>
                            <th>Tên danh mục</th>
                            <th>Hình ảnh</th>
                            <th>Số sản phẩm</th>
                            <th></th>
                        </tr>`);
                var categories = responses.categories;
                for (var i = 0; i < categories.length; i++) {
                    var str = `
                        <tr>
                            <td>${i + 1}</td>
                            <td>${categories[i].CateID}</td>
                            <td>${categories[i].CateName}</td>
                            <td>
                                <img src="${categories[i].ImgSrc != null ? categories[i].ImgSrc : '/images/no-image.jpg'}" alt="" />
                            </td>
                            <td>${categories[i].ProductCount}</td>
                            <td>
                                <div class="categories-button-box d-flex justify-content-end flex-wrap gap-2">
                                    <button class="cate-btn cate-update-btn" data-update-cate="${categories[i].CateID}">Sửa</button>
                                    <button class="cate-btn cate-delete-btn" data-del-cate="${categories[i].CateID}" ${!responses.isAdmin ? "disabled" : "" }>Xóa</button>
                                </div>
                            </td>
                        </tr>
                    `;

                    $('.categories-list table tbody').append(str);
                }
            }
        },
        error: (err) => {
            hideLoading();
            console.log(err)
        }
    })
}

$('.reload-categories').click(() => {
    reloadCategories();
})

//Hide loading --
function hideLoading() {
    var interval = setInterval(() => {
        $('.loading').hide();
        clearInterval(interval);
    }, 600);
}

function showLoading() {
    $('.loading').css('display', 'grid');
}

//---------Add, update, detele category --------------------------------------
//Add category ---------------------------------------------------
$('.add-category-btn').click(() => {
    $('.add-category').css('visibility', 'visible');
    $('.add-category .form-box').addClass('show');
})

$('.close-add-category').click(() => {
    $('.add-category').css('visibility', 'hidden');
    $('.add-category .form-box').removeClass('show');
})

$('.add-category').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.add-category').css('visibility', 'hidden');
        $('.add-category .form-box').removeClass('show');
    }
})

$('.add-category-form').submit((e) => {
    e.preventDefault();
    var cateID = $('#category-id').val();
    var cateName = $('#category-name').val();
    var imgSrc = $('#category-img').val();
   showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/categories/addcategory',
        data: {
            CateID: cateID,
            CateName: cateName,
            ImgSrc: imgSrc
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                checkInputValid($('#category-id'));
                checkInputValid($('#category-name'));
                var interval = setInterval(() => {
                    showOkForm();
                    $('#category-id').val('');
                    $('#category-name').val('');
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.error}
                </span>`;
                $('.add-category .form-error').show();
                $('.add-category .form-error').empty();
                $('.add-category .form-error').append(str);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})

//Delete category ----------------------------------------------------
$('.delete-cate-confirm').click((e) => {
    if ($(e.target).closest('.delete-cate-confirm-box').length <= 0) {
        $('.delete-cate-confirm').css('visibility', 'hidden');
        $('.delete-cate-confirm .delete-cate-confirm-box').removeClass('show');
    }
})
function hideCateNotice() {
    $('.categories-action-notice').hide();
}

function showCateNotice() {
    $('.categories-action-notice').css('display', 'flex');
}

$(document).on('click', '.cate-delete-btn', (e) => {
    var cateID = $(e.target).data('del-cate');

    if (cateID.length > 0) {
        $('.delete-cate-confirm').css('visibility', 'visible');
        $('.delete-cate-confirm-box').addClass('show');
    }

    $('.delete-cate-confirm-no').off('click').click(() => {
        $('.delete-cate-confirm').css('visibility', 'hidden');
        $('.delete-cate-confirm-box').removeClass('show');
    })

    $('.delete-cate-confirm-yes').off('click').click(() => {
        showLoading();
        $.ajax({
            type: 'POST',
            url: '/admin/categories/deletecategory',
            data: {
                CateID: cateID
            },
            success: (response) => {
                hideLoading();
                if (response.success) {
                    $('.delete-cate-confirm').css('visibility', 'hidden');
                    $('.delete-cate-confirm-box').removeClass('show');

                    var interval = setInterval(() => {
                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-delete-notice .complete-notice-box').addClass('showForm');
                        clearInterval(interval);
                    }, 620);
                }
                else {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.err}
                    </span>`
                    $('.categories-action-notice').empty();
                    showCateNotice();
                    $('.categories-action-notice').append(str);
                    $('.delete-cate-confirm').css('visibility', 'hidden');
                    $('.delete-cate-confirm-box').removeClass('show');

                    var interval = setInterval(() => {
                        hideCateNotice();
                        clearInterval(interval);
                    }, 4000);
                }
            },
            error: () => {
                hideLoading();
            }
        })
    })

})

//Update categories ---------------------------------------------
$('#update-category-id').on('keydown', (e) => {
    e.preventDefault();
})

function setCateValue(e) {
    var cateID = $(e.target).data('update-cate');

    $.ajax({
        type: 'get',
        url: '/api/categories',
        data: { CateID: cateID },
        success: (data) => {
            $('#update-category-id').val(data.CateID);
            $('#update-category-name').val(data.CateName);
            $('#update-category-img').val(data.ImgSrc);

            checkInputValid($('#update-category-id'));
            checkInputValid($('#update-category-name'));
            checkInputValid($('#update-category-img'));

            $('.update-category .form-error').hide();
            $('.update-category .form-error').empty();
            $('.update-category .form-submit-btn').prop('disabled', false);
        },
        error: () => {  }
    })
}

$(document).on('click', '.cate-update-btn', (e) => {
    $('.update-category').css('visibility', 'visible');
    $('.update-category .form-box').addClass('show');
    setCateValue(e);
})

$('.close-update-category').click(() => {
    $('.update-category').css('visibility', 'hidden');
    $('.update-category .form-box').removeClass('show');
    $('#update-category-id').val('');
    $('#update-category-name').val('');
    $('#update-category-img').val('');
    $('.update-category .form-submit-btn').prop('disabled', true);
})

$('.update-category').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.update-category').css('visibility', 'hidden');
        $('.update-category .form-box').removeClass('show');
    }
})

$('.update-category-form').submit((e) => {
    e.preventDefault();
    var cateID = $('#update-category-id').val();
    var cateName = $('#update-category-name').val();
    var imgSrc = $('#update-category-img').val();

    showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/categories/updatecategory',
        data: {
            CateID: cateID,
            CateName: cateName,
            ImgSrc: imgSrc
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                var interval = setInterval(() => {
                    showUpdateOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>${response.error}</span>`;
                $('.update-category .form-error').show();
                $('.update-category .form-error').empty();
                $('.update-category .form-error').append(str);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})


//--------Add, update, delete brand --------------------------
//Reload brand list
function reloadBrands() {
    showLoading();
    $.ajax({
        type: 'GET',
        url: '/api/brands',
        success: (responses) => {
            hideLoading();
            if (responses != null) {
                $('.brand-list table tbody').empty();
                $('.brand-list table tbody').append(`<tr>
                            <th>STT</th>
                            <th>Mã hãng</th>
                            <th>Tên hãng</th>
                            <th>Số điện thoại</th>
                            <th>Địa chỉ</th>
                            <th>Hình ảnh</th>
                            <th></th>
                        </tr>`);
                var brands = responses.brands;
                for (var i = 0; i < brands.length; i++) {
                    var str = `
                        <tr>
                            <td>${i + 1}</td>
                            <td>${brands[i].BrandID}</td>
                            <td>${brands[i].BrandName}</td>
                            <td>${brands[i].Phone != null ? brands[i].Phone : ''}</td>
                            <td>${brands[i].BrandAddress != null ? brands[i].BrandAddress : ''}</td>
                            <td>
                                ${brands[i].BrandImgSrc != null ? `<img src="${brands[i].BrandImgSrc}" alt="" />` : ''}
                            </td>
                            <td>
                                <div class="brands-button-box d-flex justify-content-end flex-wrap gap-2">
                                    <button class="cate-btn brand-update-btn" data-update-brand="${brands[i].BrandID}">Sửa</button>
                                    <button class="cate-btn  brand-delete-btn" data-del-brand="${brands[i].BrandID}" ${!responses.isAdmin ? "disabled" : ""}>Xóa</button>
                                </div>
                            </td>
                        </tr>
                    `;

                    $('.brand-list table tbody').append(str);
                }
            }
        },
        error: (err) => {
            hideLoading();
            console.log(err)
        }
    })
}

$('.reload-brands-btn').click(() => {
    reloadBrands();
})

//Add brand --------------------------------

$('.add-brand-btn').click(() => {
    $('.add-brand').css('visibility', 'visible');
    $('.add-brand .form-box').addClass('show');
})

$('.close-add-brand').click(() => {
    $('.add-brand').css('visibility', 'hidden');
    $('.add-brand .form-box').removeClass('show');
})

$('.add-brand').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.add-brand').css('visibility', 'hidden');
        $('.add-brand .form-box').removeClass('show');
    }
})

$('.add-brand-form').submit((e) => {
    e.preventDefault();
    var brandID = $('#brand-id').val();
    var brandName = $('#brand-name').val();
    var brandAddress = $('#brand-address').val();
    var brandPhone = $('#brand-phone').val();
    var imgSrc = $('#brand-img').val();
    showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/brands/addbrand',
        data: {
            BrandID: brandID,
            BrandName: brandName,
            BrandAddress: brandAddress,
            Phone: brandPhone,
            BrandImgSrc: imgSrc
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                checkInputValid($('#brand-id'));
                checkInputValid($('#brand-name'));
                checkInputValid($('#brand-address'));
                checkInputValid($('#brand-phone'));
                checkInputValid($('#brand-img'));
                var interval = setInterval(() => {
                    showOkForm();
                    $('#brand-id').val('');
                    $('#brand-name').val('');
                    $('#brand-address').val('');
                    $('#brand-phone').val('');
                    $('#brand-img').val('');
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.error}
                </span>`;
                $('.add-brand .form-error').show();
                $('.add-brand .form-error').empty();
                $('.add-brand .form-error').append(str);
            }
        },
        error: () => {
            hideLoading();
        }
    })
})


//Update brand -----------------------
$('#update-brand-id').on('keydown', (e) => {
    e.preventDefault();
})

function setBrandValue(e) {
    var brandID = $(e.target).data('update-brand');

    $.ajax({
        type: 'get',
        url: '/api/brands',
        data: { brandID: brandID },
        success: (data) => {
            $('#update-brand-id').val(data.BrandID);
            $('#update-brand-name').val(data.BrandName);
            $('#update-brand-address').val(data.BrandAddress);
            $('#update-brand-phone').val(data.Phone);
            $('#update-brand-img').val(data.BrandImgSrc);

            checkInputValid($('#update-brand-id'));
            checkInputValid($('#update-brand-name'));
            checkInputValid($('#update-brand-address'));
            checkInputValid($('#update-brand-phone'));
            checkInputValid($('#update-brand-img'));

            $('.update-brand .form-error').hide();
            $('.update-brand .form-error').empty();
            $('.update-brand .form-submit-btn').prop('disabled', false);
        },
        error: () => { }
    })
}


$(document).on('click', '.brand-update-btn', (e) => {
    $('.update-brand').css('visibility', 'visible');
    $('.update-brand .form-box').addClass('show');
    setBrandValue(e);
})

$('.close-update-brand').click(() => {
    $('.update-brand').css('visibility', 'hidden');
    $('.update-brand .form-box').removeClass('show');
    $('#update-brand-id').val('');
    $('#update-brand-name').val('');
    $('#update-brand-address').val('');
    $('#update-brand-phone').val('');
    $('#update-brand-img').val('');
    $('.update-brand .form-submit-btn').prop('disabled', true);
})

$('.update-brand').click((e) => {
    if ($(e.target).closest('.form-box').length <= 0) {
        $('.update-brand').css('visibility', 'hidden');
        $('.update-brand .form-box').removeClass('show');
    }
})

$('.update-brand-form').submit((e) => {
    e.preventDefault();
    var brandID = $('#update-brand-id').val();
    var brandName = $('#update-brand-name').val();
    var brandAddress = $('#update-brand-address').val();
    var brandPhone = $('#update-brand-phone').val();
    var imgSrc = $('#update-brand-img').val();

    showLoading();
    $.ajax({
        type: 'POST',
        url: '/admin/brands/updatebrand',
        data: {
            BrandID: brandID,
            BrandName: brandName,
            BrandAddress: brandAddress,
            Phone: brandPhone,
            BrandImgSrc: imgSrc
        },
        success: (response) => {
            hideLoading();
            if (response.success) {
                var interval = setInterval(() => {
                    showUpdateOkForm();
                    clearInterval(interval);
                }, 620);
            }
            else {
                var str = `<span>${response.error}</span>`;
                $('.update-brand .form-error').show();
                $('.update-brand .form-error').empty();
                $('.update-brand .form-error').append(str);
            }
        },
        error: () => {
        }
    })
})

//--Delete brand ------------------------
$(document).on('click', '.brand-delete-btn', (e) => {
    var brandID = $(e.target).data('del-brand');

    if (brandID.length > 0) {
        $('.delete-brand-confirm').css('visibility', 'visible');
        $('.delete-brand-confirm-box').addClass('show');
    }

    $('.delete-brand-confirm-no').off('click').click(() => {
        $('.delete-brand-confirm').css('visibility', 'hidden');
        $('.delete-brand-confirm-box').removeClass('show');
    })

    $('.delete-brand-confirm-yes').off('click').click(() => {
        showLoading();
        $.ajax({
            type: 'POST',
            url: '/admin/brands/deletebrand',
            data: {
                brandID: brandID
            },
            success: (response) => {
                hideLoading();
                if (response.success) {
                    $('.delete-brand-confirm').css('visibility', 'hidden');
                    $('.delete-brand-confirm-box').removeClass('show');

                    var interval = setInterval(() => {
                        $('.complete-delete-notice').css('visibility', 'visible');
                        $('.complete-delete-notice .complete-notice-box').addClass('showForm');
                        clearInterval(interval);
                    }, 620);
                }
                else {
                    var str = `<span>
                    <i class="fa-solid fa-circle-exclamation error-icon"></i>
                    ${response.err}
                    </span>`
                    $('.categories-action-notice').empty();
                    showCateNotice();
                    $('.categories-action-notice').append(str);
                    $('.delete-brand-confirm').css('visibility', 'hidden');
                    $('.delete-brand-confirm-box').removeClass('show');

                    var interval = setInterval(() => {
                        hideCateNotice();
                        clearInterval(interval);
                    }, 4000);
                }
            },
            error: () => {
                hideLoading();
            }
        })
    })

})