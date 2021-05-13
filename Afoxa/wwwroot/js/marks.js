'use strict';

$('document').ready(function () {
    fixCard();
    if (document.getElementsByClassName('content')[0].dataset.role == "Teacher") {
        setMarks();
    } else {
        let userId = document.getElementsByClassName('checked-container')[0].id;
        let courseId = document.getElementsByClassName('content')[0].id;
        $.get('/Submition/GetUserSubmitions', { userId: userId, courseId: courseId })
            .done(function (data) {
                let finalMark = 0;
                let counter = 0;
                for (let key in data) {
                    if (data[key].mark !== -1) {
                        let submition = document.createElement('div');
                        submition.classList.add('checked-submition');

                        let date = document.createElement('span');
                        date.classList.add('submition-date');
                        date.innerText = new Date(parseInt(data[key].unixTime) * 1000).format('dd.mm.yy HH:MM');

                        let name = document.createElement('span');
                        name.classList.add('task-name');
                        name.innerText = key

                        let mark = document.createElement('span');
                        mark.classList.add('mark');
                        mark.innerText = data[key].mark;
                        finalMark += data[key].mark;

                        submition.appendChild(date);
                        submition.appendChild(name);
                        submition.appendChild(mark);

                        document.getElementsByClassName('checked-submitions')[0].appendChild(submition);
                        counter++
                    }        
                }
                if (counter == 0) {
                    let filler = document.createElement('div');
                    filler.classList.add('checked-submition');
                    filler.innerText = 'Немає нічогісінько 🤷‍';
                    document.getElementsByClassName('checked-submitions')[0].appendChild(filler);
                } else {
                    let final = document.createElement('div');
                    final.classList.add('final');
                    final.innerHTML = 'Поточна оцінка: <span class="mark" id="finalMark">' + finalMark + '</span>';
                    document.getElementsByClassName('checked-submitions')[0].appendChild(final);
                }
            })
            .fail(function () {
                alert('error');
            });
    }
});

$('.submition-card').click(function () {
    document.getElementById('modalAvatar').src = this.dataset.avatar;
    document.getElementById('modalUsername').innerText = this.dataset.username;
    document.getElementById('modalUsername').href = 'https://t.me/' + this.dataset.username;
    document.getElementById('modalDate').innerText = new Date(parseInt(this.dataset.utime) * 1000).format('dd.mm.yy HH:MM');
    document.getElementById('modalTask').innerText = this.dataset.task;
    document.getElementById('modalLink').href = this.dataset.link;
    document.getElementById('modalComment').innerText = this.dataset.comment;
    document.getElementById('modalMark').value = 0;
    document.getElementById('markBtn').disabled = true;
    document.getElementById('markBtn').dataset.submitionid = this.id;
    document.getElementById('cancelBtn').dataset.submitionid = this.id;
});

$('.user-card').click(function () {
    document.getElementById('userModalAvatar').src = this.dataset.avatar;
    document.getElementById('userModalName').innerText = this.dataset.username;
    document.getElementById('userModalName').href = 'https://t.me/' + this.dataset.username;
    let courseId = document.getElementsByClassName('content')[0].id;
    document.getElementsByClassName('final')[0].hidden = true;
    document.getElementsByClassName('modal-submitions')[0].innerHTML = '';
    let filler = document.createElement('span');
    filler.id = 'modalFiller';
    filler.innerText = 'Секундочку...';
    document.getElementsByClassName('modal-submitions')[0].appendChild(filler);

    $.get('/Submition/GetUserSubmitions', { userId: this.dataset.userid, courseId: courseId })
        .done(function (data) {
            let finalMark = 0;
            let counter = 0;
            for (let key in data) {
                let submition = document.createElement('div');
                submition.classList.add('modal-submition');

                let date = document.createElement('span');
                date.classList.add('submition-date');
                date.innerText = new Date(parseInt(data[key].unixTime) * 1000).format('dd.mm.yy HH:MM');

                let name = document.createElement('span');
                name.classList.add('task-name');
                name.innerText = key

                let mark = document.createElement('span');
                mark.classList.add('mark');
                if (data[key].mark == -1) {
                    mark.innerText = '🤷‍'
                } else {
                    mark.innerText = data[key].mark;
                    finalMark += data[key].mark;
                }

                submition.appendChild(date);
                submition.appendChild(name);
                submition.appendChild(mark);

                document.getElementById('modalFiller').hidden = true;
                document.getElementsByClassName('modal-submitions')[0].appendChild(submition);

                counter++
            }
            if (counter == 0) {
                document.getElementById('modalFiller').hidden = false;
                document.getElementById('modalFiller').innerText = 'Немає нічогісінько 🤷‍';
            } else {
                document.getElementsByClassName('final')[0].hidden = false;
                document.getElementById('finalMark').innerText = finalMark;
            }
        })
        .fail(function () {
            alert('error');
        });
});

$('#markBtn').click(function () {
    let submitionId = this.dataset.submitionid;
    let mark = document.getElementById('modalMark').value;
    let courseId = document.getElementsByClassName('content')[0].id;

    $.post("/Submition/SetMark", { courseId: courseId, submitionId: submitionId, mark: mark })
        .done(function (data) {
            $('#ShowSubmitionModal').modal('toggle');
            let submitions = document.getElementById('submitions');
            for (let i = 0; i < submitions.children.length; i++) {
                if (submitions.children[i].id == submitionId) {
                    submitions.children[i].remove();
                }
            }
            setMarks();
        })
        .fail(function () {
            alert('error');
        });
});

$('#cancelBtn').click(function () {
    let submitionId = this.dataset.submitionid;
    let courseId = document.getElementsByClassName('content')[0].id;

    $.post("/Submition/Cancel", { courseId: courseId, submitionId: submitionId})
        .done(function (data) {
            $('#ShowSubmitionModal').modal('toggle');
            let submitions = document.getElementById('submitions');
            for (let i = 0; i < submitions.children.length; i++) {
                if (submitions.children[i].id == submitionId) {
                    submitions.children[i].remove();
                }
            }
        })
        .fail(function () {
            alert('error');
        });
});

$('#modalMark').change(function () {
    document.getElementById('markBtn').disabled = false;
});

function setMarks() {
    let courseId = document.getElementsByClassName('content')[0].id;

    $.get('/Submition/GetMarks', {courseId: courseId })
        .done(function (data) {
            let userCards = document.getElementsByClassName('user-card');
            for (let i = 0; i < userCards.length; i++) {
                userCards[i].getElementsByClassName('mark')[0].innerText = data[userCards[i].dataset.userid];
            }
        })
        .fail(function () {
            alert('error');
        });
}

function fixCard() {
    let times = document.getElementsByClassName('datetime');
    for (let i = 0; i < times.length; i++) {
        times[i].innerHTML = new Date(parseInt(times[i].dataset.utime) * 1000).format('dd.mm.yy HH:MM');
    }
}