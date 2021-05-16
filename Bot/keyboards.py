from telebot import types

hello_keyboard = types.InlineKeyboardMarkup()
be_teacher_btn = types.InlineKeyboardButton(text="🧑‍🏫 Стать преподавателем", callback_data='be_teacher')
hello_keyboard.add(be_teacher_btn)

user_menu_key = types.InlineKeyboardMarkup(row_width=1)
material_btn = types.InlineKeyboardButton(text="📚 Материалы курса", callback_data='material')
task_btn = types.InlineKeyboardButton(text="🗒 Задания", callback_data='task')
perf_btn = types.InlineKeyboardButton(text="🥇 Успеваемость", callback_data='perf')
write_teacher_btn = types.InlineKeyboardButton(text="✏️ Написать преподавателю", callback_data='write_teacher')
courses_btn = types.InlineKeyboardButton(text="🗂 Мои курсы", callback_data='courses')
settings_btn = types.InlineKeyboardButton(text="⚙️ Настройки", callback_data='settings')
user_menu_key.add(material_btn,task_btn, perf_btn, write_teacher_btn, courses_btn, settings_btn)

accept_method_key = types.InlineKeyboardMarkup(row_width=2)
acces_key_btn = types.InlineKeyboardButton(text="Код доступа", callback_data='acces_key')
pay_btn = types.InlineKeyboardButton(text="Оплатить 1$⚠️", callback_data='pay')
cancel_btn = types.InlineKeyboardButton(text="❌ Отмена", callback_data='cancel')
accept_method_key.add(pay_btn, acces_key_btn, cancel_btn)

universal_cancel_key = types.InlineKeyboardMarkup(row_width=1)
cancel_btn = types.InlineKeyboardButton(text="❌ Отмена", callback_data='cancel')
universal_cancel_key.add(cancel_btn)

teacher_menu_key = types.InlineKeyboardMarkup(row_width=1)
cabinet_btn = types.InlineKeyboardButton(text="🚪 Вход в кабинет", url='https://habrahabr.ru')
send_notice_btn = types.InlineKeyboardButton(text="📣 Cоздать обьявление", callback_data='send_notice')
question_btn = types.InlineKeyboardButton(text="❓ Вопросы учеников", callback_data='question')
complete_tasks_btn = types.InlineKeyboardButton(text="✅ Выполненые задания", callback_data='complete_tasks')
teacher_menu_key.add(cabinet_btn, send_notice_btn, question_btn, complete_tasks_btn, settings_btn)
