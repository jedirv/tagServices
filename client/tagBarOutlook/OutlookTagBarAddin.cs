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
        private GlobalTaggingContext globalTaggingContext = new GlobalTaggingContext();
        public GlobalTaggingContext GetGlobalTaggingContext()
        {
            return this.globalTaggingContext;
        }
        protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            return new Ribbon1();
        }
        public Outlook.Application InPlayApplication
        {
            get
            {
                return this.Application;
            }
        }
        public OutlookTagBar ExplorerTagBar
        {
            get
            {
                return explorerTagBar;
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
            explorerTagBar = new OutlookTagBar(this, new LocalTaggingContext(this.globalTaggingContext));
            explorerCustomTaskPane = this.CustomTaskPanes.Add(explorerTagBar, "Explorer Tag Bar");
            explorerCustomTaskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            explorerCustomTaskPane.Height = 57;
            explorerTagBar.LoadTagList(Utils.GetLatestTagList());
            explorerCustomTaskPane.Visible = true;

            // explorer event
            currentExplorer = this.Application.ActiveExplorer();
            currentExplorer.SelectionChange += new Outlook.ExplorerEvents_10_SelectionChangeEventHandler(CurrentExplorer_SelectionChanged);
            

            // inspector event
            System.Diagnostics.Debug.Write("In THIS ADDIN STARTUP\n");
            //Application.PickerDialog.Properties.
            //Application.PickerDialog.Show(false);
           
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
            try
            {
                // this only fires when we open a new window, not when we just single click on an email
                if (Inspector.CurrentItem is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = Inspector.CurrentItem as Outlook.MailItem;
                    this.globalTaggingContext.SetMostRecentNavigatedToMailItem(mailItem);
                    //mailItem.BeforeAttachmentSave += MailItem_BeforeAttachmentSave;
                    //mailItem.BeforeAttachmentRead += MailItem_BeforeAttachmentRead;
                    HookEventHandlersToMailItem(mailItem);


                    // ((Outlook.ItemEvents_10_Event)mailItem).Send += new Outlook.ItemEvents_10_SendEventHandler(MailItem_Send);
                    System.Diagnostics.Debug.Write("NewInspector event fired for mailItem " + mailItem.Subject + " \n");

                    if (InspectorWrapper.inspectorWrappersValue.ContainsKey(Inspector))
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
                        InspectorWrapper.inspectorWrappersValue.Add(Inspector, new InspectorWrapper(this, Inspector, mailItem));
                    }

                }
            }
            catch(Exception e)
            {
                String expMessage = e.Message;
                System.Windows.Forms.MessageBox.Show(expMessage + "\n" + e.StackTrace);
            }
            
        }
        private void HookEventHandlersToMailItem(Outlook.MailItem mailItem)
        {
            ((Outlook.ItemEvents_10_Event)mailItem).Reply -= new Outlook.ItemEvents_10_ReplyEventHandler(MailItem_Reply);
            ((Outlook.ItemEvents_10_Event)mailItem).Reply += new Outlook.ItemEvents_10_ReplyEventHandler(MailItem_Reply);

            ((Outlook.ItemEvents_10_Event)mailItem).ReplyAll -= new Outlook.ItemEvents_10_ReplyAllEventHandler(MailItem_ReplyAll);
            ((Outlook.ItemEvents_10_Event)mailItem).ReplyAll += new Outlook.ItemEvents_10_ReplyAllEventHandler(MailItem_ReplyAll);

            ((Outlook.ItemEvents_10_Event)mailItem).BeforeRead -= new Outlook.ItemEvents_10_BeforeReadEventHandler(MailItem_Read);
            ((Outlook.ItemEvents_10_Event)mailItem).BeforeRead += new Outlook.ItemEvents_10_BeforeReadEventHandler(MailItem_Read);
        }
        /*
        private void MailItem_Send(ref bool cancel)
        {
            Outlook.MailItem mailItem = (Outlook.MailItem)
            
            System.Diagnostics.Debug.Write("sending email: " +mailItem.Subject + "\n");
            string entryID = mailItem.EntryID;
            System.Diagnostics.Debug.Write("Send entryID is " + entryID + "\n");
            string u = mailItem.Subject;

        }*/
        private void MailItem_Reply(Object response, ref bool cancel)
        {
            Outlook.MailItem mi = response as Outlook.MailItem;
            this.globalTaggingContext.SetMostRecentNavigatedToMailItem(mi);
            this.globalTaggingContext.SetMostRecentEventReply();
        }
        private void MailItem_ReplyAll(Object response, ref bool cancel)
        {
            this.globalTaggingContext.SetMostRecentEventReplyAll();
        }
        private void MailItem_Read()
        {
            this.globalTaggingContext.SetMostRecentEventRead();
        }

        private void MailItem_BeforeAttachmentRead(Outlook.Attachment Attachment, ref bool Cancel)
        {
            System.Diagnostics.Debug.Write("called MailItem_BeforeAttachmentRead\n");
        }

       

        private void MailItem_BeforeAttachmentSave(Outlook.Attachment Attachment, ref bool Cancel)
        {
            System.Diagnostics.Debug.Write("called MailItem_BeforeAttachmentSave\n");
        }

        private void CurrentExplorer_SelectionChanged()
        {
            System.Diagnostics.Debug.Write("CurrentExplorer_SelectionChanged event fired\n");
            try
            {
                if (this.Application.ActiveExplorer().Selection.Count > 0)
                {
                    Object selObject = this.Application.ActiveExplorer().Selection[1];
                    if (selObject is Outlook.MailItem)
                    {

                        Outlook.MailItem mailItem = selObject as Outlook.MailItem;
                        this.globalTaggingContext.SetMostRecentNavigatedToMailItem(mailItem);

                        HookEventHandlersToMailItem(mailItem);

                        //explorerTagBar.SetMostRecentEmailItem(mailItem);
                        
                        explorerTagBar.SetLocalTaggingContext(new LocalTaggingContext(this.globalTaggingContext));
                        inspectors = this.Application.Inspectors;
                        foreach (Outlook.Inspector inspector in inspectors)
                        {
                            InspectorWrapper iWrapper = InspectorWrapper.inspectorWrappersValue[inspector];
                            OutlookTagBar otb = iWrapper.getTagBar();
                            if (otb.GetTagBasisMailItem().EntryID.Equals(mailItem.EntryID))
                            {
                                otb.RefreshTagButtons();
                            }
                            /*if (inspector.CurrentItem is Outlook.MailItem)
                            {
                                Outlook.MailItem mi = inspector.CurrentItem as Outlook.MailItem;
                                //otb.SetMostRecentEmailItem(mi);
                                if (mi.EntryID.Equals(mailItem.EntryID))
                                {
                                    otb.RefreshTagButtons();
                                }
                            }*/
                        }
                        String senderName     = mailItem.Sender.Name;
                        Backend.AddPerson(Utils.NormalizeName(senderName));
                        Backend.ShowPersons();
                        String entryID = mailItem.EntryID;
                        String conversationID = mailItem.ConversationID;
                        Backend.AddEmail(entryID, conversationID);
                        System.Diagnostics.Debug.Write("CurrentExplorer_SelectionChanged FIRED \n");
                    }
                }
            }
            catch (Exception e)
            {
                String expMessage = e.Message;
                System.Windows.Forms.MessageBox.Show(expMessage + "\n" + e.StackTrace);
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

    
}
