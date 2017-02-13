using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using System.Windows.Forms;
using TagCommon;

namespace OutlookTagBar
{
    public partial class OutlookTagBarAddin
    {
        //private Dictionary<Microsoft.Office.Interop.Outlook.Inspector, Microsoft.Office.Tools.CustomTaskPane> inspectorTaskPaneDictionary = 
        //    new Dictionary<Microsoft.Office.Interop.Outlook.Inspector, Microsoft.Office.Tools.CustomTaskPane>();
        private Outlook.Inspectors inspectors = null;
        private Outlook.Explorer currentExplorer = null;
        private OutlookTagBar explorerTagBar;
        private Microsoft.Office.Tools.CustomTaskPane explorerCustomTaskPane;

        private Dictionary<Outlook.Inspector, InspectorWrapper> inspectorWrappersValue =
            new Dictionary<Outlook.Inspector, InspectorWrapper>();
        private String NL = Environment.NewLine;
        public Dictionary<Outlook.Inspector, InspectorWrapper> InspectorWrappers
        {
            get
            {
                return inspectorWrappersValue;
            }
        }
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            /**
             * This section from https://msdn.microsoft.com/en-us/library/bb296010.aspx is for the 
             * case where the add in is loaded after outlook is already up, which I don't believe is
             * a use case we need to support, but leave this in case.
             */
            inspectors = this.Application.Inspectors;
            inspectors.NewInspector +=
                new Outlook.InspectorsEvents_NewInspectorEventHandler(
                Inspectors_NewInspector);

            currentExplorer = this.Application.ActiveExplorer();

            foreach (Outlook.Inspector inspector in inspectors)
            {
                Inspectors_NewInspector(inspector);
            }
 

            /*
             * create the explorer tagBar
             */
            explorerTagBar = new OutlookTagBar(this, null);
            explorerCustomTaskPane = this.CustomTaskPanes.Add(explorerTagBar, "Explorer Tag Bar");
            explorerCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            explorerCustomTaskPane.Height = 57;
            explorerTagBar.LoadTagList(GetLatestTagList());
            explorerCustomTaskPane.Visible = true;

            // explorer event
            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(CurrentExplorer_SelectionChanged);

