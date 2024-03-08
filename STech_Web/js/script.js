//Load sub header data --------------------------
$(document).ready(() => {
    $.ajax({
        type: 'GET',
        url: '/api/categories',
        success: (data) => {
            $('.sub-header-item-list').empty();
            for (var i = 0; i < data.categories.length; i++) {
                var str = `<li class="sub-header-item">
                    <a href="/collections/${data.categories[i].CateID}" class="sub-header-link">${data.categories[i].CateName}</a>
                </li>`;

                $('.sub-header-item-list').append(str);
            }
        },
        error: () => {
            console.error("Cannot get category list");
        }
    })
})

//Show overlay when click category button -------------------------------------------
$(".categories-btn").click(() => {
    $(".hidden-menu").toggleClass("showHiddenMenu");

    $(".hidden-menu").click((e) => {
        if ($(e.target).closest('.sidebar').length <= 0) {
            $('.hidden-menu').removeClass("showHiddenMenu");
            $(".overlay").removeClass("showOverlay");
        }      
    });

    $(".overlay").addClass("showOverlay");

    $(".overlay").click(() => {
        $(".hidden-menu").removeClass("showHiddenMenu");
        $(".overlay").removeClass("showOverlay");
    });
});

$(".overlay").click(() => {
    $(".overlay").removeClass("showOverlay");
});


//Show change theme
$('.toogle-theme-btn').click(() => {
    $('.change-theme').toggleClass('show');
})

//Change theme -----------------------------
function changeThemeColor(newColor, btnHoverColor, headerBtnBackground) {
    $('body').css('--primary-color', newColor);
    $('body').css('--primary-color-hover', btnHoverColor);
    $('body').css('--header-btn-background', headerBtnBackground);
}

function saveColorToLocalStorage(color1, color2, headerBtnBackground, themeValue) {
    localStorage.setItem('themeColor', color1);
    localStorage.setItem('btnHoverColor', color2);
    localStorage.setItem('headerBtnBackground', headerBtnBackground);
    localStorage.setItem('themeValue', themeValue);
}

function loadThemeColor() {
    var themeColor = localStorage.getItem('themeColor');
    var btnHoverColor = localStorage.getItem('btnHoverColor');
    var headerBtnBackground = localStorage.getItem('headerBtnBackground');
    var themeValue = localStorage.getItem('themeValue');
    changeThemeColor(themeColor, btnHoverColor, headerBtnBackground);

    //checked theme radio
    $('#' + themeValue).prop('checked', true);
}

$(document).ready(() => {
    loadThemeColor();
})

$('input[name="theme"]').on('change', (e) => {
    var colorValue = $(e.target).val();

    if (colorValue == "theme-1") {
        changeThemeColor('#e30019', '#fba5a5', '#be1529');
        saveColorToLocalStorage('#e30019', '#fba5a5', '#be1529', 'theme-1');
    }
    else if (colorValue == "theme-2") {
        changeThemeColor('var(--primary-color-1)', 'var(--primary-color-1-hover)', 'var(--header-btn-background-1)');
        saveColorToLocalStorage('var(--primary-color-1)', 'var(--primary-color-1-hover)', 'var(--header-btn-background-1)', 'theme-2');
    }
    else if (colorValue == "theme-3") {
        changeThemeColor('var(--primary-color-2)', 'var(--primary-color-2-hover)', 'var(--header-btn-background-2)');
        saveColorToLocalStorage('var(--primary-color-2)', 'var(--primary-color-2-hover)', 'var(--header-btn-background-2)', 'theme-3');
    }
    else if (colorValue == "theme-4") {
        changeThemeColor('var(--primary-color-3)', 'var(--primary-color-3-hover)', 'var(--header-btn-background-3)');
        saveColorToLocalStorage('var(--primary-color-3)', 'var(--primary-color-3-hover)', 'var(--header-btn-background-3)', 'theme-4');
    }
    else if (colorValue == "theme-5") {
        changeThemeColor('var(--primary-color-4)', 'var(--primary-color-4-hover)', 'var(--header-btn-background-4)');
        saveColorToLocalStorage('var(--primary-color-4)', 'var(--primary-color-4-hover)', 'var(--header-btn-background-4)', 'theme-5');
    }
})

//Show/hide web loader
function showWebLoader() {
    $('.webloading').css('display', 'grid');
}

function hideWebLoader() {
    $('.webloading').hide();
}

//Show scroll to top button ----------------------------------------------------------
const scrollTopBtn = document.querySelector(".to-top-btn");

window.onscroll = function () {
    if (document.body.scrollTop > 200 || document.documentElement.scrollTop > 200) {
        scrollTopBtn.classList.add("showToTopBtn");
    } else {
        scrollTopBtn.classList.remove("showToTopBtn");
    }
}

