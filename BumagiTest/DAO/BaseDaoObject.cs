using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

namespace DAO
{
    public interface IStrRecoverable
    {
        void FromString(string str, string id);
    }

    public abstract class BaseDaoObject<T>
    {
        /// <summary>
        /// является именем файла, куда пишется объект
        /// </summary>
        public T Id { get; set; }
    }
}
