//Author:- Sudhir Rawat 
//Purpose :- Get azure service and blog update, add query string and hashtag

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace ReadRssFeed
{
    
    class Program
    {
        static void Main(string[] args)

        {
            //GetRSSFeed function takes 3 parameter
            //1) Feed URL :- Url to the RSS feed 
            //2) File Name append:- Any string you want to add to final output file name
            //3) Query string to add

            Console.WriteLine();
            Console.WriteLine("Working on Azure Service update");



            GetRSSFeed("https://azurecomcdn.azureedge.net/en-us/updates/feed/", "_AzureUpdate_English_Socialpost", "?wt.mc_id=AID2463800_QSG_SCL_361865&ocid=AID2463800_QSG_SCL_361865&utm_medium=Owned%20%26%20Operated&utm_campaign=FY20_APAC_Dev%20Community_CFT_Internal%20Social");
            Console.WriteLine("Working on Azure Blog update");
            GetRSSFeed("https://azurecomcdn.azureedge.net/en-us/blog/feed/", "_AzureBlog_English_Socialpost", "?wt.mc_id=AID2463800_QSG_SCL_361865&ocid=AID2463800_QSG_SCL_361865&utm_medium=Owned%20%26%20Operated&utm_campaign=FY20_APAC_Dev%20Community_CFT_Internal%20Social");

            Console.WriteLine("Done");
        }

        public static void GetRSSFeed(string rssfeedlink, string append2filename, string queryString)
        {

            
            XmlReader reader= XmlReader.Create(rssfeedlink); //Default is ENglish
            string fileName = "";         
            
            fileName = @"C:\Temp\" + DateTime.Today.Date.DayOfWeek.ToString() + append2filename +".txt";
            FileStream fsc = File.Create(fileName);
            fsc.Close();   
            SyndicationFeed feed = SyndicationFeed.Load(reader);
         
            foreach (SyndicationItem item in feed.Items)
            {
                string strTitle = item.Title.Text.ToString().Replace("Azure", "#Azure") + " #azure4developers";
                Console.WriteLine(strTitle);
                string strNewlink = item.Links[0].Uri.ToString() + queryString;     
                
                string strShortenLink = "http://tinyurl.com/api-create.php?url=" + strNewlink;
                var request = WebRequest.Create(strShortenLink);
                var res = request.GetResponse();
                string strreceiveShortenLink;
                using (var responsereader = new StreamReader(res.GetResponseStream()))
                {
                    strreceiveShortenLink = responsereader.ReadToEnd();

                    Console.WriteLine(strShortenLink);
                }

                try
                {
                    
                    // Create a new file     
                    using (FileStream fs = File.Open(fileName, FileMode.Append))
                    {
                        byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes(strTitle);
                        fs.Write(title, 0, title.Length);
                        fs.Write(newline, 0, newline.Length);

                        Byte[] publishDate = new UTF8Encoding(true).GetBytes(item.PublishDate.ToString());
                        fs.Write(publishDate, 0, publishDate.Length);
                        fs.Write(newline, 0, newline.Length);

                        byte[] summary = new UTF8Encoding(true).GetBytes(item.Summary.Text.ToString());
                        fs.Write(summary, 0, summary.Length);
                        fs.Write(newline, 0, newline.Length);

                        byte[] link = new UTF8Encoding(true).GetBytes(strreceiveShortenLink);
                        fs.Write(link, 0, link.Length);
                        fs.Write(newline, 0, newline.Length);
                        fs.Write(newline, 0, newline.Length);
                        fs.Write(newline, 0, newline.Length);
                        fs.Write(newline, 0, newline.Length);
                        fs.Write(newline, 0, newline.Length);
                        fs.Write(newline, 0, newline.Length);

                        fs.Close();
                    }    
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }
            }
        }
        
    }
}

