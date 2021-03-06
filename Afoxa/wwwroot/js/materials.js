'use strict';

let day = new Date();

$('document').ready(function () {
    $('.remote').hide(100);
})

$('#isRemote').click(function () {
    if ($(this).is(':checked')) {
        $('.remote').show(100);
    } else {
        $('.remote').hide(100);
    }
});

$.fn.datepicker.language['ru'] = {
    days: ['Неділя', 'Понеділок', 'Вівторок', 'Середа', 'Четвер', 'П’ятниця', 'Субота'],
    daysShort: ['Нед', 'Пон', 'Вів', 'Сер', 'Чет', 'П’ят', 'Суб'],
    daysMin: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
    months: ['Січень', 'Лютий', 'Березень', 'Квітень', 'Травень', 'Червень', 'Липень', 'Серпень', 'Вересень', 'Жовтень', 'Листопад', 'Грудень'],
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