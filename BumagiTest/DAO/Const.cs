using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class DaoConst
    {
        public const string FIO = "ФИО";
        public const string BirthDay = "Дата рождения(Формат ДД.ММ.ГГГГ)";
        public const string ProgLang = "Любимый язык программирования";
        public const string Experience = "Опыт программирования на указанном языке (Полных лет)";
        public const string Phone = "Мобильный телефон";
        public const string CreateDate = "Анкета заполнена";

        public const int FioId = 0;
        public const int BirthDayId = 1;
        public const int ProgLangId = 2;
        public const int ExperienceId = 3;
        public const int PhoneId = 4;
        

        public static readonly Dictionary<int,string> Questions = new Dictionary<int, string>
        {
            {FioId, FIO},
            {BirthDayId, BirthDay},
            {ProgLangId, ProgLang},
            {ExperienceId, Experience},
            {PhoneId, Phone}
        };

        
       
    }

}
