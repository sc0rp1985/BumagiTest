using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programm
{
    static class Const
    {
        public const string NewProfile = "-new_profile";
        public const string Help = "-help";
        public const string Exit = "-exit";
        public const string Save = "-save";
        public const string Goto = "-goto_question";
        public const string GotoPrev = "-goto_prev_question";
        public const string Restart = "-restart_profile";
        public const string Find = "-find";
        public const string Delete = "-delete";
        public const string List = "-list";
        public const string ListToday = "-list_today";
        public const string Zip = "-zip";
        public const string Stat = "-statistics";

        public static readonly Dictionary<string,string> CommandDescriptions = new Dictionary<string, string>
        {
            {NewProfile,"Заполнить новую анкету"},
            {Stat, "Показать статистику всех заполненных анкет"},
            {Save, "Сохранить заполненную анкету"},
            {Goto, "Вернуться к указанному вопросу (Команда доступна только при заполнении анкеты, вводится вместо ответа на любой вопрос)"},
            {GotoPrev,"Вернуться к предыдущему вопросу (Команда доступна только при заполнении анкеты, вводится вместо ответа на любой вопрос)"},
            {Restart, "Заполнить анкету заново (Команда доступна только при заполнении анкеты, вводится вместо ответа на любой вопрос)"},
            {Find, "<Имя файла анкеты> - Найти анкету и показать данные анкеты в консоль"},
            {Delete, "<Имя файла анкеты> - Удалить указанную анкету"},
            {List, "Показать список названий файлов всех сохранённых анкет"},
            {ListToday, "Показать список названий файлов всех сохранённых анкет, созданных сегодня"},
            {Zip,"<Имя файла анкеты> <Путь для сохранения архива> - Запаковать указанную анкету в архив и сохранить архив по указанному пути"},
            {Help,"Показать список доступных команд с описанием"},
            {Exit, "Выйти из приложения"}
        };

        public static readonly List<string> EditAvailableCmdList =
            new List<string> {Help, Restart, Save, Goto, GotoPrev,Exit};


    }
}
