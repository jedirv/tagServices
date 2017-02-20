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
        public static string RESOURCE_TYPE_FILE = "FILE";
        public static string RESOURCE_TYPE_URL = "URL";

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

        public static List<String> CleanTagNames(String[] names)
        {
            List<String> result = new List<String>();
            foreach (String name in names)
            {
                String cleanString = "";
                if (name.StartsWith(" "))
                {
                    cleanString = name.Remove(0, 1);
                }
                else
                {
                    cleanString = name;
                }
                result.Add(cleanString);
            }
            return result;
        }
        public static List<String> GetLatestTagList()
        {
            String json = Backend.AllTags();
            TagNames tagNames = Utils.GetTagNamesForJson(json);
            List<TagName> tagNameList = tagNames.Tags;
            List<String> tags = new List<String>();
            foreach (TagName tag in tagNameList)
            {
                tags.Add(tag.Name);
            }
            tags.Sort();
            return tags;
        }

        public static void TagResourceForMailItem(String entryID, string resourceName)
        {
            String json = TagCommon.Backend.TagsForEmail(entryID);
            TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
            List<TagName> tags = tagNames.Tags;
            foreach (TagName tag in tags)
            {
                Backend.TagResource(Utils.RESOURCE_TYPE_FILE, resourceName, tag.Name);
            }
        }

    }
}
