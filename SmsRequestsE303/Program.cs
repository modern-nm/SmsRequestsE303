using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmsRequestsE303
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri("http://192.168.1.1/api/send_sms");
            try
            {
                string text = args[1];
                string phone = args[0];
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); //2022-07-01 17:52:37
                string xml = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
                            <request>
                            <Index>-1</Index >
                            <Phones><Phone>{phone}</Phone></Phones>
                            <Sca></Sca>
                            <Content>{text}</Content>
                            <Length>{text.Length}</Length >
                            Reserved>1</ Reserved >
                            <Date>{now}</Date> 
                        </request>";

                SmsContent content = new SmsContent(xml, "text/xml");
                HttpResponseMessage response = await client.PostAsync(uri, content);
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    Console.WriteLine(response.StatusCode);
                }
                else
                {
                    Console.WriteLine(response.StatusCode + " smthing went wrong");
                }
            }
            catch (Exception e)
            {

                Console.WriteLine($"{e.Message}\n{e.StackTrace}\n{e.Source}\n{e.Data}");
            }
            
        }
    }

    public class SmsContent : HttpContent
    {
        public string Content { get; set; }
        public string ContentType { get; set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            throw new NotImplementedException();
        }

        protected override bool TryComputeLength(out long length)
        {
            throw new NotImplementedException();
        }
        public SmsContent(string content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }
    }
}
