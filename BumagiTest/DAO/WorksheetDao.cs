using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Unity;


namespace DAO
{
    public interface IWorksheetDao
    {
        Worksheet Get(string wId);
        List<Worksheet> List(WorksheetQuery query);
        void Put(Worksheet w);
        void Delete(string wId);
    }

    public class WorksheetQuery
    {
        public DateTime? Date { get; set; } 
    }

    public class WorksheetDao : IWorksheetDao
    {
        [Dependency]
        public IProvider Provider { get; set; }


        public Worksheet Get(string wId)
        {
            return Provider.Get<Worksheet>(wId);
        }

        public List<Worksheet> List(WorksheetQuery query)
        {
            var list = Provider.List<Worksheet>();
            return query?.Date != null ? list.Where(x => x.CreateDate.AsDate() == query.Date).ToList() : list;
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
