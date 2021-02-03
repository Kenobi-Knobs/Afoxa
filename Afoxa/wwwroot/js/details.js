'use strict';

document.querySelector('emoji-picker')
    .addEventListener('emoji-click', event => replaceEmoji(event.detail));

function replaceEmoji(emoji) {
    let unicodeEmoji = emoji.unicode;
    $('.emoji').text(unicodeEmoji);
}

$('.edit-btn').click(function () {
    let emoji = $('.emoji-header').text();
    let name = $('.name').text().trim();
    let about = $('.about').text().trim();

    $('.emoji').text(emoji);
    $('#name').val(name);
    $('#about').val(about);
});

$('.submit-btn').click(function () {
    let id = $('.content').attr('id');
    let emojiIcon = $('.emoji').text();
    let name = $('#name').val();
    let about = $('#about').val();

    let course = {
        Id: id,
        Name: name,
        About: about,
        Emoji: emojiIcon
    };

    $.post('/Course/CreateOrUpdate', course)
        .done(function () {
            $('#Modal').modal('toggle');
            $('.name').text(course.Name);
            $('.about').text(course.About);
            $('.emoji-header').text(course.Emoji);
            $('title').text(course.Emoji + ' ' + course.Name);
            $('.course-emoji').text(course.Emoji);
        })
        .fail(function () {
            alert('error');
        });

});