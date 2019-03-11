using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using Unity;
using Xunit;

namespace DAO.Xunit
{
    public class UnitTest
    {
        private readonly IUnityContainer Cfg;

        public UnitTest()
        {
            Cfg = new UnityContainer();
            Cfg.RegisterInstance(Cfg).
                DaoContainer();
        }

        [Fact]
        public void ProviderTest()
        {
            var wsDao = Cfg.Resolve<WorksheetDao>();
            var provider = Cfg.Resolve<IProvider>();
            var id = "Иванов Иван Иванович";
            var ws = new Worksheet
            {
                Id = id, 
                Detail = new List<WorksheetDetail>
                {
                    new WorksheetDetail
                    {
                        QuestionId = DaoConst.FioId,
                        Question = DaoConst.FIO,
                        Answer = id,
                    },
                    new WorksheetDetail
                    {
                        QuestionId = DaoConst.BirthDayId,
                        Question = DaoConst.BirthDay,
                        Answer = "01.01.1990"
                    },
                    new WorksheetDetail
                    {
                        QuestionId = DaoConst.ProgLangId,
                        Question = DaoConst.ProgLang,
                        Answer = CommonConst.ProgLangList[0],
                    },
                    new WorksheetDetail
                    {
                        QuestionId = DaoConst.ExperienceId,
                        Question = DaoConst.Experience,
                        Answer = "5"
                    },
                    new WorksheetDetail
                    {
                        QuestionId = DaoConst.PhoneId,
                        Question = DaoConst.Phone,
                        Answer = "+71111111111"
                    },
                },
                CreateDate = DateTime.Today.ToString(),
            };
            wsDao.Put(ws);
            Console.WriteLine(provider.ConnectionString);
            Assert.True(File.Exists(provider.ConnectionString + ws.Id + ".txt"));
            var loadedWs = wsDao.Get(id);
            Assert.NotNull(loadedWs);
            Assert.True(loadedWs.Detail.Count == 5);
            Assert.True(loadedWs.Id  == id);
            var list = wsDao.List(null);
            Assert.True(list.Count == 1 );  
            Assert.True(list[0].Detail.Count == 5);
            Assert.True(list[0].Id == id);
            
        }

        [Fact]
        public void Validate()
        {
            var name = "Иванов Иван Иванович";
            var b = Regex.IsMatch(name, CommonConst.RefExpConst.RxFullFIO);
            Assert.True(b);

            var phone = "+70000000000";
            b= Regex.IsMatch(phone, CommonConst.RefExpConst.RxPhone);
            Assert.True(b);
        }

        [Fact]
        public void Test123()
        {
            Console.WriteLine("dlg;ld;gmfg,");
            int n = 1;
            if (n > 19 || n < 10)
            {
                var last = n % 10;
                if (last == 1) Console.Write("год");
                else if (last == 0 || last >= 5) Console.Write("лет");
                else Console.Write("года");
            }
            else Console.Write("лет");
            //Assert.True(n>0);
        }
    }

}
