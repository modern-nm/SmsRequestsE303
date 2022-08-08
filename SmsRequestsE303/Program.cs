using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmsRequestsE303
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            System.Net.ServicePointManager.Expect100Continue = false;
            HttpClient client = new HttpClient();
            try
            {
                string text = args[1];
                string phone = NormalizePhone(args[0]);

                if (!IsPhoneValid(phone))
                {
                    MessageBox.Show($"Phone number {phone} is not valid", "Invalid PhoneNumber", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //2022-07-01 17:52:37
                string xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <request>
                                <Index>-1</Index>
                                <Phones><Phone>{phone}</Phone></Phones>
                                <Sca></Sca>
                                <Content>{text}</Content>
                                <Length>{text.Length}</Length>
                                <Reserved>1</Reserved>
                                <Date>{now}</Date>
                            </request>";


                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(@"http://192.168.1.1/api/sms/send-sms");
                request.Method = HttpMethod.Post;


                request.Content = new StringContent(xml, Encoding.UTF8, "text/xml");
                HttpResponseMessage response = await client.SendAsync(request);
                Console.WriteLine(response.RequestMessage);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    
                    Console.WriteLine(response.StatusCode);
                    MessageBox.Show(response.StatusCode + "The message was sent.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TryWriteLog(response, phone, text, now);
                }
                else
                {
                    Console.WriteLine(response.StatusCode + "! Something went wrong");
                    MessageBox.Show(response.StatusCode + "! Bad response, see logs. The message was not sent.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    TryWriteLog(response, phone, text, now);
                }
            }
            catch (Exception e)
            {
                TryWriteCrashLog(e);
                Console.WriteLine($"{e.Message}\n{e.StackTrace}\n{e.Source}\n{e.Data}");
            }
            Console.ReadLine();
        }
        public static bool TryWriteLog(HttpResponseMessage responseMessage, string phone, string text, string date)
        {
            
            try
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "log.txt", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine($"status:{responseMessage.StatusCode}\tphone:{phone}\ttext:{text}\tdate:{date}");
                sw.Close();
                fs.Close();
                sw.Dispose();
                fs.Dispose();
                return true;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "crash.txt", FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine($"{DateTime.Now} {e.Message}\n{e.StackTrace}\n{e.Source}\n{e.Data}");
                sw.WriteLine($"status:{responseMessage.StatusCode}\tphone:{phone}\ttext:{text}\tdate:{date}");
                sw.Close();
                fs.Close();
                sw.Dispose();
                fs.Dispose();
                return false;
            }
        }
        public static bool TryWriteCrashLog(Exception e)
        {
            try
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "crash.txt", FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine($"{DateTime.Now} {e.Message}\n{e.StackTrace}\n{e.Source}\n{e.Data}");
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Error during to write crashlog!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool IsPhoneValid(string phone)
        {
            Regex regex = new Regex(@"\+375\d{9}");

            return regex.IsMatch(phone);
        }

        public static string NormalizePhone(string phone)
        {
            string result = "";


            for(int i = 0; i < phone.Length; i++)
            {
                if (IsDigit(phone[i]))
                    result += phone[i];
            }
            return result;
        }

        public static bool IsDigit(char ch)
        {
            return (ch >= '0' && ch <= '9') || ch=='+';
        }

    }    
}
