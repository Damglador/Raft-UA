Allows you to add new languages to the game. You can stack couple of this mod and it will add multiple languages.
[![Imgur](https://i.imgur.com/0uKK1F6.png)](https://i.imgur.com/0uKK1F6.png)
###### To load translation you have to:
1. Download this mod
2. Take its file (TranslationTemplate.rmod) and replace .rmod with .zip so u can open it with windows zip explorer 
3. Create your mod via RML 
4. Go back to zip or unpacked folder, open TranslationTemplate.cs and copy code to your mod
5. Change name of class from `TranslationTemplate` to your mod name
5. Place Loc.csv in folder of your mod where modinfo.json located
***
> To get Loc.csv you can download [Renamer](https://www.raftmodding.com/mods/renamer) and use console command ExportLocalizationToCSV. 
> Downloaded file will be located in `...Steam\steamapps\common\Raft\mods\ModData\Renamer\NameOfTheFile.csv`.
> After you get file you must rename it to `Loc.csv` and delete all columns except `Key` `Type` `Desc` `English`. You can tho delete English, but it would be hard to translate without it.
> Create column for your language and write in first row `Whatever you want [languageKey]`
> `Whatever you want` - will appears as language name in game menu
> `languageKey` - you must get it in https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes for your language
> It should look like this: `Беларуская [be]`
> For editing columns easily you can use VSCode with extension [Edit csv](https://marketplace.visualstudio.com/items?itemName=janisdd.vscode-edit-csv) or web version of Edit csv [here](https://edit-csv.net). I only used VSCode version.
> And after you done with `Loc.csv` file you can put it into mod folder. Put mod author (yourself) and description in `modinfo.json` and compile mod using `build.bat`.
Everything should look something like this:
<details>
<summary>(Click here to see huge images)</summary>
[![here](https://imgur.com/D1PcGJw.png)](https://imgur.com/D1PcGJw.png)
****
[![here](https://imgur.com/maosWq4.png)](https://imgur.com/maosWq4.png)
****
[![here](https://imgur.com/2OVl3E3.png)](https://imgur.com/2OVl3E3.png)
****
[![here](https://imgur.com/bee6nuG.png)](https://imgur.com/bee6nuG.png)
</details>
Be sure to save everything before compiling!
I hope my explanation isn't too bad. Contact with me in Discord - `damglador` if you need help. Or ask someone on Raft Modding Discord server.



There is also a possibility to add normal fonts to the translation, but you need to have someone create them.
You can do something with fonts that's already in game using lines in translation file:
* Fonts/CalibriTMP
* Fonts/Calibrib
* Fonts/ChineseRocks
* Fonts/monorama-medium
* Fonts/tt_marks_rough_bold