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
            var id = "first";
            var ws = new Worksheet
            {
                Id = id, 
                Detail = new List<WorksheetDetail>
                {
                    new WorksheetDetail
                    {
                        Question = DaoConst.FIO,
                        Answer = "Ivanov Ivan Ivanych"
                    },
                    new WorksheetDetail
                    {
                        Question = DaoConst.BirthDay,
                        Answer = "01.01.1990"
                    },
                    new WorksheetDetail
                    {
                        Question = DaoConst.ProgLang,
                        Answer = CommonConst.ProgLangList[0],
                    },
                    new WorksheetDetail
                    {
                        Question = DaoConst.Experience,
                        Answer = "5"
                    },
                    new WorksheetDetail
                    {
                        Question = DaoConst.Phone,
                        Answer = "+71111111111"
                    },
                },
                CreateDate = DateTime.Today.ToString(),
            };
            wsDao.Put(ws);
            Assert.True(File.Exists(provider.ConnectionString + ws.Id + ".txt"));
            var loadedWs = wsDao.Get("first");
            Assert.NotNull(loadedWs);
            Assert.True(loadedWs.Detail.Count == 5);
            Assert.True(loadedWs.Id  == id);
            var list = wsDao.List();
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

            var phone = "+79020412124";
            b= Regex.IsMatch(phone, CommonConst.RefExpConst.RxPhone);
            Assert.True(b);
        }
    }

}
