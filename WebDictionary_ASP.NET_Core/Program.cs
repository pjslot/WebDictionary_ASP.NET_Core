using System.Runtime.Serialization.Formatters;
using System.Text;
using System;

namespace WebDictionary_ASP.NET_Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //������� �������
            Dictionary<string, string> words = new Dictionary<string, string>();
            words.Add("cat", "�����");
            words.Add("dog", "������");
            words.Add("mouse", "����");
            words.Add("lion", "���");
            words.Add("tiger", "����");

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            //�������� ��������
            app.Map("/111", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                await ctx.Response.WriteAsync("<h2>����� ���������� �<i> WEB-�������</i>!</h2> " +
                    "<h3><p>���������� �� ����������:</p>" +
                    "<p style='background-color: #E1F896'>/words - ���������� ��� �������� �������</p>" +
                    "<p style='background-color: #E1F896'>/words/add?en=table&ru=���� - �������� ����� ����</p>" +
                    "<p style='background-color: #E1F896'>/words/get?en=table - ����� ������� ��� ��������� �����</p>" +
                    "<p style='background-color: #E1F896'>/words/delete?en=table - ������� ����� � ��� ������� �� �������</p>" +
                    "<p style='background-color: #E1F896'>/words/test - ���� �� ������ �� �����������</p>" +
                    "<p style='background-color: #E1F896'>/ - ��� ��������</p></h3>");
            });

            //����� ����� ������� 
            app.Map("/words", async (ctx) =>
            {               
                ctx.Response.ContentType = "text/html; charset=utf-8";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<u><h5 style='color: #0008ff'>�������.</h5></u>");
                foreach (var word in words)
                {
                    sb.AppendLine("<p style='background-color: #E1F896'>" + word.Key + " --- " + word.Value+"</p>");
                }
                await ctx.Response.WriteAsync(sb.ToString());
            });

            //���������� ���� � �������
            app.Map("words/add", async (ctx) =>
            {
               ctx.Response.ContentType = "text/html; charset=utf-8";
               //������� ���������� ����� query ������
               string en = ctx.Request.Query["en"];
               string ru = ctx.Request.Query["ru"];
                if (en == null || ru == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query ������ ������� �����������.</h2>" +
                     " ������� �� �����: <p>words/add?en=table&ru=����</p>");
                else
                //�������� �� �������� ����� � �������
                if (en != null && words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>C���� '" + en + "' ��� ���� � �������.</h2>" +
                     " ������� ���� ����, ������� ����������� � �������.");
                else
                //���������� ���� � �������
                {
                    words.Add(en, ru);
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'>���� ���� '" + en + "' --- '"+ru+"' ������� ��������� � �������.</h2> " +
                       "<a href = '/words'>---> ���������� ���������� ������� <---</a>");
                }   
            });

            //����� �������� �����
            app.Map("words/get", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //������� ���������� ����� query ������
                string en = ctx.Request.Query["en"];                
                if (en == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query ������ ������� �����������.</h2>" +
                     " ������� �� �����: <p>words/get?en=table</p>");
                else
                //�������� �� ����������� ����� � �������
                if (en != null && !words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>C���� '" + en + "' ����������� � �������.</h2>" +
                     " ������� ����� ���������. <p><a href = '/words'>---> ���������� ���������� ������� <---</a></p>");
                else
                //����� ��������
                {         
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'>������� ����� '" + en + "' --- '" + words[en] + "'. ������� �������� �������.</h2> " +
                       "<a href = '/words'>---> ���������� ���������� ������� <---</a>");
                }
            });

            //�������� ���� �� �������
            app.Map("words/delete", async (ctx) =>
            {
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //������� ���������� ����� query ������
                string en = ctx.Request.Query["en"];
                if (en == null) await ctx.Response.WriteAsync("<h2 style='background-color: red'>Query ������ ������� �����������.</h2>" +
                     " ��� �������� ����� �� ������� ������� ����� ������ �� �����: <p>words/delete?en=table</p>");
                else
                //�������� �� ���������� ����� � �������
                if (en != null && !words.ContainsKey(en)) await ctx.Response.WriteAsync("<h2 style='background-color: red'>C���� '" + en + "' �� ������� � �������.</h2>" +
                     " ��� �������� ����� �� ������� ������� ����� ������� ����������� � �������. <p><a href = '/words'>---> ���������� ���������� ������� <---</a></p>");
                else
                //�������� ���� �� �������
                {
                    words.Remove(en);
                    await ctx.Response.WriteAsync("<h2 style='background-color: green'> ����� '" + en + "' � ��� ������� ������� ������� �� �������.</h2> " +
                       "<a href = '/words'>---> ���������� ���������� ������� <---</a>");
                }
            });

            //������������ �� ������
            app.Map("/words/test", async (HttpContext ctx) => 
            {
                StringBuilder sb = new StringBuilder();
                ctx.Response.ContentType = "text/html; charset=utf-8";
                //���� ���� �������������
                if (ctx.Request.Method == "GET")
                {
                    sb.AppendLine("<h2> ����! ������� �������� ��������� ���� </h2>");
                    sb.AppendLine(@"<form method = ""POST"">");

                    for (int i = 0; i < words.Count; i++)
                    {
                        string en = words.ElementAt(i).Key;
                        sb.AppendLine($"<p>����� �� ����������:<b> {en}</b></p> ");
                        sb.AppendLine($"������� �� �������: ");
                        sb.AppendLine($@"<input type=""text"" name={en}>");
                    }

                    sb.AppendLine(@"<p><input type=""submit"" value = ""�� ��������!""></p> ");
                    sb.AppendLine("</form>");
                }
                //��������� �����������
                else if(ctx.Request.Method == "POST")
                {
                    int counter = 0, done = 0, fail = 0;
                    
                    sb.AppendLine("<h2> ���������� �����: </h2>");
                    foreach (var item in ctx.Request.Form)
                    {
                        sb.AppendLine($"<p><b>����� ����� {counter+1}:</b></p> ");
                        sb.AppendLine($"����� <b>{item.Key}</b> �� �������� ��� - <b>{item.Value}</b>");
                        //���� ���������
                        if ((Convert.ToString(item.Value)).ToLower() == (Convert.ToString(words.ElementAt(counter).Value)).ToLower())
                        {
                            sb.AppendLine($"<p style='background-color: green'>� ��� ���������!</p>");
                            done++;
                        }
                        //���� �����������
                        else
                        {
                            sb.AppendLine($"<p style='background-color: red'>� ��� �� ���������!� ��������� <b> {words.ElementAt(counter).Value} </b></p>");
                            fail++;
                        }                        
                        counter++;
                    }
                    //����� �����������
                    sb.AppendLine("<h2> �����: </h2>");
                    sb.AppendLine($"<p style='background-color: green'>������ �������: {done}</p> ");
                    sb.AppendLine($"<p style='background-color: red'>�������� �������: {fail}</p> ");
                    sb.AppendLine("<a href = '/words'>---> ���������� ���������� ������� <---</a>");
                }
                await ctx.Response.WriteAsync(sb.ToString());
            });

            app.Run();
        }
    }

}