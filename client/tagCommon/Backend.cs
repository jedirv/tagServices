using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace TagCommon
{
    public class Backend
    {
        public static String GetJsonFromBackend(String relativeUrl)
        {
            String url = "http://127.0.0.1:5000/" + relativeUrl;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webResp = (HttpWebResponse)webRequest.GetResponse();
            Stream stream = webResp.GetResponseStream();
            TextReader tr = new StreamReader(stream);
            //System.Diagnostics.Debug.Write("web service call returned: " + tr.ReadToEnd() + NL);
            String json = tr.ReadToEnd();
            webResp.Close();
            return json;
        }
    }
}
