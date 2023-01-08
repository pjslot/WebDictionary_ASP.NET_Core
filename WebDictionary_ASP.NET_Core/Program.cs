using System.Runtime.Serialization.Formatters;
using System.Text;
using System;

namespace WebDictionary_ASP.NET_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //базовый словарь
            Dictionary<string, string> words = new Dictionary<string, string>();
            words.Add("cat", "кошка");
            words.Add("dog", "собака");
            words.Add("mouse", "мышь");
            words.Add("lion", "лев");
            words.Add("tiger", "тигр");

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            //корневая страница
            app.Map("/111", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                await ctx.Response.WriteAsync("<h2>Добро пожаловать в<i> WEB-Словарь</i>!</h2> " +
                    "<h3><p>Инструкция по применению:</p>" +
                    "<p style='background-color: #E1F896'>/words - Отобразить все элементы словаря</p>" +
                    "<p style='background-color: #E1F896'>/words/add?en=table&ru=стол - Добавить новую пару</p>" +
                    "<p style='background-color: #E1F896'>/words/get?en=table - Найти перевод для заданного слова</p>" +
                    "<p style='background-color: #E1F896'>/words/delete?en=table - Удалить слово и его перевод из словаря</p>" +
                    "<p style='background-color: #E1F896'>/words/test - Тест по словам НЕ РЕАЛИЗОВАНО</p>" +
                    "<p style='background-color: #E1F896'>/ - Эта страница</p></h3>");
            });

            //вывод всего словаря 
            app.Map("/words", async (ctx) =>
            {               
                ctx.Response.ContentType = "text/html; charset=utf-8";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<u><h5 style='color: #0008ff'>СЛОВАРЬ.</h5></u>");
                foreach (var word in words)
                {
                    sb.AppendLine("<p style='background-color: #E1F896'>" + word.Key + " --- " + word.Value+"</p>");
                }
                await ctx.Response.WriteAsync(sb.ToString());
            });

            //добавление пары в словарь
            app.Map("words/add", async (ctx) =>
            {
               ctx.Response.ContentType = "text/html; charset=utf-8";
               //прверка ошибочного ввода query строки
               string en = ctx.Request.Query["en"];
               string ru = ctx.Request.Query["ru"];
                if (en == null || ru == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query строка введена некорректно.</h2>" +
                     " Введите по форме: <p>words/add?en=table&ru=стол</p>");
                else
                //проверка на дубликат слова в словаре
                if (en != null && words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Cлово '" + en + "' уже есть в словаре.</h2>" +
                     " Введите пару слов, которая отсутствует в словаре.");
                else
                //добавление пары в словарь
                {
                    words.Add(en, ru);
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'>Пара слов '" + en + "' --- '"+ru+"' успешно добавлена в словарь.</h2> " +
                       "<a href = '/words'>---> Посмотреть актуальный словарь <---</a>");
                }   
            });

            //поиск перевода слова
            app.Map("words/get", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //прверка ошибочного ввода query строки
                string en = ctx.Request.Query["en"];                
                if (en == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query строка введена некорректно.</h2>" +
                     " Введите по форме: <p>words/get?en=table</p>");
                else
                //проверка на присутствие слова в словаре
                if (en != null && !words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Cлово '" + en + "' отсутствует в словаре.</h2>" +
                     " Введите слово правильно. <p><a href = '/words'>---> Посмотреть актуальный словарь <---</a></p>");
                else
                //вывод перевода
                {         
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'>Перевод слова '" + en + "' --- '" + words[en] + "'. Перевод выполнен успешно.</h2> " +
                       "<a href = '/words'>---> Посмотреть актуальный словарь <---</a>");
                }
            });

            //удаление пары из словаря
            app.Map("words/delete", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //прверка ошибочного ввода query строки
                string en = ctx.Request.Query["en"];
                if (en == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query строка введена некорректно.</h2>" +
                     " Для удаления слова из словаря введите кюери строку по форме: <p>words/delete?en=table</p>");
                else
                //проверка на отсутствие слова в словаре
                if (en != null && !words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Cлово '" + en + "' не найдено в словаре.</h2>" +
                     " Для удаления слова из словаря введите слово которое присуствует в словаре. <p><a href = '/words'>---> Посмотреть актуальный словарь <---</a></p>");
                else
                //удаление пары из словаря
                {
                    words.Remove(en);
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'> Слово '" + en + "' и его перевод успешно удалены из словаря.</h2> " +
                       "<a href = '/words'>---> Посмотреть актуальный словарь <---</a>");
                }
            });

            //тестирование по словам
            app.Map("/words/test", async (HttpContext ctx) => 
            {
                StringBuilder sb = new StringBuilder();
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //ввод слов пользователем
                if (ctx.Request.Method == "GET")
                {
                    sb.AppendLine("<h2> ТЕСТ! Введите переводы следующих слов </h2>");
                    sb.AppendLine(@"<form method = ""POST"">");

                    for (int i = 0; i < words.Count; i++)
                    {
                        string en = words.ElementAt(i).Key;
                        sb.AppendLine($"<p>Слово на английском:<b> {en}</b></p> ");
                        sb.AppendLine($"Перевод на русском: ");
                        sb.AppendLine($@"<input type=""text"" name={en}>");
                    }

                    sb.AppendLine(@"<p><input type=""submit"" value = ""На проверку!""></p> ");
                    sb.AppendLine("</form>");
                }
                //обработка результатов
                else if(ctx.Request.Method == "POST")
                {
                    int counter = 0, done = 0, fail = 0;
                    
                    sb.AppendLine("<h2> Результаты теста: </h2>");
                    foreach (var item in ctx.Request.Form)
                    {
                        sb.AppendLine($"<p><b>Слово номер {counter+1}:</b></p> ");
                        sb.AppendLine($"Слово <b>{item.Key}</b> вы перевели как - <b>{item.Value}</b>");
                        //если правильно
                        if ((Convert.ToString(item.Value)).ToLower() == (Convert.ToString(words.ElementAt(counter).Value)).ToLower())
                        {
                            sb.AppendLine($"<p style='background-color: green'>И это правильно!</p>");
                            done++;
                        }
                        //если неправильно
                        else
                        {
                            sb.AppendLine($"<p style='background-color: red'>И это не правильно!А правильно <b> {words.ElementAt(counter).Value} </b></p>");
                            fail++;
                        }                        
                        counter++;
                    }
                    //вывод результатов
                    sb.AppendLine("<h2> Итого: </h2>");
                    sb.AppendLine($"<p style='background-color: green'>Верных ответов: {done}</p> ");
                    sb.AppendLine($"<p style='background-color: red'>Неверных ответов: {fail}</p> ");
                    sb.AppendLine("<a href = '/words'>---> Посмотреть актуальный словарь <---</a>");
                }
                await ctx.Response.WriteAsync(sb.ToString());
            });

            app.Run();
        }
    }

}