using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace DAO
{
    public static class ContainerConfig
    {
        public static IUnityContainer DaoContainer(this IUnityContainer cfg)
        {
            cfg.RegisterType<IProvider, Provider>()
                .RegisterType<IWorksheetDao, WorksheetDao>()
                .RegisterType<IQuestionDao, QuestionDao>();
            return cfg;
        }
    }
}
