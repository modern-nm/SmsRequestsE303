using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
            System.Net.ServicePointManager.Expect100Continue = false;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(@"http://192.168.1.1");
            //Uri uri = new Uri("http://192.168.1.5/awtadfg");
            try
            {
                string text = args[1];
                string phone = args[0];
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
                //                string xm = $@"<?xml version=""1.0"" encoding =""UTF-8""?>
                //<request>
                //    <Index>-1</Index>
                //    <Phones><Phone>+375336742726</Phone></Phones>
                //    <Sca></Sca>
                //    <Content>Uasd</Content>
                //    <Length>4</Length>
                //    <Reserved>1</Reserved>
                //    <Date>2022-08-07 17:52:37</Date>
                //</request>";
                string xm = $@"<request>
    <Index>-1</Index>
    <Phones><Phone>+375336742726</Phone></Phones>
    <Sca></Sca>
    <Content>Uasd</Content>
    <Length>4</Length>
    <Reserved>1</Reserved>
    <Date>2022-08-07 17:52:37</Date>
</request>";


                HttpRequestMessage request = new HttpRequestMessage();
                request.RequestUri = new Uri(@"http://192.168.1.1/api/sms/send-sms");
                request.Method = HttpMethod.Post;


                request.Content = new StringContent(xm, Encoding.UTF8, "text/xml");
                //SmsContent content = new SmsContent(xml, "text/xml");
                //HttpResponseMessage response = await client.PostAsync(uri, content);
                try
                {
                    HttpResponseMessage response = await client.SendAsync(request);
                    Console.WriteLine(response.RequestMessage);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode + " smthing went wrong");
                    }
                }
                catch (Exception f)
                {

                    Console.WriteLine($"{f.Message}\n{f.StackTrace}\n{f.Source}\n{f.Data}");
                }





            }
            catch (Exception e)
            {

                Console.WriteLine($"{e.Message}\n{e.StackTrace}\n{e.Source}\n{e.Data}");
            }
            Console.ReadLine();
        }
    }

    public class SmsContent : HttpContent
    {
        public string Content { get; set; }
        public string ContentType { get; set; }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            Contract.Assert(stream != null);

            return Task.CompletedTask;
        }

        protected override bool TryComputeLength(out long length)
        {
            length = Content.Length;
            return true;
            
        }
        public SmsContent(string content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }
    }
}
