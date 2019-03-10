using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DAO
{
    public class Worksheet : BaseDaoObject<string>, IStrRecoverable
    {
        public List<WorksheetDetail> Detail { get; set; }
        public string CreateDate { get; set; }
        

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            foreach (var item in Detail.OrderBy(x=>x.QuestionId))
            {
                sb.AppendLine($"{item.QuestionId+1}. {item.Question}: {item.Answer}");
            }

            sb.AppendLine("");
            sb.AppendLine($"{DaoConst.CreateDate}: {CreateDate}");
            return sb.ToString();
        }

        public static T FromString<T>(string str) where T : BaseDaoObject<string>
        {
            throw new NotImplementedException();
        }

        public void FromString(string str, string id)
        {
            var chars = new[] {'\r', '\n', ' '};
            try
            {
                Detail = new List<WorksheetDetail>();

                var lines = str.Trim(chars).Split('\r');
                if (lines.Length != 7)
                {
                    return;
                }

                foreach (var pair in DaoConst.Questions)
                {
                    var line = lines.FirstOrDefault(x => x.Contains(pair.Value));
                    if (line.IsNullOrEmpty()) continue;
                    var splitLine = line.Split(':');
                    var splitQ = splitLine[0].Split('.');
                    
                    Detail.Add(new WorksheetDetail
                    {
                        QuestionId = Convert.ToInt32(splitQ[0].Trim(chars))-1,
                        Question =  splitQ[1].Trim(chars),
                        Answer = splitLine[1].Trim(chars),
                    });
                }

                var createDateStr = lines.FirstOrDefault(x => x.Contains(DaoConst.CreateDate));
                if (createDateStr.IsNullOrEmpty()) return;
                CreateDate = createDateStr.Split(':')[1];
                Id = id;
            }
            catch (Exception e)
            {
                //если что-то не распарсилось, то обнуляем все
                //т.к. похоже что кто-то пошарился своими рученками
                Detail = new List<WorksheetDetail>();
                CreateDate = string.Empty;
                Id = string.Empty;
            }
            
        }
    }

    public class WorksheetDetail
    {
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
