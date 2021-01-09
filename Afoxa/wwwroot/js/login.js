'use strict';

$('document').ready(function () {
    $('.warning').hide();
});

function onTelegramAuth(user) {
    $.post('/Account/Login', user)
        .done(function() {
            window.location.replace('/')
        })
        .fail(function () {
            $('.warning').show();
            $('.warning').text('⚠️ Такого користувача не знайдено або доступ заборонено');
        });

}