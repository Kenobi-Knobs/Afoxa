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

def get_course_keyboard(role, course_id):
    course = types.InlineKeyboardMarkup(row_width=2)
    if role == 'Student':
        lect_btn = types.InlineKeyboardButton(text="🎙 Лекції", callback_data='course_' + str(course_id) + "_lectlist")
        task_btn = types.InlineKeyboardButton(text="📝 Завдання", callback_data='course_' + str(course_id) + "_tasklist")
        teachers_btn = types.InlineKeyboardButton(text="👨‍🏫 Викладачі", callback_data='course_' + str(course_id) + "_teachers")
        rate_btn = types.InlineKeyboardButton(text="‍✅ Оцінки", callback_data='course_' + str(course_id) + "_rate")
        back_btn = types.InlineKeyboardButton(text="‍⬅️ Назад", callback_data="return_to_course_list")
        course.add(lect_btn, task_btn, teachers_btn, rate_btn, back_btn)
    if role == 'Teacher':
        stud_btn = types.InlineKeyboardButton(text="‍🎓 Студенти", callback_data='course_' + str(course_id) + "_students")
        link_btn = types.InlineKeyboardButton(text="🔑 Посилання", callback_data='course_' + str(course_id) + "_link")
        revoke_btn = types.InlineKeyboardButton(text="🔄 Скинути посилання", callback_data='course_' + str(course_id) + "_revoke")
        ads_btn = types.InlineKeyboardButton(text="📢‍ Оголошення", callback_data='course_' + str(course_id) + "_ads")
        back_btn = types.InlineKeyboardButton(text="‍⬅️ Назад", callback_data="return_to_course_list")
        course.add(stud_btn, link_btn, revoke_btn, ads_btn, back_btn)
    return course
