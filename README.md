[![Crowdin](https://badges.crowdin.net/raft-ua/localized.svg)](https://uk.crowdin.com/project/raft-ua)

Мод, що додає українську мову в гру. Модифікація автоматично створить теку для своїх файлів в `ModData` та буде завантажувати/оновлювати файл перекладу при кожному заході в гру, тому перший запуск проведіть з інтернетом.
> Ви можете допомогти з перекладом на платформі **[Crowdin](https://uk.crowdin.com/project/raft-ua)**
>> Повідомити про помилку можна мені у **Discord** — `damglador`
* * *
#### Що він робить?
* Заміняє шрифт гри на ідентичний, з присутністю кирилиці *(російських символів нема, якщо хто захоче перекласти гру на білоруську - пишіть)*;
* Додає в гру українську мову, файл перекладу при вході в гру окремо завантажує з репозиторію GitHub;
* Додає файл, що записує останню використану мову та відновлює її при повторному запуску мода. Теоретично має працювати і з будь-якими іншими мовами.
#### Як встановити?
1. Встановлюєте **[Raft Mod Loader](https://www.raftmodding.com/download)** (без нього ніяк);
2. Тицяєте кнопку **«Install Mod»** [тут](https://www.raftmodding.com/mods/ukrainian-language), на сайті;
3. Відкриється віконце, де ви підтверджуєте встановлення, натиснувши на зелену кнопку;
4. Заходите в гру через **Raft Mod Loader**, натиснувши **«Play»** (це треба буде робити кожен раз, коли ви захочете грати з перекладом);
5. При першому завантаженні модифікація створить всі потрібні файли, завантажить файл перекладу з репозиторію на GitHub, тому перший запуск проводьте з доступом до інтернету. Надалі переклад буде оновлюватися за наявності інтернету при кожному вході в гру, але навіть якщо у вас не буде інтернету, він буде завантажувати вже встановлену версію перекладу.
* * *
#### Подяки
* Моду [Renamer](https://www.raftmodding.com/mods/renamer) від **funjoker** за основу кода, фундамент для перекладу, хоч я й переписав більшу його частину;
* **Aidanamite** за деяку оптимізацію коду та базу для оптимізації завантажувача шрифтів та код для завантаження TMP шрифтів;
* **amadare** за доведення завантажувача шрифтів до робочого стану.
* Всім, хто перекладав гру на Crowdin
#### Підтримка
Я втюхав на цей мод декілька дні активного кодингу, тому якщо хочете підтримати саме **кодера**, можете поповнити монобанку за цими реквізитами:
* Посилання: [\*тиць\*](https://send.monobank.ua/jar/9fYeh5mu3Y)
* Номер: `5375 4112 0013 5544`
* * *
#### English description
A mod that adds the Ukrainian language to the game. The mod creates all the files it needs and downloads the translation from GitHub when it is first launched, so you should install it with Internet access. Later on, it will try to update the translation every time you launch the game.
#### Credits
* The [Renamer](https://www.raftmodding.com/mods/renamer)  mod by **funjoker** for localization loading code;
* **Aidanamite** for some code optimization & base for font loader optimization, TMP font loader;
* **amadare** for font loader optimization.