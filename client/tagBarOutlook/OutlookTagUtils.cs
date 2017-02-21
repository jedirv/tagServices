using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCommon;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookTagBar
{
    public class OutlookTagUtils
    {
        private static String NL = Environment.NewLine;
        public static void RemoveTagFromEmail(String tag, Outlook.MailItem mi, Outlook.Application application, OutlookTagBar explorerTagBar)
        {
            Backend.UntagEmail(mi.EntryID, tag);
            CategoryUtils.RemoveCategoryFromMailITem(tag, mi);
            RemoveTagFromExplorerEmailIfMatch(mi.EntryID, tag, application, explorerTagBar);
            foreach (Outlook.Inspector inspector in application.Inspectors)
            {
                RemoveTagFromInspectorEmailIfMatch(inspector, mi.EntryID, tag);
            }
        }



        public static void AddTagToEmail(String tag, Outlook.MailItem mi, Outlook.Application application, OutlookTagBar explorerTagBar)
        {
            Backend.TagEmail(mi.EntryID, tag);
            Backend.TagPerson(Utils.NormalizeName(mi.SenderName), tag);
            CategoryUtils.AddCategoryToMailItem(mi, tag, application);
            AddTagToExplorerEmailIfMatch(mi.EntryID, tag, application, explorerTagBar);
            foreach (Outlook.Inspector inspector in application.Inspectors)
            {
                AddTagToInspectorEmailIfMatch(inspector, mi.EntryID, tag);
            }
        }

        private static void RemoveTagFromInspectorEmailIfMatch(Outlook.Inspector inspector, String entryID, String tag)
        {
            if (inspector.CurrentItem is Outlook.MailItem)
            {
                Outlook.MailItem mailItem = inspector.CurrentItem as Outlook.MailItem;
                if (entryID.Equals(mailItem.EntryID))
                {
                    InspectorWrapper iWrapper = InspectorWrapper.inspectorWrappersValue[inspector];
                    OutlookTagBar otb = iWrapper.getTagBar();
                    otb.RemoveTagButton(tag);
                }
            }
        }
        private static void AddTagToInspectorEmailIfMatch(Outlook.Inspector inspector, String entryID, String tag)
        {
            if (inspector.CurrentItem is Outlook.MailItem)
            {
                Outlook.MailItem mailItem = inspector.CurrentItem as Outlook.MailItem;
                if (entryID.Equals(mailItem.EntryID))
                {
                    InspectorWrapper iWrapper = InspectorWrapper.inspectorWrappersValue[inspector];
                    OutlookTagBar otb = iWrapper.getTagBar();
                    otb.AddNewButton(tag);
                }
            }
        }
        private static void RemoveTagFromExplorerEmailIfMatch(String entryID, String tag, Outlook.Application application, OutlookTagBar explorerTagBar)
        {
            try
            {
                if (application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;
                        if (mailItem.EntryID.Equals(entryID))
                        {
                            explorerTagBar.RemoveTagButton(tag);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                String expMessage = ex.Message;
                System.Windows.Forms.MessageBox.Show(expMessage);
            }
        }
        private static void AddTagToExplorerEmailIfMatch(String entryID, String tag, Outlook.Application application, OutlookTagBar explorerTagBar)
        {
            try
            {
                if (application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;
                        if (mailItem.EntryID.Equals(entryID))
                        {
                            explorerTagBar.AddNewButton(tag);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                String expMessage = ex.Message;
                System.Windows.Forms.MessageBox.Show(expMessage);
            }
        }
        public static void CreateNewTag(String tag, Outlook.Application application, OutlookTagBar explorerTagBar)
        {
            System.Diagnostics.Debug.Write("New tag : " + tag + NL);
            CategoryUtils.AddCategory(tag, application);
            Backend.AddTag(tag);
            List<String> latestTags = Utils.GetLatestTagList();
            explorerTagBar.LoadTagList(latestTags);
            Dictionary<Outlook.Inspector, InspectorWrapper>.KeyCollection keys = InspectorWrapper.inspectorWrappersValue.Keys;
            foreach (Outlook.Inspector inspector in keys)
            {
                InspectorWrapper iWrapper = InspectorWrapper.inspectorWrappersValue[inspector];
                iWrapper.getTagBar().LoadTagList(latestTags);
            }
        }

    }
}
