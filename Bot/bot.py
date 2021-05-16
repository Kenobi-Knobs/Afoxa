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

@bot.message_handler(commands=['start'])
def start(message):
    params = {'BotToken': token, 'TelegramId': message.from_user.id}
    r = requests.get(url=domen + "/API/GetUser", params=params)
    if r.status_code == requests.codes.ok:
        print(r.json())
    else:
        print("register")

    # com = message.text.split(" ")


    # if len(com) != 1:
    #     params = {'BotToken': token, 'TelegramId': message.from_user.id}
    #     r = requests.get(url=domen + "/API/GetUser", params=params)
    #     if r.status_code == requests.codes.ok:
    #         print('add to course ' + com[1])
    #     else:
    #         print("register")
    # elif len(com) == 1:
    #
    #     print("register")

try:
    bot.polling(none_stop=True, interval=0)
except:
    time.sleep(20)
    print("connecting...")
    bot.polling(none_stop=True, interval=0)
