using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace TagCommon
{
    public class Utils
    {
        public static Documents GetDocumentsForJson(String json)
        {
            Documents docs = JsonConvert.DeserializeObject<Documents>(json);
            return docs;
        }
        public static Persons GetPersonsForJson(String json)
        {
            Persons persons = JsonConvert.DeserializeObject<Persons>(json);
            return persons;
        }
        public static TagNames GetTagNamesForJson(String json)
        {
            TagNames tags = JsonConvert.DeserializeObject<TagNames>(json);
            return tags;
        }
        public static String SerializeObjectToString(Object obj)
        {
            string output = JsonConvert.SerializeObject(obj);
            return output;
        }
        
        public static String NormalizeName(String name)
        {
            if (name.Contains(", "))
            {
                int indexOfComma = name.IndexOf(',');
                String lastName = name.Substring(0, indexOfComma);
                String firstName = name.Substring(indexOfComma + 2, name.Length-(indexOfComma+2));
                name = firstName + " " + lastName;
            }
            return name;
        }

        public static String URLEscapeString(String s)
        {
            return System.Net.WebUtility.UrlEncode(s);
        }
    }
}
