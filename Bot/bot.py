import telebot
import requests
import json
import configparser
from telebot import types
from keyboards import *
import datetime
import time
import schedule
import threading

config = configparser.ConfigParser()
config.read("settings.ini")

token = config["bot"]["token"]
domen = config["bot"]["domen"]
bot_name = config["bot"]["bot_name"]

bot = telebot.TeleBot(token);

def register_user(user, chat_id):
    params = {'id': user.id,'userName': user.first_name,'firstName': user.first_name, 'status': 'Student', 'chatId': chat_id, 'BotToken': token }
    r = requests.post(domen + "/Account/Register", data=params)
    return r.status_code == requests.codes.ok

def get_user(user, chat_id):
    params = {'BotToken': token, 'TelegramId': user.id}
    r = requests.get(domen + "/API/GetUser", params=params)
    if r.status_code == requests.codes.ok:
        return r.json()
    else:
        bot.send_message(chat_id, "Реєструю, секундочку...")
        if register_user(user, chat_id):
            bot.send_message(chat_idd, "Тепер ви зареєстровані як студент та можете ввійти в кабінет")

            return get_user(user, chat_id)
        else:
            bot.send_message(chat_id, "Хмм не можу зареєструвати, зверніться в підтримку")

def add_to_course(data, key):
    params = {'BotToken': token, 'UserId': data['telegramId'], 'Token': key}
    r = requests.post(domen + "/Course/AddStudent", data=params)
    return r.status_code == requests.codes.ok

def get_courses(c):
    params = {'BotToken': token, 'TelegramId': c.from_user.id}
    r = requests.get(domen + "/API/GetCourses", params=params)
    if r.status_code == requests.codes.ok:
        return r.json()

