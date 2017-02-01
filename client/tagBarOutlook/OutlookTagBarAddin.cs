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

            foreach (Outlook.Inspector inspector in inspectors)
            {
                Inspectors_NewInspector(inspector);
            }
 

            /*
             * create the explorer tagBar
             */
            explorerTagBar = new OutlookTagBar();
            explorerCustomTaskPane = this.CustomTaskPanes.Add(explorerTagBar, "Explorer Tag Bar");
            explorerCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            explorerCustomTaskPane.Height = 57;
            System.Windows.Forms.ComboBox cb = explorerTagBar.Controls["comboBox1"] as System.Windows.Forms.ComboBox;

            cb.Items.Add("teaching\\cs500");
            cb.Items.Add("teaching\\cs501");
            cb.Items.Add("teaching\\cs502");
            cb.Items.Add("teaching\\cs503");
            cb.Items.Add("teaching\\cs504");
            cb.Items.Add("teaching\\cs505");
            cb.Items.Add("teaching\\cs506");
            cb.Items.Add("teaching\\cs507");
            // cb.Items.Add("word\\tags\\control render testing\\implement remove tag");
            cb.SelectedIndex = 1;
            explorerCustomTaskPane.Visible = true;

            // explorer event
            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(CurrentExplorer_Event);

            // inspector event
            System.Diagnostics.Debug.Write("In THIS ADDIN STARTUP\n");
            inspectors = this.Application.Inspectors;
            inspectors.NewInspector +=
            new Microsoft.Office.Interop.Outlook.InspectorsEvents_NewInspectorEventHandler(Inspectors_NewInspector);
            
        
        }
       
        void Inspectors_NewInspector(Microsoft.Office.Interop.Outlook.Inspector Inspector)
        {
            if (Inspector.CurrentItem is Outlook.MailItem)
            {
                System.Diagnostics.Debug.Write("NewInspector event fired\n");
                Outlook.MailItem mailItem = Inspector.CurrentItem as Outlook.MailItem;
                if (inspectorWrappersValue.ContainsKey(Inspector))
                {
                    System.Diagnostics.Debug.Write("SKIPPING REDUNDANT inspectorWRapper\n");
                }
                else
                {
                    System.Diagnostics.Debug.Write("CREATING inspectorWrapper\n");
                    inspectorWrappersValue.Add(Inspector, new InspectorWrapper(Inspector, mailItem));
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
        public static void ExpressTagButtonsFromBackend(OutlookTagBar tagBar, Outlook.MailItem mailItem)
        {
            String entryID = "00001";
            String json = TagCommon.Backend.GetJsonFromBackend("tagapi/tagsForEmail/" + entryID);
            TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
            List<TagName> tags = tagNames.Tags;
            foreach (TagName tag in tags)
            {
                tagBar.AddNewButton(tag.Name);
            }
        }
        private void CurrentExplorer_Event()
        {
            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = this.Application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {
                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;
                        explorerTagBar.RemoveAllTagButtons();
                        ExpressTagButtonsFromBackend(explorerTagBar, mailItem);
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
        public InspectorWrapper(Outlook.Inspector Inspector, Outlook.MailItem mailItem)
        {
            this.mailItem = mailItem;
            this.inspector = Inspector;
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            System.Diagnostics.Debug.Write("ADDING taskPane (inspectorTagBar)\n");
            OutlookTagBar inspectorTagBar = new OutlookTagBar();
            taskPane = Globals.OutlookTagBarAddin.CustomTaskPanes.Add(inspectorTagBar, "Inspector Tag Bar", this.inspector);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;
            taskPane.VisibleChanged += new EventHandler(TaskPane_VisibleChanged);
            if (mailItem != null)
            {
                //inspectorTagBar.RemoveAllTagButtons();
                OutlookTagBarAddin.ExpressTagButtonsFromBackend(inspectorTagBar, mailItem);
            }
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
