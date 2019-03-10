using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DAO
{
    public interface IProvider
    {
        List<T> List<T>() where T :  IStrRecoverable, new();
        T Get<T>(object objId) where T :  IStrRecoverable, new ();
        void Put<T>(T obj) where T : BaseDaoObject<string>;
        void Delete<T>(string objId) where T : BaseDaoObject<string>;

        string ConnectionString { get;  }
    }

    public class Provider : IProvider
    {
        public void Delete<T>(string objId) where T : BaseDaoObject<string>
        {
            var path = ConnectionString + objId.ToString() + ".txt";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            else
            {
                throw new ApplicationException("Анкета не найдена");
            }
        }

        public string ConnectionString => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\Анкеты\\";

        private string GetStoragePath()
        {
            var path = ConnectionString;
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        


        public List<T> List<T>() where T : IStrRecoverable, new()
        {
            var path = GetStoragePath();

            var dirInfo = new DirectoryInfo(path);
            FileInfo[] files = dirInfo.GetFiles();
            var res = new List<T>();
            foreach (var file in files.Where(x=>Path.GetExtension(x.Name) == ".txt"))
            {
                res.Add(Get<T>(Path.GetFileNameWithoutExtension(file.Name)));
            }

            return res;
        }

        public T Get<T>(object objId) where T : IStrRecoverable, new()
        {
            var path = ConnectionString + objId.ToString() + ".txt";
            if (File.Exists(path))
            {
                using (var sr = new StreamReader(path))
                {
                    var str = sr.ReadToEnd();
                    var obj = new T();
                    obj.FromString(str, objId.ToString());
                    return obj;
                }
            }
            else
            {
                throw new ApplicationException("Анкета не найдена");
            }
        }

        public void Put<T>(T obj) where T : BaseDaoObject<string>
        {
            var path = GetStoragePath();
            using (var sw = new StreamWriter(path+obj.Id+".txt"))
            {
                sw.Write(obj.ToString());
            }
        }

        
    }
}
