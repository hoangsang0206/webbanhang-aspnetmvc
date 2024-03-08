$(document).ready(() => {
    var filterBtn = $('.filter-btn').toArray();
    var filterContents = $('.filter-contents').toArray();
    filterBtn.forEach(item => {
        var _item = $(item);
        var filterHead = _item.children('.filter-head');
        var filterContent = _item.children('.filter-contents');

        filterHead.click(() => {
            filterContents.forEach(item1 => {
                $(item1).removeClass('show');
            });

            $('.side-overlay').addClass('show');
            filterContent.addClass('show');
        });
    });

    $('.side-overlay').click(() => {
        filterContents.forEach(item1 => {
            $(item1).removeClass('show');
        });

        $('.side-overlay').removeClass('show');
    })

    $('.close-filter').click(() => {
        $('.filter-all-dropdown .filter-contents').removeClass('show');
        $('.side-overlay').removeClass('show');
    })
});