scrollTopBtn.onclick = function () {
    document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
};

//Show sidebar menu ------------------------------------------------------------------------
function showSidebar() {
    $(".mobile-sidebar").addClass("showMobileSidebar");
    $(".overlay").addClass("showOverlay");
    $(".overlay").click(function () {
        $(".mobile-sidebar").removeClass("showMobileSidebar");
    });
};

$(".mobile-categories-btn").click(showSidebar);
$(".bottom-nav-show-sidebar").click(showSidebar);
$(".sidebar-close-btn").click(function () {
    $(".mobile-sidebar").removeClass("showMobileSidebar");
    $(".overlay").removeClass("showOverlay");
});

//Show mobile sidebar menu level 2, 3
var sidebarContents = $('.mobile-sidebar .megamenu-item').toArray();
sidebarContents.forEach(item => {
    var _item = $(item);

    var sidebarChervon = _item.find('.megamenu-item-box .megamenu-chevron');
    sidebarChervon.click(() => {
        _item.children('.megamenu-content').toggleClass('showSidebarContent');
        sidebarChervon.toggleClass('chevronRotate');

        var sidebarContentsLvl2 = _item.children('.megamenu-content').find('.megamenu-content-item').toArray();

        sidebarContentsLvl2.forEach(item1 => {
            var _item1 = $(item1);
            var sidebarChevronLevel2 = _item1.find('.sub-megamenu-box .megamenu-chevron')

            sidebarChevronLevel2.click(() => {
                _item1.children('.megamenu-item-list').toggleClass('showSidebarContent');
                sidebarChevronLevel2.toggleClass('chevronRotate');
            })
        })
    })


})

//Show bottom navigation --------------------------------------------------------------
$(document).ready(function () {
    let preScrollPos = window.scrollY;
    $(window).scroll(() => {
        const currentScrollPos = window.scrollY;
        if (currentScrollPos > preScrollPos) {
            $(".bottom-nav-mobile").addClass("showBottomNav");
        }
        else {
            $(".bottom-nav-mobile").removeClass("showBottomNav");
        }
        preScrollPos = currentScrollPos;
    })
});

//Slick Slider ------------------------------------------------------------------------
$(".sale-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: true,
    autoplay: true,
    autoplaySpeed: 2000,
    arrows: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    dots: true,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                autoplay: false,
                infinite: false,
                arrows: false
            }
        }
    ]
});

//Slick Slider for Collection in main page
$(".collection-slick-slider").slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    infinite: false,
    arrows: true,
    swipeToSlide: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                arrows: false
            }
        }
    ]
})

//Slick Slider for Brands Collection
$(".brand-collections").slick({
    slidesToShow: 7,
    slidesToScroll: 1,
    dots: false,
    infinite: true,
    arrows: false,
    autoplay: true,
    autoplaySpeed: 1500,
    swipeToSlide: true,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 5,
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 3
            }
        }
    ]
})

//Slick slider in product detail page -----------------------
$('.slider-main').slick({
    slidesToShow: 1,
    slidesToScroll: 1,
    arrows: true,
    fade: false,
    infinite: false,
    useTransform: true,
    speed: 400,
    cssEase: 'cubic-bezier(0.77, 0, 0.18, 1)',
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-arrow-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-arrow-right"></i></button>`,
})

$('.slider-nav').on('init', function (event, slick) {
    $('.slider-nav .slick-slide.slick-current').addClass('is-active');
}).slick({
    slidesToShow: 5,
    slidesToScroll: 1,
    dots: false,
    focusOnSelect: false,
    infinite: false,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-arrow-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-arrow-right"></i></button>`,
})

$('.slider-main').on('afterChange', function (event, slick, currentSlide) {
    $('.slider-nav').slick('slickGoTo', currentSlide);
    var currrentNavSlideElem = '.slider-nav .slick-slide[data-slick-index="' + currentSlide + '"]';
    $('.slider-nav .slick-slide.is-active').removeClass('is-active');
    $(currrentNavSlideElem).addClass('is-active');
});

$('.slider-nav').on('click', '.slick-slide', function (event) {
    event.preventDefault();
    var goToSingleSlide = $(this).data('slick-index');

    $('.slider-main').slick('slickGoTo', goToSingleSlide);
});


//------
//Slick Slider for Collection in product detail page
$(".order-product-slick-slider").slick({
    slidesToShow: 6,
    slidesToScroll: 1,
    infinite: false,
    arrows: true,
    swipeToSlide: true,
    prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
    nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
    responsive: [
        {
            breakpoint: 1024,
            settings: {
                slidesToShow: 4,
                arrows: false
            }
        },
        {
            breakpoint: 767,
            settings: {
                slidesToShow: 2.2,
                arrows: false
            }
        }
    ]
})

