'use strict';

document.querySelector('emoji-picker')
    .addEventListener('emoji-click', event => replaceEmoji(event.detail));

$('document').ready(function () {
    let date = new Date();
    let hours = date.getHours();
    if (hours >= 6 && hours <= 11) {
        //Доброго ранку
        $('.time').text('Доброго ранку');
    }
    if (hours >= 12 && hours <= 16) {
        //Доброго Дня
        $('.time').text('Доброго дня')
    }
    if (hours >= 17 && hours <= 21) {
        //Доброго Вечора
        $('.time').text('Доброго вечора')
    }
    if ((hours >= 22 && hours <= 23) || (hours >= 0 && hours <= 5)) {
        //Доброї ночі
        $('.time').text('Доброї ночі')
    }

});

$('.create-btn').click(function () {
    let emojiIcon = $('.emoji').text();
    let name = $('#name').val();
    let about = $('#about').val();

    let course = {
        Name: name,
        About: about,
        Emoji: emojiIcon
    };

    $.post('/Course/CreateOrUpdate', course)
        .done(function (id) {
            addCourse(course, id);
            $('#Modal').modal('toggle');
            $('#name').val('');
            $('#about').val('');
            $('.emoji').text('🐸');
        })
        .fail(function () {
            alert('error');
        });
});

$('body').on('click', '.redirect-button', function () {
    location.href = "/Course/Details/" + this.id;
});

function replaceEmoji(emoji) {
    let unicodeEmoji = emoji.unicode;
    $('.emoji').text(unicodeEmoji);
}

function addCourse(course, id){
    let courseButton = document.createElement('button');
    let courseIcon = document.createElement('div');
    let emoji = document.createElement('span');
    let courseName = document.createElement('p');

    courseButton.id = id;
    courseButton.classList.add('course-card');
    courseButton.classList.add('redirect-button');
    courseIcon.classList.add('course-icon');
    courseName.classList.add('course-name');



    courseName.innerText = course.Name;
    emoji.innerHTML = course.Emoji;

    courseIcon.appendChild(emoji);
    courseButton.appendChild(courseIcon);
    courseButton.appendChild(courseName);

    $('.course-list').append(courseButton);
    $('.course-list').append('<br/>');
    $('.subtext').text('Ось ваші курси:');
}
