from telebot import types
import configparser

config = configparser.ConfigParser()
config.read("settings.ini")

domen = config["bot"]["domen"]

menu_keyboard = types.InlineKeyboardMarkup(row_width=1)
cabinet_btn = types.InlineKeyboardButton(text="🚪 Вхід в кабінет", url=domen)
courses_btn = types.InlineKeyboardButton(text="🗂 Курси", callback_data='courses')
menu_keyboard.add(cabinet_btn, courses_btn)

def create_course_keyboard(course):
    courses = types.InlineKeyboardMarkup(row_width=2)
    if len(course.keys()) == 0:
        btn = types.InlineKeyboardButton(text='Нічого немає 🤷‍', callback_data='none')
        courses.add(btn)
        return courses
    for name in course.keys():
        btn = types.InlineKeyboardButton(text=name, callback_data='course_' + str(course[name]))
        courses.add(btn)
    return courses


# accept_method_key = types.InlineKeyboardMarkup(row_width=2)
# acces_key_btn = types.InlineKeyboardButton(text="Код доступа", callback_data='acces_key')
# pay_btn = types.InlineKeyboardButton(text="Оплатить 1$⚠️", callback_data='pay')
# cancel_btn = types.InlineKeyboardButton(text="❌ Отмена", callback_data='cancel')
# accept_method_key.add(pay_btn, acces_key_btn, cancel_btn)
#
# universal_cancel_key = types.InlineKeyboardMarkup(row_width=1)
# cancel_btn = types.InlineKeyboardButton(text="❌ Отмена", callback_data='cancel')
# universal_cancel_key.add(cancel_btn)
#
# teacher_menu_key = types.InlineKeyboardMarkup(row_width=1)
# cabinet_btn = types.InlineKeyboardButton(text="🚪 Вход в кабинет", url='https://habrahabr.ru')
# send_notice_btn = types.InlineKeyboardButton(text="📣 Cоздать обьявление", callback_data='send_notice')
# question_btn = types.InlineKeyboardButton(text="❓ Вопросы учеников", callback_data='question')
# complete_tasks_btn = types.InlineKeyboardButton(text="✅ Выполненые задания", callback_data='complete_tasks')
# teacher_menu_key.add(cabinet_btn, send_notice_btn, question_btn, complete_tasks_btn, settings_btn)