//Show footer column content -----------------------------------------------------------
var footerColArr = $(".footer-col").toArray();
footerColArr.forEach(item => {
    var _item = $(item);
    _item.children(".footer-title").click(() => {
        _item.toggleClass("activeFooter");
    });
});

//Lazy loading image
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

$(document).ready(lazyLoading);


//$(document).click(function (e) {
//    var target = e.target;
//    while (target && target.tagName !== 'A') {
//        target = target.parentNode;
//    }
//    if (target.getAttribute('href') === '#') {
//        e.preventDefault();
//        window.location.href = "/error/notfound";
//    }
//})


 //---Show order dropdown content---------------------------
$(".sort-dropdown-btn").click(() => {
    $(".sort-dropdown-content").toggleClass("showDropdown");
    $(".sort-dropdown-content").click(() => {
        $(".sort-dropdown-content").removeClass("showDropdown");
    })
})

//----------------------------------------------------------
//---Save search history to Local Storage-------------------
$(document).ready(function () {
    $('.search-form').submit(() => {
        var searchText = $('#search').val();

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        if (searchHistory.length > 13) {
            searchHistory.shift();
        }

        if (searchText.length > 0 || searchText != null) {
            searchHistory.push(searchText);
        }

        localStorage.setItem('searchHistory', JSON.stringify(searchHistory));
    })
})

//Show search width 100% in mobile
$(document).ready(function () {
    $('#search').on('click', () => {
        const windowWidth = $(window).width();
        if (windowWidth < 768) {
            $('.header-box').hide();
            $('.header-btn').hide();
            $('body').css('overflow', 'hidden');
            $('.search').css('width', '100%');
            $('.search').css('transition', 'var(--trans-all-03)');
            $('.close-search').show();
            $('.search-history').show();
        }

        $('.close-search').click(() => {
            $('.header-box').css('display', 'flex');
            $('.header-btn').show();
            $('body').css('overflow', 'scroll');
            $('.search').css('width', '60%');
            $('.close-search').hide();
            $('.ajax-search-autocomplete').hide();
            $('.search-history').hide();
        })

        var searchHistory = JSON.parse(localStorage.getItem('searchHistory')) || [];

        $('.search-history-list').empty();
        if (searchHistory.length > 0) {
            $('.search-history').css('padding', '0 0 1rem 0');
            $('.search-history').children('h4').hide();

            for (var i = searchHistory.length - 1; i >= 0; i--) {
                $('.search-history-list').append(`<a href="/search/${searchHistory[i]}">`
                    + `<li class="search-history-list-item">`
                    + searchHistory[i] + '</li>' + '</a>');
            }
        }
        else {
            $('.search-history').children('h4').show();
        }

        $('.clear-search-history').click(() => {
            $('.search-history-list').empty();
            localStorage.removeItem('searchHistory');
            $('.search-history').css('padding', '1rem');
            $('.search-history').children('h4').show();
        })
    })

})

//---AJAX---------------------------------------------------

//---AJAX autocomplete search----------------------------
var typingTimeOut;
$("#search").keyup(function () {
    clearTimeout(typingTimeOut);

    typingTimeOut = setTimeout(() => {
        var searchText = $(this).val();
        if (searchText.length > 0) {
            $.ajax({
                url: '/api/products',
                type: 'GET',
                data: {
                    proName: searchText
                },
                success: function (responses) {
                    var maxItems = window.innerWidth < 768 ? 25 : 6

                    if (responses == null || responses.length <= 0) {
                        $('.ajax-search-autocomplete').show();
                        $('.ajax-search-items').empty();
                        $('.ajax-search-empty').css('display', 'grid');
                    }
                    else {
                        $('.ajax-search-autocomplete').show();
                        $('.ajax-search-items').empty();
                        $('.ajax-search-empty').hide();
                        for (let i = 0; i <= responses.length && i < maxItems; i++) {
                            const product = responses[i];
                            if (product != null) {
                                const strHTML = `<a href="/product/${product.ProductID}"> 
                                    <div class="ajax-search-item d-flex justify-content-between align-items-center">
                                        <div class="ajax-search-item-info">
                                            <div class="ajax-search-item-name d-flex align-items-center">
                                                <h3>${product.ProductName}</h3>
                                            </div>
                                            <div class="ajax-search-item-price d-flex align-items-center">
                                                <h3>${product.Price.toLocaleString("vi-VN") + 'đ'}</h3>
                                                <h4>${product.Cost > product.Price ? product.Cost.toLocaleString("vi-VN") + 'đ' : ''}</h4>
                                            </div>
                                        </div>
                                        <div class="ajax-search-item-image">
                                            <img src="${product.ImgSrc != null ? product.ImgSrc : '/images/no-image.jpg'}" alt="" />
                                        </div>
                                    </div>
                                </a>`;

                                $('.ajax-search-items').append(strHTML);
                            }
                        }
                    }
                },
                error: () => {
                    $('.ajax-search-autocomplete').hide();
                }
            })
        }
        else {
            $('.ajax-search-autocomplete').hide();
        }
    }, 500)

    $(document).on('click', (e) => {
        if (window.innerWidth > 768) {
            var ajaxSearch = $('.ajax-search-autocomplete');
            if (!$(e.target).closest('.ajax-search-autocomplete').length) {
                ajaxSearch.hide();
            }
        }
    })
})

