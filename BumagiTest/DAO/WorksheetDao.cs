using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;


namespace DAO
{
    public interface IWorksheetDao
    {
        Worksheet Get(string wId);
        List<Worksheet> List();
        void Put(Worksheet w);
        void Delete(string wId);
    }

    public class WorksheetDao : IWorksheetDao
    {
        [Dependency]
        public IProvider Provider { get; set; }


        public Worksheet Get(string wId)
        {
            return Provider.Get<Worksheet>(wId);
        }

        public List<Worksheet> List()
        {
            return Provider.List<Worksheet>();
        }

        public void Put(Worksheet w)
        {
            Provider.Put(w);
        }

        public void Delete(string wId)
        {
            Provider.Delete<Worksheet>(wId);
        }
    }
}
