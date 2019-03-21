using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common;
using DAO;
using ICSharpCode.SharpZipLib.Zip;
using Unity;

namespace Programm
{
    class Program
    {
        static IUnityContainer cfg;
        protected static IUnityContainer Cfg => cfg;

        static void Main(string[] args)
        {
            cfg = new UnityContainer();
            cfg.RegisterInstance(cfg)
                .DaoContainer();
           
            var wsDao = cfg.Resolve<IWorksheetDao>();
            Console.WriteLine("Для продолжения введите требуемую команду");
            Worksheet ws = null;

            var isEditMode = false;
            while (true)
            {
               // var cmd = Console.ReadLine()?.Trim().ToLower();

                var input = Console.ReadLine()?.Trim().ToLower();
                if (input.IsNullOrEmpty())
                    continue;
                var isCmd = input[0] == '-';

                if (isCmd)
                {
                    var splitcmd = input.Split(new[] {' '});
                    /*if (splitcmd.Length > 2)
                        Console.WriteLine("Не поддерживаемая команда");*/
                    var cmd = splitcmd[0];
                    var wsName = splitcmd.Length >= 4 ? splitcmd[1]+" "+splitcmd[2] + " " +splitcmd[3]: string.Empty;
                    var zipPath = wsName.IsNullOrEmpty() ? string.Empty : input.Substring(input.IndexOf(wsName) + wsName.Length);
                    try
                    {
                        switch (cmd)
                        {
                            case Const.NewProfile:
                                Edit();
                                break;
                            case Const.Help:
                                ShowHelp();
                                break;
                            case Const.List:
                                ShowList(wsDao, null);
                                break;
                            case Const.ListToday:
                                ShowList(wsDao, DateTime.Today);
                                break;
                            case Const.Exit:
                                Exit();
                                break;
                            case Const.Find:
                                Find(wsName, wsDao);
                                break;
                            case Const.Delete:
                                Delete(wsName, wsDao);
                                break;
                            case Const.Zip:
                                Zip(wsName, zipPath, wsDao);
                                break;
                            case Const.Stat:
                                ShowStat(wsDao);
                                break;
                            default:
                                Console.WriteLine(
                                    $"Команда не распознана. введите {Const.Help} для просмотра поддерживаемых команд");
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }
            }
        }

        private static void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }

        static void ShowHelp()
        {
            foreach (var pair in Const.CommandDescriptions)
            {
                Console.WriteLine($"{pair.Key} - {pair.Value}");
            }
        }

        static void ShowList(IWorksheetDao wsDao, DateTime? date)
        {
            var list = wsDao.List(new WorksheetQuery{Date = date});
            foreach (var ws in list)
            {
                Console.WriteLine(ws.Id);
            }
        }

        static void Save(Worksheet ws, IWorksheetDao wsDao )
        {
            var d = ws.Detail.First(q => q.QuestionId == DaoConst.FioId);
            ws.Id = d.Answer;
            try
            {
                wsDao.Put(ws);
                Console.WriteLine("сохранено");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        static void Find(string fileName,IWorksheetDao wsDao)
        {
            try
            {
                var ws = wsDao.Get(fileName);
                if (ws != null)
                {
                    Console.WriteLine(ws.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }
        static void Delete(string fileName,IWorksheetDao wsDao)
        {
            try
            {
                wsDao.Delete(fileName);
                Console.WriteLine("Анкета удалена");
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }
        static void Zip(string wsName,string zipPath, IWorksheetDao wsDao)
        {
            try
            {
                var ws = wsDao.Get(wsName);
                if (!Directory.Exists(zipPath))
                {
                    Directory.CreateDirectory(zipPath);
                }
                using (var zip = ZipFile.Create(zipPath+"\\"+wsName+".zip"))
                {
                    zip.BeginUpdate();
                    var tmpFile = Path.GetTempPath()+wsName+".txt";
                    using (var sw = new StreamWriter(tmpFile))
                    {
                        sw.Write(ws.ToString());
                    }
                    zip.Add(tmpFile, Path.GetFileName(tmpFile));
                    zip.CommitUpdate();
                }
                    
                Console.WriteLine("Анкета заархивирована");
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
        }

        static void ShowStat(IWorksheetDao wsDao)
        {
            var list = wsDao.List(null);
            if (list.IsNullOrEmpty())
            {
                Console.Write("Список анкет пуст");
                return;
            }

            var bdList = list.Select(x =>
                x.Detail.Where(q => q.QuestionId == DaoConst.BirthDayId && q.AsDate.HasValue).Select(a => a.AsDate).FirstOrDefault()).Where(x => x.HasValue).Select(x=>x.Value).ToList();
            var avgDays = bdList.Select(x => (DateTime.Today - x).Days).Average();
            var yers = (int)avgDays / 365;
            Console.WriteLine($"Средний возраст всех опрошенных: {yers} {GetYersWord(yers)}");


            var expStrList = list.Select(x =>
                x.Detail.Where(q => q.QuestionId == DaoConst.ExperienceId && q.AsInt.HasValue).Select(a => a.AsInt).FirstOrDefault()).Where(x=>x.HasValue).ToList();
            var maxExp = expStrList.Max();

            var plGroupList = list.Select(x => new
            {
                Id = x.Id,
                ProgLang = x.Detail.Where(q => q.QuestionId == DaoConst.ProgLangId).Select(a => a.Answer).First(),
            }).GroupBy(g => g.ProgLang).Select(t=> new
            {
                ProgLang = t.Key,
                Qty = t.Count()
            }).ToList();

            var qty = plGroupList.Select(x => x.Qty).Max();
            var maxPlName = plGroupList.Where(x => x.Qty == qty).Select(p => p.ProgLang).ToList();
            Console.WriteLine($"Самый популярный язык программирования: {maxPlName.Aggregate((p, n) => p + ", " + n)}");
            var tmp = list.Where(x => x.Detail.Any(q => q.QuestionId == DaoConst.ExperienceId && q.AsInt == maxExp))
                .Select(x => x.Id).ToList();
            Console.WriteLine($"Самый опытный программист: {tmp.Aggregate((p,n)=>p+", "+n)} {maxExp} {GetYersWord(maxExp.Value)}");
        }

        static string GetYersWord(int years)
        {
            var s = string.Empty;
            var n = years % 100;
            if (n > 19 || n < 10)
            {
                var last = n % 10;
                if (last == 1) s = "год";
                else if (last == 0 || last >= 5) s = "лет";
                else s = "года";
            }
            else s = "лет";

            return s;
        }

        static void Edit()
        {
            var qDao = Cfg.Resolve<IQuestionDao>();
            var wsDao = Cfg.Resolve<IWorksheetDao>();
            var qList = qDao.List();
            var currentQuestionItem = 0;
            var ws = new Worksheet
            {
                CreateDate = DateTime.Today.ToString("dd.MM.yyyy"),
                Detail = new List<WorksheetDetail>(),
            };
            var wsSaved = false;
            while (true)
            {
                if (currentQuestionItem > qList.Count - 1)
                {
                    if (ws.Detail.Count ==qList.Count)
                        Console.WriteLine(
                            "Анкета заполнена. Сохраните анкету или вернитесь к нужному вопросу для редактирования");
                    else
                    {
                        var requiredQuestions = new List<int>();
                        var str = ValidateWS(ws, qList,requiredQuestions);
                        if (!str.IsNullOrEmpty())
                        {
                            Console.WriteLine(str);
                            currentQuestionItem = requiredQuestions[0];
                            continue;
                        }
                    }
                }
                else
                {
                    var q = qList[currentQuestionItem];
                    Console.WriteLine($"{currentQuestionItem + 1}. {q.Text}");
                }

                var input = Console.ReadLine().Trim();
                if (input.IsNullOrEmpty())
                    continue;
                var isCmd = input[0] == '-';
                
                if (isCmd)
                {
                    var splitcmd = input.Split(new[] { ' ' });
                    if (splitcmd.Length > 2)
                        Console.WriteLine("Не поддерживаемая команда");
                    var cmd = splitcmd[0];
                    var arg = splitcmd.Length == 2 ? splitcmd[1] : string.Empty;

                    var isKnownCmd = Const.CommandDescriptions.ContainsKey(cmd.ToLower());
                    if (!isKnownCmd)
                    {
                        Console.WriteLine("Не известная команда");
                        continue;
                    }
                    if (!Const.EditAvailableCmdList.Contains(cmd.ToLower()))
                    {
                        Console.WriteLine($"Команда {cmd} не поддерживается в режиме редактирования анкеты");
                        continue;
                    }

                    switch (cmd)
                    {
                        case Const.Restart:
                            ws = new Worksheet();
                            currentQuestionItem = 0;
                            break;
                        case Const.Goto:
                            var str = ValidateInt(arg);
                            if(!str.IsNullOrEmpty())
                            {
                                Console.WriteLine(str);
                                break;
                            }
                            var num = Convert.ToInt32(arg);
                            if (num < 1 || num > 5)
                            {
                                Console.WriteLine("Указан не правильный номер вопроса");
                                break;
                            }
                            currentQuestionItem = num-1;
                            break;
                        case Const.GotoPrev:
                            currentQuestionItem = currentQuestionItem == 0 ? 0 : currentQuestionItem - 1;
                            break;
                        case Const.Save:
                            var requiredQuestions = new List<int>();
                            var msg = ValidateWS(ws, qList,requiredQuestions);
                            if (!msg.IsNullOrEmpty())
                            {
                                Console.WriteLine(msg);
                                currentQuestionItem = requiredQuestions[0]; 
                                break;
                            }
                            Save(ws, wsDao);
                            return;
                        case Const.Exit:
                            Exit();
                            break;
                        case Const.Help:
                            ShowHelp();
                            break;
                        default:
                            Console.WriteLine(
                                $"Команда {cmd} не распознана. введите {Const.Help} для просмотра поддерживаемых команд");
                            break;
                    }
                }
                else
                {
                    var detail = ws.Detail.FirstOrDefault(x => x.QuestionId == currentQuestionItem);
                    var errorMsg = string.Empty;
                    if (currentQuestionItem == DaoConst.FioId)
                    {
                        errorMsg = ValidateFio(input);
                    }
                    else if (currentQuestionItem == DaoConst.BirthDayId)
                    {
                        errorMsg = ValidateDate(input);
                    }
                    else if (currentQuestionItem == DaoConst.ExperienceId)
                    {
                        errorMsg = ValidateInt(input);
                    }
                    else if (currentQuestionItem == DaoConst.PhoneId)
                    {
                        errorMsg = ValidatePhone(input);
                    }else if (currentQuestionItem == DaoConst.ProgLangId)
                    {
                        errorMsg = ValidateProgLang(input);
                    }

                    if (!errorMsg.IsNullOrEmpty())
                    {
                        Console.WriteLine(errorMsg);
                        continue;
                    }
                    
                    if (detail != null)
                    {
                        detail.Answer = input;
                    }
                    else
                    {
                        ws.Detail.Add(new WorksheetDetail
                        {
                            Question = qList[currentQuestionItem].Text,
                            Answer = input,
                            QuestionId = Convert.ToInt32(qList[currentQuestionItem].Id),
                        });
                    }

                    currentQuestionItem++;
                }
            }
        }

        static string ValidateWS(Worksheet ws, List<Question> qList, List<int> requiredQuestrions)
        {
            var qId = qList.Select(x => x.Id)
                .Where(k => !ws.Detail.Select(d => d.QuestionId).ToList().Contains(k)).ToList();
            requiredQuestrions.AddRange( qId);
            if (!qId.IsNullOrEmpty())
                return 
                    $"Надо ответить на вопросы {qId.Select(x => (x+1).ToString()).Aggregate((p, n) => p + ", " + n)} ";
            //var res = ValidateInt()
            return string.Empty;
        }

        static string ValidateInt(string val)
        {
            return val.AsInt().HasValue ? string.Empty : "Введено  не корректное значение";
        }

        static string ValidatePhone(string phone)
        {
            return (!phone.IsNullOrEmpty() && Regex.IsMatch(phone, Common.CommonConst.RefExpConst.RxPhone))
                ? string.Empty
                : "Не корректный номер телефона";

        }

        static string ValidateFio(string fio)
        {

            return !fio.IsNullOrEmpty() && Regex.IsMatch(fio, Common.CommonConst.RefExpConst.RxFullFIO)
                ? string.Empty
                : "Ошибка в формате ввода ФИО";
            
        }

        static string ValidateProgLang(string val)
        {
            return (CommonConst.ProgLangList.Select(x => x.ToLower()).Contains(val.ToLower()))
                ? string.Empty
                : $"Разрешенные для ввода значения {CommonConst.ProgLangList.Aggregate((p, n) => p + ", " + n)}";
        }

        static string ValidateDate(string date)
        {
            var dt = date.AsDate();
            return dt.HasValue
                ? String.Empty
                : "Не верный формат даты. Требуемый формат дд.мм.гггг";
        } 

    }
}
