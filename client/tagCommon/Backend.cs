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
            String url = "http://127.0.0.1:5000/tagapi/" + relativeUrl;
            System.Diagnostics.Debug.Write("To    backend: " + relativeUrl);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse webResp = (HttpWebResponse)webRequest.GetResponse();
            Stream stream = webResp.GetResponseStream();
            TextReader tr = new StreamReader(stream);
            //System.Diagnostics.Debug.Write("web service call returned: " + tr.ReadToEnd() + NL);
            String json = tr.ReadToEnd();
            System.Diagnostics.Debug.Write("From  backend: " + json);
            webResp.Close();
            return json;
        }
       

        public static String AddEmail(String entryID, String conversationID)
        {
            return GetJsonFromBackend("addEmail/" + conversationID + "/" + entryID);
        }
        public static String AddPerson(String name)
        {
            return GetJsonFromBackend("addPerson/" + name);
        }
        public static String AddTag(String name)
        {
            return GetJsonFromBackend("addTag?tag=" + name);
        }
        public static String AddResource(String type, String name)
        {
            return GetJsonFromBackend("addResource/" + type + "/" + name);
        }
        public static String TagPerson(String name, String tag)
        {
            return GetJsonFromBackend("tagPerson?name=" + name + "&tag=" + tag);
        }
        public static String TagEmail(String entryID, String tag)
        {
            return GetJsonFromBackend("tagEmail?entryID=" + entryID + "?tag=" + tag);
        }
        public static String TagResource(String type, String name, String tag)
        {
            return GetJsonFromBackend("tagResource?type=" + type + "&name=" + name + "&tag=" + tag);
        }
        public static String TagsForEmail(String entryID)
        {
            return GetJsonFromBackend("tagsForEmail/" + entryID);
        }

        public static String DocsForTag(String tag)
        {
            return GetJsonFromBackend("docsForTag?tag=" + tag);
        }
        public static String ShowPersons()
        {
            return GetJsonFromBackend("showPersons");
        }
    }
}
