using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public class Question : BaseDaoObject<int>
    {
        public string Text { get; set; }
    }
}
