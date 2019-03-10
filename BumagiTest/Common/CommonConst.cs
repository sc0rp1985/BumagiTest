using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CommonConst
    {
        public class RefExpConst
        {
            public const string RxPhone = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";
            /// <summary>
            /// проверка полностью ФИО
            /// </summary>
            public const string RxFullFIO = @"^([А-ЯЁ][а-яё]+)((-[А-ЯЁ][а-яё]+)|(\s[А-ЯЁ][а-яё]+))?\s[А-ЯЁ][а-яё]+((\s[А-ЯЁ][а-яё]+)|())";
        }

        public static readonly List<string> ProgLangList = new List<string> { "PHP", "JavaScript", "C", "C++", "Java", "C#", "Python", "Ruby" };
    }
}
