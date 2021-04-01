# Afoxa
Remote education web-app for telegram user.

## Getting Started

_NOTE_: You need telegram account for authorization and telegram bot

Config file `conf.json` :

    {
      "BotToken": "youBotToken",
      "BotUsername": "youBotName"
    }

Authorization configuration: 

You need a https domen for authorize with telegram login widget: 
- Best to use [ngrok.io](https://ngrok.com/)
- Use coomand: `ngrok http https://localhost:44387 -host-header=localhost:44387`
- When https domen is received you need to specify it as the bot domain address in [@BotFather](https://t.me/BotFather)

If everything is correct then the login widget will be displayed correctly.

Have a questions? Ask [@Kenobi_Knobs](https://t.me/Kenobi_Knobs)
