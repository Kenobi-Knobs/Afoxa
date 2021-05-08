'use strict';

let day = new Date();

$('document').ready(function () {
    $('.remote').hide(100);
    fixCard();
})

$('.is-distant').click(function () {
    if ($(this).is(':checked')) {
        $('.remote').show(100);
    } else {
        $('.remote').hide(100);
        $('.clink').val('');
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

let datapicker = $('.date').datepicker({
    timepicker: true,
    language: 'ru',
    minDate: new Date(),
    timeFormat: "hh:ii",
    multipleDates: false,
    onSelect: function (fd, d, picker) {
        day = d;
    }
}).data('datepicker').selectDate(new Date());

$('#createTaskDate').datepicker({
    language: 'ru',
    minDate: new Date(),
    multipleDates: false,
    onSelect: function (fd, d, picker) {
        day = d;
    }
}).data('datepicker').selectDate(new Date());

$('#editTaskDate').datepicker({
    language: 'ru',
    minDate: new Date(),
    multipleDates: false,
    onSelect: function (fd, d, picker) {
        day = d;
    }
}).data('datepicker').selectDate(new Date());

function createOrUpdateLection(mode, id) {
    let topic = $('#topic'+ mode).val();
    let materialLink = $('#materialLink' + mode).val();
    let unix = Math.floor(day.getTime() / 1000);
    let courseId = $('.content').attr('id');
    let isDistant = $('#isDistant' + mode).is(':checked');
    let conferenceLink = '';
    if (isDistant) {
        conferenceLink = $('#conferenceLink' + mode).val();
    }

    let lection = {
        Id: id,
        Topic: topic,
        MaterialLink: materialLink,
        UnixTime: unix,
        ConferenceLink: conferenceLink,
        CourseId: courseId,
    }

    $.post('/Lection/CreateOrUpdate', lection)
        .done(function (data) {
            $('#' + mode + 'LectionModal').modal('toggle');
            if (mode === 'Create') {
                lection.Id = data;
                console.log(lection);
                addLection(lection);
            }
            else {
                refreshLection(lection);
            }
        })
        .fail(function () {
            alert('error');
        });
}

$('.add-btn').click(function () {
    $('.remote').hide(100);
    $('.clink').val('');
    $('.is-distant').prop('checked', false);
});

$('.create-lection-btn').click(function () {
    createOrUpdateLection('Create', 0)
});

$('.update-lection-btn').click(function () {
    createOrUpdateLection('Edit', this.dataset.id)
});

$('.delete-lection-btn').click(function () {
    let id = this.dataset.id;
    $.post('/Lection/Delete/' + id)
        .done(function () {
            $('#EditLectionModal').modal('toggle');
            let items = document.getElementsByClassName('item');
            let count = 0;
            for (let i = 0; i < items.length; i++) {
                if (items[i].dataset.type === 'lection') {
                    count++;
                    if(items[i].dataset.id === id) {
                        items[i].remove();
                    }
                }
            }
            if (count === 1) {
                let item = document.createElement('div');
                item.classList.add('item');
                item.classList.add('blank-lection');
                let title = document.createElement('span');
                title.classList.add('title');
                title.innerHTML = 'Лекції відсутні.';
                let datetime = document.createElement('span');
                datetime.classList.add('date-time');
                datetime.innerHTML = '🤷‍';
                item.appendChild(title);
                item.appendChild(datetime);
                document.getElementById('lections').appendChild(item);
            }
        })
        .fail(function () {
            alert('error');
        });
});

$('.create-task-btn').click(function () {
    let title = document.getElementById('createTaskTitle').value;
    let link = document.getElementById('createTaskLink').value;
    day.setHours(0);
    day.setMinutes(0);
    day.setSeconds(0);
    let time = Math.floor(day.getTime() / 1000);
    
    let task = {
        Id: 0,
        Topic: title,
        Link: link,
        UnixTime: time,
        CourseId: $('.content').attr('id'),
    };
    $.post('/Task/CreateOrUpdate', task)
        .done(function (data) {
            $('#addTaskModal').modal('toggle');
            task.Id = data;
            addTask(task);
        })
        .fail(function () {
            alert('error');
        });
});

$('.submit-btn').click(function () {
    let comment = document.getElementById('submitComment').value;
    let link = document.getElementById('submitLink').value;
    let time = Math.floor(new Date().getTime() / 1000);

    let submition = {
        Id: 0,
        Link: link,
        Comment: comment,
        UnixTime: time,
        StudentId: this.dataset.userid,
        TaskId: this.dataset.taskid,
        CourseId: $('.content').attr('id'),
    };
    $.post('/Submition/Create', submition)
        .done(function (data) {
            $('#submitTaskModal').modal('toggle');
            let items = document.getElementsByClassName('item');
            for (let i = 0; i < items.length; i++) {
                if (items[i].dataset.id == submition.TaskId && items[i].dataset.type == 'task') {
                    items[i].dataset.submition = 'true';
                }
            }
            fixCard();
        })
        .fail(function () {
            alert('error');
        });
});

$('.update-task-btn').click(function () {
    let title = document.getElementById('editTaskTopic').value;
    let link = document.getElementById('editTaskLink').value;
    let id = this.dataset.id;
    day.setHours(0);
    day.setMinutes(0);
    day.setSeconds(0);
    let time = Math.floor(day.getTime() / 1000);

    let task = {
        Id: id,
        Topic: title,
        Link: link,
        UnixTime: time,
        CourseId: $('.content').attr('id'),
    };
    $.post('/Task/CreateOrUpdate', task)
        .done(function (data) {
            $('#editTaskModal').modal('toggle');
            fefreshTask(task);
        })
        .fail(function () {
            alert('error');
        });
});

$('.delete-task-btn').click(function () {
    let id = this.dataset.id;
    $.post('/Task/Delete/' + id)
        .done(function () {
            $('#editTaskModal').modal('toggle');
            let items = document.getElementsByClassName('item');
            let count = 0;
            for (let i = 0; i < items.length; i++) {
                if (items[i].dataset.type === 'task') {
                    count++;
                    if (items[i].dataset.id === id) {
                        items[i].remove();
                    }
                }
            }
            if (count === 1) {
                let item = document.createElement('div');
                item.classList.add('item');
                item.classList.add('blank-task');
                let title = document.createElement('span');
                title.classList.add('title');
                title.innerHTML = 'Завдання відсутні.';
                let datetime = document.createElement('span');
                datetime.classList.add('date-time');
                datetime.innerHTML = '🤷‍';
                item.appendChild(title);
                item.appendChild(datetime);
                document.getElementById('tasks').appendChild(item);
            }
        })
        .fail(function () {
            alert('error');
        });
});

$('body').on('click', '.item', function () {
    switch (this.dataset.target) {
        case '#EditLectionModal':
            document.getElementById('topicEdit').value = this.getElementsByClassName('title')[0].innerHTML;
            document.getElementById('materialLinkEdit').value = this.dataset.mlink;
            document.getElementsByClassName('update-lection-btn')[0].dataset.id = this.dataset.id;
            document.getElementsByClassName('delete-lection-btn')[0].dataset.id = this.dataset.id;
            $('#dateLectionEdit').datepicker().data('datepicker').selectDate(new Date(parseInt(this.dataset.utime) * 1000));
            if (this.dataset.clink === '') {
                $('.remote').hide(100);
                document.getElementById('isDistantEdit').checked = false;
            } else {
                $('.remote').show(100);
                document.getElementById('isDistantEdit').checked = true;
                document.getElementById('conferenceLinkEdit').value = this.dataset.clink;  
            }
            break;
        case '#showLectionModal':
            let body = document.getElementById('lectionShowBody');
            body.getElementsByClassName('topic')[0].innerText = this.getElementsByClassName('title')[0].innerHTML;
            body.getElementsByClassName('time')[0].innerText = new Date(parseInt(this.dataset.utime) * 1000).format('dd.mm.yy hh:MM');
            body.getElementsByClassName('mlink')[0].href = this.dataset.mlink;
            if (this.dataset.clink !== '') {
                body.getElementsByClassName('clink')[0].href = this.dataset.clink;
                body.getElementsByClassName('clink')[0].innerText = 'Посилання на конференцію';
            } else {
                body.getElementsByClassName('clink')[0].innerText = '';
            }
            break;
        case '#editTaskModal':
            $('#editTaskDate').datepicker().data('datepicker').selectDate(new Date(parseInt(this.dataset.utime) * 1000));
            document.getElementById('editTaskTopic').value = this.getElementsByClassName('title')[0].innerHTML;
            document.getElementById('editTaskLink').value = this.dataset.link;
            document.getElementsByClassName('update-task-btn')[0].dataset.id = this.dataset.id;
            document.getElementsByClassName('delete-task-btn')[0].dataset.id = this.dataset.id;
            break;
        case '#submitTaskModal':
            let submitBody = document.getElementById('submitTaskBody');
            submitBody.getElementsByClassName('topic')[0].innerText = this.getElementsByClassName('title')[0].innerHTML;
            submitBody.getElementsByClassName('tlink')[0].href = this.dataset.link;
            document.getElementsByClassName('submit-btn')[0].dataset.taskid = this.dataset.id;
            document.getElementsByClassName('submit-btn')[0].dataset.userid = this.dataset.userid;
            document.getElementById('submitComment').value = '';
            document.getElementById('submitLink').value = '';
            if (this.dataset.submition === 'true') {
                console.log(document.getElementsByClassName('submit-btn')[0]);
                document.getElementsByClassName('submit-btn')[0].disabled = true;
                document.getElementById('submitLink').disabled = true;
                document.getElementById('submitComment').disabled = true;
            } else {
                document.getElementsByClassName('submit-btn')[0].disabled = false;
                document.getElementById('submitLink').disabled = false;
                document.getElementById('submitComment').disabled = false;
            }
            break;
    }
});

function addTask(task) {
    $('.blank-task').remove();
    let item = document.createElement('div');
    item.classList.add('item');
    let title = document.createElement('span');
    title.classList.add('title');
    let datetime = document.createElement('span');
    datetime.classList.add('date-time');
    item.dataset.toggle = "modal";
    item.dataset.target = "#editTaskModal";
    item.dataset.type = "task"
    item.dataset.utime = task.UnixTime;
    item.dataset.id = task.Id;
    item.dataset.link = task.Link;
    title.innerHTML = task.Topic;
    item.appendChild(title);
    item.appendChild(datetime);
    document.getElementById('tasks').appendChild(item);
    fixCard();
}

function fefreshTask(task) {
    let items = document.getElementsByClassName('item');
    for (let i = 0; i < items.length; i++) {
        if (items[i].dataset.type === 'task' && items[i].dataset.id === task.Id) {
            items[i].dataset.utime = task.UnixTime;
            items[i].dataset.id = task.Id;
            items[i].dataset.link = task.Link;
            items[i].getElementsByClassName('title')[0].innerHTML = task.Topic;
            fixCard();
        }
    }
}

function addLection(lection) {
    $('.blank-lection').remove();
    let item = document.createElement('div');
    item.classList.add('item');
    let title = document.createElement('span');
    title.classList.add('title');
    let datetime = document.createElement('span');
    datetime.classList.add('date-time');
    item.dataset.toggle = "modal";
    item.dataset.target = "#EditLectionModal";
    item.dataset.type = "lection"
    item.dataset.utime = lection.UnixTime;
    item.dataset.id = lection.Id;
    item.dataset.mlink = lection.MaterialLink;
    item.dataset.clink = lection.ConferenceLink;
    title.innerHTML = lection.Topic;
    item.appendChild(title);
    item.appendChild(datetime);
    document.getElementById('lections').appendChild(item);
    fixCard();
}

function refreshLection(lection) {
    let items = document.getElementsByClassName('item');
    for (let i = 0; i < items.length; i++) {
        if (items[i].dataset.type === 'lection' && items[i].dataset.id === lection.Id) {
            items[i].dataset.utime = lection.UnixTime;
            items[i].dataset.id = lection.Id;
            items[i].dataset.mlink = lection.MaterialLink;
            items[i].dataset.clink = lection.ConferenceLink;
            items[i].getElementsByClassName('title')[0].innerHTML = lection.Topic;
            fixCard();
        }
    }
}

function fixCard() {
    let items = document.getElementsByClassName('item');
    for (let i = 0; i < items.length; i++) {
        if (items[i].dataset.type == 'lection') {
            let icon = ' 🔗'
            if (items[i].dataset.clink === '') {
                icon = ' 👀'
            }
            let text = new Date(parseInt(items[i].dataset.utime) * 1000).format('dd.mm.yy hh:MM') + icon;
            items[i].getElementsByClassName('date-time')[0].innerText = text;
        }
        if (items[i].dataset.type == 'task') {
            let text = 'до ' + new Date(parseInt(items[i].dataset.utime) * 1000).format('dd.mm.yy');
            items[i].getElementsByClassName('date-time')[0].innerText = text;
            if (items[i].dataset.submition === 'true') {
                items[i].getElementsByClassName('submited')[0].innerText = '✅';
            }
            if (items[i].dataset.submition === 'false') {
                items[i].getElementsByClassName('submited')[0].innerText = '⏰⁠⁠⁠';
            }
        }
    }
}