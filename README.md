# TelegramCaptchaBot

TelegramCaptchaBot - бот для защиты чатов от спаммеров.

Базовые настройки
```
public class AppSettings
{
   /// <summary>
   /// Токен бота
   /// </summary>
   public string BotToken { get; set; }

   /// <summary>
   /// Адрес вебхука
   /// </summary>
   public string WebHookAddress { get; set; }

   /// <summary>
   /// Тип капчи
   /// </summary>
   public string CaptchaType { get; set; } = CaptchaTypes.NUMBER_SELECTION;

   /// <summary>
   /// Время на разгадывание капчи
   /// </summary>
   public TimeSpan ProcessEventTimeout { get; set; } = TimeSpan.FromMinutes(1);
   
   /// <summary>
   /// Время бана
   /// </summary>
   public TimeSpan BanTime { get; set; } = TimeSpan.FromDays(1);
}
```
---
Хотел бы отметить поле CaptchaType.<br> 
На данный момент доступен только один вид капчи:<br>
![alt text](https://image.prntscr.com/image/yMrv3vNiTeWaVwJstsSc0w.png)
