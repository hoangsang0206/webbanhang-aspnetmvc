//Lấy thời gian và thời tiết hiện tại -----------------------------------------
function updateCurrentTime() {
    var currentTime = new Date();
    var date = currentTime.toLocaleDateString('en-GB');
    var time = currentTime.toLocaleTimeString('en-US');

    var daysOfWeek = ['Chủ Nhật', 'Thứ Hai', 'Thứ Ba', 'Thứ Tư', 'Thứ Năm', 'Thứ Sáu', 'Thứ Bảy'];
    var day = daysOfWeek[currentTime.getDay()];

    $('.date').text(day + ', ' + date);
    $('.clock').text(time);
}

function getWeatherIcon(description) {
    switch (description.toLowerCase()) {
        case 'clear sky':
            return `<div class="sunny"><div class="inner"></div></div>`;
        case 'few clouds':
            return `<div class="mostlysunny"><div class="inner"></div></div>`;
        case 'scattered clouds':
            return `<div class="mostlycloudy"><div class="inner"></div></div>`;
        case 'broken clouds':
            return `<div class="mostlysunny"><div class="inner"></div></div>`;
        case 'overcast clouds':
            return `<div class="cloudy"><div class="inner"></div></div>`;
        case 'light rain':
            return `<div class="rain"><div class="inner"></div></div>`;
        case 'shower rain':
            return `<div class="rain"><div class="inner"></div></div>`;
        case 'rain':
            return `<div class="rain"><div class="inner"></div></div>`;
        case 'thunderstorm':
            return `<div class="tstorms"><div class="inner"></div></div>`;
        case 'snow':
            return `<div class="snow"><div class="inner"></div></div>`;
        default:
            return '';
    }
}

$(document).ready(() => {
    setInterval(updateCurrentTime, 1000);

    var apiKey = '0292e39f8b40834fb7a306a3a3430ca4';
    var city = 'Ho Chi Minh';
    var apiUrl = 'https://api.openweathermap.org/data/2.5/weather?q=' + city + '&appid=' + apiKey;

    $.ajax({
        url: apiUrl,
        method: 'get',
        success: (data) => {
            $('.temperature').text(Math.floor(data.main.temp - 273.15) + ' °C');
            var str = getWeatherIcon(data.weather[0].description);
            $('.weatherIcon').append(str);
            $('.city').text(data.name);
            console.log(data.weather[0].description)
        },
        error: () => { console.log('Cannot get weather data'); }
    })
})

//Đếm số đơn hàng mới
$(document).ready(() => {
    $.ajax({
        type: 'get',
        url: '/admin/orders/countneworder',
        success: (data) => {
            if (data.count > 0) {
                $('.new-order-count').css('display', 'grid');
                $('.new-order-count').text(data.count);
            }
            else {
                $('.new-order-count').hide();
            }
        },
        error: () => { }
    })
})