def get_course(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetCourse", params=params)
    if r.status_code == requests.codes.ok:
        return r.json()

def get_course_info(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetCourseInfo", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        result = "Завданнь: " + str(data['tasks']) + "\nЛекцій: " + str(data['lects']) + "\nСтудентів: " + str(data['students'])
        return result

def show_course(user, message, course):
    course_user = get_user(user, message.chat.id)
    info = get_course_info(course['id'])
    text = course['emoji'] + " " + course['name'] + "\n" + info
    keyboard = get_course_keyboard(course_user['role'], course['id'])
    bot.edit_message_text(chat_id=message.chat.id, message_id=message.message_id, text=text, reply_markup=keyboard)

def get_lections(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetLections", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        if len(data) == 0:
            return 0
        else:
            text = "Лекції курсу: \n\n"
            for lect in data:
                timestamp = datetime.datetime.fromtimestamp(lect['unixTime'])
                text += lect['topic'] + "    "+ timestamp.strftime('%H:%M %d.%m.%Y') + "\n"
                text += "[Посилання на матеріали](" +  lect['materialLink'] + ")"
                if lect['conferenceLink'] is None:
                    text += " 👀\n\n"
                else:
                    text += "\n[Посилання на конференцію](" +  lect['conferenceLink'] + ") 🔗\n\n"
            return text

def get_tasks(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetTasks", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        if len(data) == 0:
            return 0
        else:
            text = "Завдання : \n\n"
            for task in data:
                timestamp = datetime.datetime.fromtimestamp(task['unixTime'])
                text += task['topic'] + "    "+ timestamp.strftime('до %d.%m.%Y') + "\n"
                text += "[Посилання на завдання](" +  task['link'] + ")\n\n"
            return text

def get_teachers(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetTeachers", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        text = "Викладачі: \n\n"
        for teacher in data:
            text += teacher['telegramFirstName'] + "\n@" + teacher['telegramUserName'] + "\n\n"
        return text

def get_students(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetStudents", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        if len(data) == 0:
            return 0
        else:
            text = "Студенти: \n\n"
            counter = 1
            for student in data:
                text += str(counter) + ") " + student['telegramFirstName'] + " ( @" + student['telegramUserName'] + " )\n\n"
            return text

def get_marked_submition(course_id, user_id):
    params = {'BotToken': token, 'CourseId': course_id, 'UserId': user_id}
    r = requests.get(domen + "/API/GetUserSubmitions", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        if len(data.keys()) == 0:
            return 0
        else:
            text = "Оцінені завдання:\n"
            finalmark = 0
            for key in data.keys():
                t = requests.get(domen + "/Task/Get", params={"Id": int(key)})
                task = t.json();
                if data[key]['mark'] >= 0:
                    timestamp = datetime.datetime.fromtimestamp(data[key]['unixTime'])
                    text += "["+ task['topic'] +"](" +  task['link'] + ")     " + timestamp.strftime('%H:%M %d.%m.%Y') + "\n"
                    text += "[Відповідь](" +  data[key]['link'] + ")     Оцінка: " + str(data[key]['mark'])  + "\n\n"
                    finalmark += data[key]['mark']
            if finalmark >= 0:
                text += "Поточна оцінка: " + str(finalmark)
            return text

def get_ads(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetAds", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        if len(data) == 0:
            return 0
        else:
            text = "Заплановані оголошення:\n\n"
            for ads in data:
                timestamp = datetime.datetime.fromtimestamp(ads['unixTime'])
                text += ads['title'] + "       " + timestamp.strftime('%H:%M %d.%m.%Y') + "\n\n"
            return text

def revoke_invite(course_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/RevokeInvite", params=params)
    return r.status_code == requests.codes.ok

def accept_sending(ad_id):
    params = {'BotToken': token, 'AdId': ad_id}
    requests.get(domen + "/API/AcceptSending", params=params)

def send_notifications():
    params = {'BotToken': token}
    r = requests.get(domen + "/API/GetNotifications", params=params)
    if r.status_code == requests.codes.ok:
        data = r.json()
        for ad in data:
            course = get_course(ad['courseId'])
            text = '📢 Оголошення ( ' + course['emoji'] + " " + course['name'] + ' )\n\n'
            text += ad['title'] + "\n\n"
            text += ad['message']
            params = {'BotToken': token, 'CourseId': ad['courseId']}
            s = requests.get(domen + "/API/GetStudents", params=params)
            for student in s.json():
                bot.send_message(student['telegramChatId'], text)
            accept_sending(ad['id'])
            print("ad sending")
    else:
        print("[Warning!] Notification module error")

@bot.message_handler(commands=['start'])
def start(message):
    user_data = get_user(message.from_user, message.chat.id)
    com = message.text.split(" ")
    if len(com) > 1:
        if add_to_course(user_data, com[1]):
            bot.send_message(message.chat.id, "Ви успішно додані до курсу, можете перейти до нього в кабінеті, або викликавши /menu")
        else:
            bot.send_message(message.chat.id, "Нажаль немає доступу до цього курсу, перевірте посилання")
    else:
        menu(message)

@bot.message_handler(commands=['menu'])
def menu(message):
    get_user(message.from_user, message.chat.id)
    bot.send_message(message.chat.id, "Ось що ви можете:", reply_markup = menu_keyboard)

@bot.callback_query_handler(func=lambda call: True)
def callback_inline(c):
    if c.data == 'courses':
        courses = get_courses(c)
        keyboard = create_course_keyboard(courses)
        bot.send_message(c.message.chat.id, "Ваші курси:", reply_markup = keyboard)
        bot.answer_callback_query(c.id)

    if c.data == 'none':
        bot.answer_callback_query(c.id)

    if c.data == 'return_to_course_list':
        courses = get_courses(c)
        keyboard = create_course_keyboard(courses)
        bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text="Ваші курси", reply_markup=keyboard)
        bot.answer_callback_query(c.id)

    com = c.data.split("_")

    if len(com) == 2:
        if com[0] == "course":
            course = get_course(com[1])
            if course is None:
                bot.send_message(c.message.chat.id, "Щось пішло не так(")
                bot.answer_callback_query(c.id)
            else:
                show_course(c.from_user, c.message, course)
                bot.answer_callback_query(c.id)

    if len(com) == 3:
        keyboard = types.InlineKeyboardMarkup(row_width=1)
        back_btn = types.InlineKeyboardButton(text="‍⬅️ Назад", callback_data="course_" + com[1])
        if com[2] == "lectlist":
            text = get_lections(com[1])
            if text == 0:
                bot.answer_callback_query(c.id, show_alert=True, text="Лекцій немає 🤷")
            else:
                keyboard.add(back_btn)
                bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, parse_mode="Markdown", text=text, reply_markup=keyboard, disable_web_page_preview=True)
                bot.answer_callback_query(c.id)

        if com[2] == "tasklist":
            text = get_tasks(com[1])
            if text == 0:
                bot.answer_callback_query(c.id, show_alert=True, text="Завданнь немає 🤷")
            else:
                keyboard.add(back_btn)
                bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, parse_mode="Markdown", text=text, reply_markup=keyboard, disable_web_page_preview=True)
                bot.answer_callback_query(c.id)

        if com[2] == "teachers":
            text = get_teachers(com[1])
            keyboard.add(back_btn)
            bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text=text, reply_markup=keyboard, disable_web_page_preview=True)
            bot.answer_callback_query(c.id)

        if com[2] == "rate":
            text = get_marked_submition(com[1], c.from_user.id)
            if text == 0:
                bot.answer_callback_query(c.id, show_alert=True, text="Немає результатів 🤷")
            else:
                keyboard.add(back_btn)
                bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text=text, parse_mode="Markdown", reply_markup=keyboard, disable_web_page_preview=True)
                bot.answer_callback_query(c.id)

        if com[2] == "students":
            text = get_students(com[1])
            if text == 0:
                bot.answer_callback_query(c.id, show_alert=True, text="Немає студентів, запросіть їх за допомогою посилання 🤷")
            else:
                keyboard.add(back_btn)
                bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text=text, parse_mode="Markdown", reply_markup=keyboard, disable_web_page_preview=True)
                bot.answer_callback_query(c.id)

        if com[2] == "link":
            course = get_course(com[1])
            keyboard.add(back_btn)
            text = "Посилання для запрошення:\nhttps://t.me/" + bot_name + "?start=" + course['invite']
            bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text=text, reply_markup=keyboard, disable_web_page_preview=True)
            bot.answer_callback_query(c.id)

        if com[2] == "revoke":
            if revoke_invite(com[1]):
                bot.answer_callback_query(c.id, show_alert=True, text="Посилання скинуто, старе запрошення більше не дійсне")

        if com[2] == "ads":
            text = get_ads(com[1])
            if text == 0:
                bot.answer_callback_query(c.id, show_alert=True, text="Немає запланованих оголошеннь 🤷")
            else:
                keyboard.add(back_btn)
                bot.edit_message_text(chat_id=c.message.chat.id, message_id=c.message.message_id, text=text, parse_mode="Markdown", reply_markup=keyboard, disable_web_page_preview=True)
                bot.answer_callback_query(c.id)

def shed():
    schedule.every(60).seconds.do(send_notifications)
    while True:
        schedule.run_pending()
        time.sleep(1)

shed_thread = threading.Thread(target=shed)
shed_thread.start()

try:
    print("> Bot runing succesful!")
    bot.polling(none_stop=True, interval=0)
except:
    print("> connecting...")
    bot.polling(none_stop=True, interval=0)
