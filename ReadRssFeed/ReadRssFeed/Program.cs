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
//    Create table feedrecord(
//      feedID int NOT NULL IDENTITY(1,1) PRIMARY KEY,
//      actualLink varchar(max),
//  title varchar(max),
//  Summary varchar(max),
//  appendstring varchar(max),
//  Shortenlink varchar(200),
//  isPosted varchar(1)
//);

    class Program
    {
        static void Main(string[] args)

        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "sudhirawrssfeeder.database.windows.net";
            builder.UserID = "sudhiraw";
            builder.Password = "P@$$w0rd@123";
            builder.InitialCatalog = "rssfeeddb";
            string stmt = "INSERT INTO feedrecord(actualLink,title, Summary, appendstring, Shortenlink, isPosted) VALUES(@actualLink,@title, @Summary, @appendstring, @Shortenlink, @isPosted)";
            string str_Append_Query = "?wt.mc_id=AID2463800_QSG_SCL_361865&ocid=AID2463800_QSG_SCL_361865&utm_medium=Owned%20%26%20Operated&utm_campaign=FY20_APAC_Dev%20Community_CFT_Internal%20Social";
          
            XmlReader reader = XmlReader.Create("https://azurecomcdn.azureedge.net/en-us/updates/feed/");
            SyndicationFeed feed = SyndicationFeed.Load(reader);
            int i = 1;

            string fileName = @"C:\Temp\"+ DateTime.Today.Date.DayOfWeek.ToString() +"_socialpost.txt";
            FileStream fsc = File.Create(fileName);
            fsc.Close();

            foreach (SyndicationItem item in feed.Items)
            {
                string strTitle = item.Title.Text.ToString().Replace("Azure","#Azure");
                Console.WriteLine(strTitle);
                Console.WriteLine(item.Summary.Text.ToString());
                //Console.WriteLine(item.BaseUri.AbsoluteUri);
                Console.WriteLine( "Link :- " + item.Links[0].Uri.ToString());
                string strNewlink = item.Links[0].Uri.ToString() + str_Append_Query;
                Console.WriteLine("New Link :- " + strNewlink);
                string strShortenLink = "http://tinyurl.com/api-create.php?url=" + strNewlink;
                var request = WebRequest.Create(strShortenLink);
                var res = request.GetResponse();
                string strreceiveShortenLink;
                using (var responsereader = new StreamReader(res.GetResponseStream()))
                {
                    strreceiveShortenLink = responsereader.ReadToEnd();

                    Console.WriteLine(strShortenLink);
                }
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand(stmt, connection);
                    cmd.Parameters.Add("@actualLink", SqlDbType.VarChar);
                    cmd.Parameters.Add("@title", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Summary", SqlDbType.VarChar);
                    cmd.Parameters.Add("@appendstring", SqlDbType.VarChar);
                    cmd.Parameters.Add("@Shortenlink", SqlDbType.VarChar);
                    cmd.Parameters.Add("@isPosted", SqlDbType.VarChar);
                    cmd.Parameters["@actualLink"].Value = item.Links[0].Uri.ToString();
                    cmd.Parameters["@title"].Value = strTitle;
                    cmd.Parameters["@Summary"].Value = item.Summary.Text.ToString();
                    cmd.Parameters["@appendstring"].Value = str_Append_Query;
                    cmd.Parameters["@Shortenlink"].Value = strreceiveShortenLink;
                    cmd.Parameters["@isPosted"].Value = 'N';
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("Record added - " + i );
                i = i + 1;
              

                try
                {
                    // Check if file already exists. If yes, delete it.     
                    //if (File.Exists(fileName))
                    //{
                    //    File.Delete(fileName);
                    //}

                    // Create a new file     
                    using (FileStream fs = File.Open(fileName,FileMode.Append))
                    {
                        byte[] newline = Encoding.ASCII.GetBytes(Environment.NewLine);
                        // Add some text to file    
                        Byte[] title = new UTF8Encoding(true).GetBytes(strTitle);
                        fs.Write(title, 0, title.Length);
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

                    // Open the stream and read it back.    
                    
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                }



            }

            Console.WriteLine();
        }
    }
}