//-----------------------------------------------------------------------
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

//--Show form ----------------------------------------------------------------
$('.action-login-btn').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.action-register-btn').click(() => {
    $('.register').css('visibility', 'visible');
    $('.register .form-container').addClass('showForm');
})

$('.login-btn .login-link').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.bottom-nav-account').click(() => {
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

//-----

$('.login-info-logout, .account-logout').on('click', () => {
    $('.logout-confirm').css('visibility', 'visible');
    $('.logout-confirm-box').addClass('showLogoutConfirm');
})

$('.logout-confirm').click((e) => {
    if (!(e.target).closest('.logout-confirm-box')) {
        $(e.target).css('visibility', 'hidden');
        $('.logout-confirm-box').removeClass('showLogoutConfirm');
    }
})

$('.logout-confirm-no').click(() => {
    $('.logout-confirm').css('visibility', 'hidden');
    $('.logout-confirm-box').removeClass('showLogoutConfirm');
})

//---
var formArr = [$('.login'), $('.register'), $('.register'), $('.forgot-password'), $('.reset-password')];

formArr.forEach(form => {
    var _form = $(form);
//    _form.on('click', (e) => {
//        if ($(e.target).closest('.form-container').length <= 0) {
//            $(e.target).css('visibility', 'hidden');
//            $('.form-container').removeClass('showForm');
//        }
//    })

    //--CLose form -----
    _form.find('.close-form').click(() => {
        _form.css('visibility', 'hidden');
        _form.find('.form-container').removeClass('showForm');
    })
})

 //------------------------
$('.register-now-link').click(() => {
    $('.login').css('visibility', 'hidden');
    $('.login .form-container').removeClass('showForm');
    $('.register').css('visibility', 'visible');
    $('.register .form-container').addClass('showForm');
    
}) 

$('.login-now-link').click(() => {
    $('.register').css('visibility', 'hidden');
    $('.register .form-container').removeClass('showForm');
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.forgot-password-link').click(() => {
    $('.login').css('visibility', 'hidden');
    $('.login .form-container').removeClass('showForm');
    $('.forgot-password').css('visibility', 'visible');
    $('.forgot-password .form-container').addClass('showForm');
})

$('.back-to-login-link').click(() => {
    $('.forgot-password').css('visibility', 'hidden');
    $('.forgot-password .form-container').removeClass('showForm');
    $('.login').css('visibility', 'visible');
    $('.login .form-container').addClass('showForm');
})

$('.forgot-password-form').on('submit', (e) => {
    e.preventDefault();
    $('.forgot-password').css('visibility', 'hidden');
    $('.forgot-password .form-container').removeClass('showForm');
    $('.reset-password').css('visibility', 'visible');
    $('.reset-password .form-container').addClass('showForm');
})

 //--Sale countdown ------------------------------------
$(document).ready(() => {
    var days, hours, minutes, seconds, totalSeconds;
    function getCountdown() {
        $.ajax({
            type: 'GET',
            url: '/home/countdown',
            dataType: 'json',
            success: (data) => {
                if (data.success) {
                    totalSeconds = data.times.TotalSeconds;
                }
                else {
                    totalSeconds = 0;
                }
            },
            error: () => {
                totalSeconds = 0;
            }
        })
    }

    getCountdown();

    var intervalCd = setInterval(() => {
        if (totalSeconds <= 0) {
            $('.sale').hide();

            $.ajax({
                type: 'post',
                url: '/home/endsale',
                success: () => { },
                error: () => { }
            })

            clearInterval(intervalCd);
        }
        else { 
            days = Math.floor(totalSeconds / (24 * 3600));
            hours = Math.floor((totalSeconds % (24 * 3600)) / 3600);
            minutes = Math.floor((totalSeconds % 3600) / 60);
            seconds = Math.floor(totalSeconds % 60);

            updateCountdown();
        }

        totalSeconds--;
    }, 1000);

    function updateCountdown() {
        $("#countdown-days").text(String(days).padStart(2, "0"));
        $("#countdown-hours").text(String(hours).padStart(2, "0"));
        $("#countdown-minutes").text(String(minutes).padStart(2, "0"));
        $("#countdown-seconds").text(String(seconds).padStart(2, "0"));
    }
    // -------------------------------------
})

