'use strict';

$('document').ready(function () {
    fixCard();
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
});


$('.user-card').click(function () {
    document.getElementById('userModalAvatar').src = this.dataset.avatar;
    document.getElementById('userModalName').innerText = this.dataset.username;
    document.getElementById('userModalName').href = 'https://t.me/' + this.dataset.username;
});

$('#modalMark').change(function () {
    document.getElementById('markBtn').disabled = false;
});

function fixCard() {
    let times = document.getElementsByClassName('datetime');
    for (let i = 0; i < times.length; i++) {
        times[i].innerHTML = new Date(parseInt(times[i].dataset.utime) * 1000).format('dd.mm.yy HH:MM');
    }
}