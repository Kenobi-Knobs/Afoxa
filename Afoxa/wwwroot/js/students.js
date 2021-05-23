'use strict';

let day = new Date();

$('document').ready(function () {
    $('.date').hide(100);

    $('.time').each(function (index, element) {
        let unix = parseInt($(element).text());
        $(element).text(new Date(unix * 1000).format('dd.mm.yy HH:MM'));
    });
});

$.fn.datepicker.language['ru'] = {
    days: ['Неділя', 'Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П’ятниця','Субота'],
    daysShort: ['Нед', 'Пон', 'Вів', 'Сер', 'Чет', 'П’ят', 'Суб'],
    daysMin: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
    months: ['Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень','Липень','Серпень','Вересень','Жовтень','Листопад','Грудень'],
    monthsShort: ['Січ', 'Лют', 'Бер', 'Кві', 'Тра', 'Чер', 'Лип', 'Сер', 'Вер', 'Жов', 'Лис', 'Гру'],
    today: 'Сьогодні',
    clear: 'Очистити',
    dateFormat: 'dd.mm.yyyy',
    timeFormat: 'hh:ii',
    firstDay: 1
};

$('.date').datepicker({
    timepicker: true,
    language: 'ru',
    minDate: new Date(),
    timeFormat: "hh:ii",
    onSelect: function (fd, d, picker) {
        day = d;
    }
});

$('#isPostMessage').click(function () {
    if ($(this).is(':checked')) {
        $('.date').show(100);
    } else {
        day = new Date();
        $('.date').hide(100);
    }
});

$('.kick-modal').click(function () {
    $('.kick-modal-usr').text($(this).attr('name'));
    let userId = $(this).attr('id');
    $('.kick-modal-usr').attr('id', userId);
});

$('.kick-usr').click(function () {
    let userId = $('.kick-modal-usr').attr('id');
    let courseId = $('.content').attr('id');

    let req = {
        userId: userId,
        courseId: courseId,
    }

    $.post('/Course/Kick', req)
        .done(function () {
            $('.' + userId).remove();
            $('#kickModal').modal('toggle');
        })
        .fail(function () {
            alert('error');
        });
});

$('.create-ad-btn').click(function () {
    let title = $('#inputTitle').val();
    let message = $('#message').val();
    let unix = Math.floor(day.getTime() / 1000);
    let courseId = $('.content').attr('id');
    let isPost = $('#isPostMessage').is(':checked');

    let ad = {
        Title: title,
        Message: message,
        UnixTime: unix,
        CourseId: courseId,
    }

    $.post('/Ad/Create', ad)
        .done(function (id) {
            if (isPost) {
                addToAdsTable(id, ad);
            }
            $('#createAdsModal').modal('toggle');
        })
        .fail(function () {
            alert('error');
        });
});

function addToAdsTable(id, ad) {
    let table = document.getElementById('ads');
    let row = table.insertRow(0);
    let cell1 = row.insertCell(0);
    let cell2 = row.insertCell(1);
    let cell3 = row.insertCell(2);
    cell1.innerHTML = ad.Title;
    cell2.innerHTML = new Date(ad.UnixTime * 1000).format('dd.mm.yy HH:MM');
    cell2.classList.add("time");
    cell3.innerHTML = '❌';
    cell3.id = id;
    cell3.classList.add("delete-ad");
}

$('.revoke-btn').click(function () {
    let courseId = $('.content').attr('id');
    $.post('/Course/Revoke', { Id : courseId })
        .done(function (invite) {
            $('#link').val('https://t.me/MixDevBot?start=' + invite);
            $('#refreshCodeModal').modal('toggle');
        })
        .fail(function () {
            alert('error');
        });
});

$('html').on('click', '.delete-ad', function () {
    let adId = $(this).attr('id');
    let elem = $(this);
    $.post('/Ad/Delete', { Id: adId })
        .done(function () {
            elem.closest("tr").remove();
        })
        .fail(function () {
            alert('error');
        }); 
});

