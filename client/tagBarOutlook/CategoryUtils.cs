using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using TagCommon;

namespace OutlookTagBar
{
    class CategoryUtils
    {
        public static void AddCategory(String tagName, Outlook.Application application)
        {
            Outlook.Categories categories = application.Session.Categories;
            if (!CategoryExists(tagName, application))
            {
                Outlook.Category category = categories.Add(tagName);
            }
        }
        public static bool CategoryExists(string categoryName, Outlook.Application application)
        {
            try
            {
                Outlook.Category category = application.Session.Categories[categoryName];
                if (category != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        public static void AddCategoryToMailItem(Outlook.MailItem mi, String tag, Outlook.Application application)
        {
            CategoryUtils.EnsureCategoryExists(tag, application);
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

        /*public static void ExpressTagButtonsFromCategories(OutlookTagBar tagBar, Outlook.MailItem mailItem)
        {
            if (mailItem.Categories != null)
            {
                String categories = mailItem.Categories;
                if (!categories.Equals(""))
                {
                    char[] delims = new char[1];
                    delims[0] = ',';
                    String[] tagNames = categories.Split(delims);
                    List<String> cleanTagNames = Utils.CleanTagNames(tagNames);

                    foreach (String tagName in cleanTagNames)
                    {
                        tagBar.AddNewButton(tagName);
                    }
                }
            }
        }
        */
    }
}