            // inspector event
            System.Diagnostics.Debug.Write("In THIS ADDIN STARTUP\n");
        }
        public void AddTagToEmail(String tag, Outlook.MailItem mi)
        {
            Backend.TagEmail(mi.EntryID, tag);
            AddTagToExplorerEmailIfMatch(mi.EntryID, tag);
            foreach (Outlook.Inspector inspector in inspectors)
            {
                AddTagToInspectorEmailIfMatch(inspector, mi.EntryID, tag);
            }
        }
        private void AddTagToInspectorEmailIfMatch(Outlook.Inspector inspector, String entryID, String tag)
        {
            if (inspector.CurrentItem is Outlook.MailItem)
            {
                Outlook.MailItem mailItem = inspector.CurrentItem as Outlook.MailItem;
                if (entryID.Equals(mailItem.EntryID))
                {
                    InspectorWrapper iWrapper = inspectorWrappersValue[inspector];
                    OutlookTagBar otb = iWrapper.getTagBar();
                    otb.AddNewButton(tag);
                }
            }
        }
        private void AddTagToExplorerEmailIfMatch(String entryID, String tag)
        {
            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = this.Application.ActiveExplorer().Selection[1];
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
        public void CreateNewTag(String tag)
        {
            System.Diagnostics.Debug.Write("New label : " + tag + NL);
            Backend.AddTag(tag);
            List<String> latestTags = GetLatestTagList();
            explorerTagBar.LoadTagList(latestTags);
            Dictionary<Outlook.Inspector, InspectorWrapper>.KeyCollection keys = inspectorWrappersValue.Keys;
            foreach (Outlook.Inspector inspector in keys)
            {
                InspectorWrapper iWrapper = inspectorWrappersValue[inspector];
                iWrapper.getTagBar().LoadTagList(latestTags);
            }
        }
        public List<String> GetLatestTagList()
        {
            String json = TagCommon.Backend.AllTags();
            TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
            List<TagName> tagNameList = tagNames.Tags;
            List<String> tags = new List<String>();
            foreach (TagName tag in tagNameList)
            {
                tags.Add(tag.Name);
            }
            tags.Sort();
            return tags;
        }
        private void Inspector_Activated()
        {
            System.Diagnostics.Debug.Write("INSPECTOR Activated...\n");
        }
        private void Inspector_Deactivated()
        {
            System.Diagnostics.Debug.Write("INSPECTOR DEactivated...\n");
        }
        private void Inspectors_NewInspector(Microsoft.Office.Interop.Outlook.Inspector Inspector)
        {
            // this only fires when we open a new window, not when we just single click on an email
            if (Inspector.CurrentItem is Outlook.MailItem)
            {
                Outlook.MailItem mailItem = Inspector.CurrentItem as Outlook.MailItem;
                System.Diagnostics.Debug.Write("NewInspector event fired for mailItem " + mailItem.Subject + " \n");
                
                if (inspectorWrappersValue.ContainsKey(Inspector))
                {
                    System.Diagnostics.Debug.Write("SKIPPING REDUNDANT inspectorWRapper\n");
                }
                else
                {
                    ((Outlook.InspectorEvents_10_Event)Inspector).Activate += new
           Outlook.InspectorEvents_10_ActivateEventHandler(Inspector_Activated);
                    ((Outlook.InspectorEvents_10_Event)Inspector).Deactivate += new
          Outlook.InspectorEvents_10_DeactivateEventHandler(Inspector_Deactivated); ;
                    System.Diagnostics.Debug.Write("CREATING inspectorWrapper\n");
                    inspectorWrappersValue.Add(Inspector, new InspectorWrapper(this, Inspector, mailItem));
                }
                
            }
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

        public static void ExpressTagButtonsFromCategories(OutlookTagBar tagBar, Outlook.MailItem mailItem)
        {
            if (mailItem.Categories != null)
            {
                String categories = mailItem.Categories;
                if (!categories.Equals(""))
                {
                    char[] delims = new char[1];
                    delims[0] = ',';
                    String[] tagNames = categories.Split(delims);
                    List<String> cleanTagNames = CleanTagNames(tagNames);

                    foreach (String tagName in cleanTagNames)
                    {
                        tagBar.AddNewButton(tagName);
                    }
                }
            }
        }
        
        private void CurrentExplorer_SelectionChanged()
        {
            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = this.Application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {

                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;
                        explorerTagBar.SetMostRecentEmailItem(mailItem);
                        explorerTagBar.RefreshTagButtons(mailItem);
                        foreach (Outlook.Inspector inspector in inspectors)
                        {
                            InspectorWrapper iWrapper = inspectorWrappersValue[inspector];
                            OutlookTagBar otb = iWrapper.getTagBar();
                            if (inspector.CurrentItem is Outlook.MailItem)
                            {
                                Outlook.MailItem mi = inspector.CurrentItem as Outlook.MailItem;
                                otb.SetMostRecentEmailItem(mi);
                                if (mi.EntryID.Equals(mailItem.EntryID))
                                {
                                    otb.RefreshTagButtons(mi);
                                }
                            }
                        }
                        String senderName     = mailItem.Sender.Name;
                        Backend.AddPerson(Utils.URLEscapeString(Utils.NormalizeName(senderName)));
                        Backend.ShowPersons();
                        String entryID = mailItem.EntryID;
                        String conversationID = mailItem.ConversationID;
                        Backend.AddEmail(entryID, conversationID);
                        System.Diagnostics.Debug.Write("CurrentExplorer_SelectionChanged FIRED \n");
                    }
                }
            }
            catch (Exception ex)
            {
                String expMessage = ex.Message;
                System.Windows.Forms.MessageBox.Show(expMessage);
            }
        }
       
        
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see http://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }

    public class InspectorWrapper
    {
        private Outlook.Inspector inspector;
        private CustomTaskPane taskPane;
        private Outlook.MailItem mailItem;
        private OutlookTagBar inspectorTagBar;
        public InspectorWrapper(OutlookTagBarAddin addin, Outlook.Inspector Inspector, Outlook.MailItem mailItem)
        {
            this.mailItem = mailItem;
            this.inspector = Inspector;
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            System.Diagnostics.Debug.Write("ADDING taskPane (inspectorTagBar)\n");
            inspectorTagBar = new OutlookTagBar(addin, mailItem);
            inspectorTagBar.LoadTagList(addin.GetLatestTagList());
            taskPane = Globals.OutlookTagBarAddin.CustomTaskPanes.Add(inspectorTagBar, "Inspector Tag Bar", this.inspector);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;
            taskPane.VisibleChanged += new EventHandler(TaskPane_VisibleChanged);
            if (mailItem != null)
            {
                //inspectorTagBar.RemoveAllTagButtons();
                inspectorTagBar.ExpressTagButtonsFromBackend(mailItem);
            }
        }
        public OutlookTagBar getTagBar()
        {
            return this.inspectorTagBar;
        }
        void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            //Globals.Ribbons[inspector].ManageTaskPaneRibbon.toggleButton1.Checked =
           //     taskPane.Visible;
        }

        void InspectorWrapper_Close()
        {
            if (taskPane != null)
            {
                System.Diagnostics.Debug.Write("REMOVING taskPane\n");
                Globals.OutlookTagBarAddin.CustomTaskPanes.Remove(taskPane);
            }

            taskPane = null;
            if (inspector != null)
            {
                System.Diagnostics.Debug.Write("REMOVING InspectorEvents_CloseEventHandler\n");
                Globals.OutlookTagBarAddin.InspectorWrappers.Remove(inspector);
                ((Outlook.InspectorEvents_Event)inspector).Close -=
                    new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);
                
            }
            inspector = null;
        }

        public CustomTaskPane CustomTaskPane
        {
            get
            {
                return taskPane;
            }
        }
    }
}
