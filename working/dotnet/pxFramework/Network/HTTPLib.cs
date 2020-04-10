using System;
using System.IO;
using System.Net;
using System.Text;

namespace PhazeX.Network
{
    class HTTPLib
    {
        public static string getPage(string url)
        {
            string page = "";

            HttpWebRequest request;
            HttpWebResponse response;
            Stream httpStream;
            int length;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] buffer = new byte[65536];

            try
            {
                //make request
                request = (HttpWebRequest)WebRequest.Create(url);

                //get response
                response = (HttpWebResponse)request.GetResponse();
                httpStream = response.GetResponseStream();

                length = httpStream.Read(buffer, 0, buffer.Length);
                while (length != 0)
                {
                    //save page & convert line feeds
                    page += encoding.GetString(buffer, 0, length);
                    length = httpStream.Read(buffer, 0, buffer.Length);
                }
                httpStream.Close();

            }
            catch (Exception)
            {
                page = "";
            }

            return page;
        }

        public static Stream getStream(string url)
        {
            return HTTPLib.getStream(url, "");
        }

        public static Stream getStream(string url, string referer)
        {
            HttpWebRequest request;
            HttpWebResponse response;
            Stream httpStream;
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] buffer = new byte[65536];
            try
            {
                //make request
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Referer = referer;

                //get response
                response = (HttpWebResponse)request.GetResponse();
                httpStream = response.GetResponseStream();
            }
            catch (Exception)
            {
                httpStream = null;
            }
            return httpStream;
        }
    }
}
