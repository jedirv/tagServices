using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using Outlook = Microsoft.Office.Interop.Outlook;

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

        public static void EnsureCategoryExists(String tag, Outlook.Application application)
        {
            Outlook.Categories categories = application.Session.Categories;
            Outlook.Category match = null;
            foreach (Outlook.Category category in categories)
            {
                if (category.Name.Equals(tag))
                {
                    match = category;
                }
            }
            if (null == match)
            {
                throw new TagServicesException("Tried to add nonexistent category mailItem as per tag " + tag);
            }
        }
        public static void RemoveCategoryFromMailITem(String tag, Outlook.MailItem mi)
        {
            if (tag.Equals(mi.Categories))
            {
                mi.Categories = "";
                mi.Save();
            }
            else
            {
                if (mi.Categories.StartsWith(tag + ", "))
                {
                    mi.Categories = mi.Categories.Replace(tag + ", ", "");
                    mi.Save();
                }
                if (mi.Categories.Contains(", " + tag + ", "))
                {
                    mi.Categories = mi.Categories.Replace(", " + tag + ", ", ", ");
                    mi.Save();
                }
                if (mi.Categories.EndsWith(", " + tag))
                {
                    mi.Categories = mi.Categories.Replace(", " + tag, "");
                    mi.Save();
                }
            }
        }
        public static void AddCategoryToMailItem(Outlook.MailItem mi, String tag, Outlook.Application application)
        {
            Utils.EnsureCategoryExists(tag, application);
            String categoriesString = mi.Categories;
            if (null == categoriesString || "".Equals(categoriesString))
            {
                mi.Categories = tag;// adding first category
                mi.Save();
            }
            else
            {
                // some categories already assigned
                String[] cats = categoriesString.Split(',');
                bool categoryAlreadyAssociated = false;
                foreach (String cat in cats)
                {
                    if (cat.Equals(tag))
                    {
                        categoryAlreadyAssociated = true;
                    }
                }
                if (categoryAlreadyAssociated)
                {
                    // don't change anything
                }
                else
                {
                    mi.Categories = categoriesString + "," + tag;
                    mi.Save();
                }
            }
        }
    }
}
