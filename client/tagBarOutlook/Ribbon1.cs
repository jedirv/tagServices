using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using TagCommon;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OutlookTagBar
{
    [ComVisible(true)]
    public class Ribbon1 : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public Ribbon1()
        {

        }
        public void mySaveAttachments(Office.IRibbonControl control, bool cancelDefault)
        {
            cancelDefault = false;
            /*
            var context = control.Context;

            Outlook.Explorer explorer = Globals.OutlookTagBarAddin.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = item as Outlook.MailItem;
                    Outlook.Attachments attachments = mailItem.Attachments;
                    if (attachments.Count == 0)
                    {
                        cancelDefault = true;
                        return;
                    }
                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    for (; i < attachments.Count; i++) {
                        Outlook.Attachment a = attachments[i];
                        sb.Append(a.FileName + ",");
                    }
                    sb.Append(attachments[i].FileName);
                    string fileListString = "" + sb;
                    
                    SaveFileDialog sfd = new SaveFileDialog();
                   
                    sfd.Title = "Testing Jed";
                    sfd.FileName = fileListString;
                    sfd.Filter = "All files(*.*) | *.*";

                    sfd.ShowDialog();
                    String nameListString = sfd.FileName;
                    System.Diagnostics.Debug.Write("snameListString : " + nameListString + "\n");
                    string[] names = nameListString.Split(',');
                    for (i = 0; i < names.Length; i++)
                    {
                        string name = names[i];
                        Outlook.Attachment a = attachments[i + 1];
                        System.Diagnostics.Debug.Write("save attachment as : " + name + "\n");
                        a.SaveAsFile(name);
                    }
                    
                    cancelDefault = true;
                }
                else
                {
                    cancelDefault = false;
                }
            }
            else
            {
                cancelDefault = false;
            }
            */
        }
        public void mySaveAttachAs(Office.IRibbonControl control, bool cancelDefault)
        {
            //Office.IRibbonControl.Context context = control.Context;
            
            Outlook.Explorer explorer = Globals.OutlookTagBarAddin.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = item as Outlook.MailItem;
                    Outlook.Attachments attachments = mailItem.Attachments;
                    Outlook.Attachment a = attachments[1];
                    System.Diagnostics.Debug.Write("a.displayName : " + a.DisplayName + "\n");
                    System.Diagnostics.Debug.Write("a.pathName : " + a.PathName + "\n");
                    System.Diagnostics.Debug.Write("a.fileName : " + a.FileName + "\n");
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save Attachment";
                    sfd.FileName = a.FileName;
                    sfd.Filter = "All files(*.*) | *.*";
                    sfd.DefaultExt = System.IO.Path.GetExtension(a.FileName);

                    sfd.ShowDialog();
                    String resourceName = sfd.FileName;
                    
                    System.Diagnostics.Debug.Write("resourceName : " + resourceName + "\n");
                    a.SaveAsFile(sfd.FileName);
                    Backend.AddResource(Utils.RESOURCE_TYPE_FILE, resourceName);
                    TagResourceForMailItem(mailItem, resourceName);
                    //a.SaveAsFile(@"C:\Users\sudo\Downloads");
                    cancelDefault = true;
                }
                else
                {
                    cancelDefault = false;
                }
            }
            else
            {
                cancelDefault = false;
            }
            // nlog
        }
        public void TagResourceForMailItem(Outlook.MailItem mailItem, string resourceName)
        {
            String entryID = mailItem.EntryID;
            String json = TagCommon.Backend.TagsForEmail(entryID);
            TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
            List<TagName> tags = tagNames.Tags;
            foreach (TagName tag in tags)
            {
                Backend.TagResource(Utils.RESOURCE_TYPE_FILE, resourceName, tag.Name);
            }
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("OutlookTagBar.Ribbon1.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
