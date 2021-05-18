import telebot
import requests
import json
import configparser
from telebot import types
from keyboards import *

config = configparser.ConfigParser()
config.read("settings.ini")

token = config["bot"]["token"]
domen = config["bot"]["domen"]

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

def get_course(course_id, user_id):
    params = {'BotToken': token, 'CourseId': course_id}
    r = requests.get(domen + "/API/GetCourse", params=params)
    if r.status_code == requests.codes.ok:
        return r.json()

def show_course(user, chat_id, course):
    course_user = get_user(user, chat_id)
    text = course['emoji'] + " " + course['name'] + ": "
    print(text)

    if course_user['role'] == 'Student':
        print("Student")
    if course_user['role'] == 'Teacher':
        print("Teacher")



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

    com = c.data.split("_")
    if com[0] == 'course':
        course = get_course(com[1], c.from_user.id)
        show_course(c.from_user, c.message.chat.id, course)
        bot.answer_callback_query(c.id)

try:
    print("> Bot runing succesful!")
    bot.polling(none_stop=True, interval=0)
except:
    print("> connecting...")
    bot.polling(none_stop=True, interval=0)
