using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface IQuestionDao
    {
        List<Question> List();
    }

    public class QuestionDao : IQuestionDao
    {
        public List<Question> List()
        {
            return DaoConst.Questions.Select(pair => new Question
            {
                Id = pair.Key,
                Text = pair.Value,
            }).ToList();
        }
    }
}
