using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using TagCommon;
using System.Drawing;
using System.Diagnostics;
using NLog;
using System.IO;

namespace OutlookTagBar
{
    public class OutlookTagBarDecorator : TagBarHelper
    {

        private string contextID;
        private String NL = Environment.NewLine;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private TagBar tagBar;
        private OutlookTagBarContext localTaggingContext;
        private OutlookTagBarAddin addin;
        public OutlookTagBarDecorator(OutlookTagBarAddin addin, TagBar tagBar, OutlookTagBarContext localTaggingContext)
        {
            this.addin = addin;
            this.tagBar = tagBar;
            SetLocalTaggingContext(localTaggingContext);
            
        }
        public string GetContextID()
        {
            return this.contextID;
        }
        public void SetContextID(string ID)
        {
            this.contextID = ID;
        }

        public void Status(String s)
        {
            //TextBox tb = this.Controls["textBox1"] as TextBox;
            //tb.Text = s;
        }
        public void SetLocalTaggingContext(OutlookTagBarContext context)
        {
            this.localTaggingContext = context;

            if (context.isRead())
            {
                SetContextID(context.GetEmailBeingRead().EntryID);
                RefreshTagButtons();
                Status("read...");
            }
            else if (context.isReply())
            {
                SetContextID(context.GetEmailBeingRepliedTo().EntryID);
                RefreshTagButtons();
                Status("reply...");
            }
            else if (context.isExplorerInit())
            {
                Status("expl init...");
            }
            else if (context.isCompose())
            {
                throw new TagServicesException("isCompose case Not Yet Implemented for OutlookTag Bar");
            }
        }
        public void ExpressTagButtonsFromBackend(OutlookTagBarContext localTaggingContext)
        {
            if (localTaggingContext.isRead())
            {
                string entryID = localTaggingContext.GetEmailBeingRead().EntryID;
                string json = TagCommon.Backend.TagsForEmail(entryID);
                TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
                List<TagName> tags = tagNames.Tags;
                foreach (TagName tag in tags)
                {
                    AddNewButton(tag.Name, localTaggingContext);
                }
            }
            else if (localTaggingContext.isReply())
            {
                string entryID = localTaggingContext.GetEmailBeingRepliedTo().EntryID;
                string json = TagCommon.Backend.TagsForEmail(entryID);
                TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
                List<TagName> tags = tagNames.Tags;
                foreach (TagName tag in tags)
                {
                    AddNewButton(tag.Name, localTaggingContext);
                }
            }
            else if (localTaggingContext.isExplorerInit())
            {
                // skip it - the CurrentExplorer_SelectionChanged event will trigger the refresh in a bit 
            }
            else if (localTaggingContext.isCompose())
            {
                throw new TagServicesException("refreshTagButtons not implemented for Compose state");
            }


        }
        public void AddNewButton(string name)
        {
            AddNewButton(name, this.localTaggingContext);
        }
        public void AddNewButton(String name, OutlookTagBarContext localTaggingContext)
        {
            if (!this.tagBar.IsButtonAlreadyPresent(name))
            {
                Button newButton = CreateButton(name, localTaggingContext);
                this.tagBar.AddAndPositionTagButton(newButton);
                
            }
        }
        public Button CreateButton(String text, OutlookTagBarContext localTaggingContext)
        {
            Button newButton = new TagButton(text);
            newButton.Click += new EventHandler(TagButton_Click);
            if (localTaggingContext.isReply())
            {
                AddMenusToButtonFromBackend(newButton, localTaggingContext.GetReplyEmail());
            }
            else if (localTaggingContext.isRead())
            {
                AddAttachmentsMenu(newButton, localTaggingContext.GetEmailBeingRead());
            }
            else
            {
                throw new TagServicesException("create tag button only implemented for read, reply so far");
            }
            return newButton;
        }
        public void TagButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int imageWidth = clickedButton.Image.Width;
            MouseEventArgs mea = e as MouseEventArgs;
            if (mea.Location.X <= imageWidth)
            {
                OutlookTagUtils.RemoveTagFromEmail(clickedButton.Text, localTaggingContext.GetTagNameSourceMailItem(), this.addin.InPlayApplication, this.addin.ExplorerTagBarHelper);
            }
        }

        private void AddAttachmentsMenu(Button b, Outlook.MailItem mailItem)
        {
            ContextMenuStrip menuStrip = b.ContextMenuStrip;
            if (null == menuStrip)
            {
                menuStrip = new ContextMenuStrip();
                b.ContextMenuStrip = menuStrip;
            }
            Outlook.Attachments attachments = mailItem.Attachments;
            if (attachments.Count == 0)
            {
                ToolStripMenuItem noAttachmentsItem = new ToolStripMenuItem();
                noAttachmentsItem.Text = "(No Attachments)";
                noAttachmentsItem.ForeColor = Color.DarkGray;
                noAttachmentsItem.Enabled = true;
                noAttachmentsItem.Click += new System.EventHandler(this.NoAttachmentsMenuItem_Click);
                menuStrip.Items.Add(noAttachmentsItem);
            }
            else
            {
                ToolStripMenuItem attachmentsItem = new ToolStripMenuItem();
                attachmentsItem.Text = "Attachments";
                menuStrip.Items.Add(attachmentsItem);

                // make the save all attachments item and pass info through Tag
                ToolStripMenuItem saveAllAttachmentItem = new ToolStripMenuItem();
                saveAllAttachmentItem.Text = "Save All";
                saveAllAttachmentItem.Click += new System.EventHandler(this.SaveAllAttachmentsMenuItem_Click);
                MailItemAttachments mias = new MailItemAttachments();
                foreach (Outlook.Attachment att in attachments)
                {
                    mias.Add(new MailItemAttachment(mailItem, att));
                }
                saveAllAttachmentItem.Tag = mias;
                attachmentsItem.DropDownItems.Add(saveAllAttachmentItem);

                // make an entry for each attachment to be saved individually and pass info through Tag
                foreach (Outlook.Attachment att in attachments)
                {
                    ToolStripMenuItem attachmentItem = new ToolStripMenuItem();
                    attachmentItem.Text = att.DisplayName;

                    ToolStripMenuItem saveAttachmentItem = new ToolStripMenuItem();
                    saveAttachmentItem.Text = "Save";
                    saveAttachmentItem.Tag = new MailItemAttachment(mailItem, att);
                    saveAttachmentItem.Click += new System.EventHandler(this.SaveAttachmentMenuItem_Click);
                    attachmentItem.DropDownItems.Add(saveAttachmentItem);

                    ToolStripMenuItem saveAndOpenAttachmentItem = new ToolStripMenuItem();
                    saveAndOpenAttachmentItem.Text = "Save and Open";
                    saveAndOpenAttachmentItem.Tag = new MailItemAttachment(mailItem, att);
                    saveAndOpenAttachmentItem.Click += new System.EventHandler(this.SaveAndOpenAttachmentMenuItem_Click);
                    attachmentItem.DropDownItems.Add(saveAndOpenAttachmentItem);

                    attachmentsItem.DropDownItems.Add(attachmentItem);
                }
            }

        }
        public void SaveAttachmentMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                MailItemAttachment mia = (MailItemAttachment)senderMenuItem.Tag;
                Outlook.MailItem mailItem = mia.GetMailItem();
                Outlook.Attachment att = mia.GetAttachment();
                string resourceName = ShowDialogAndGetPath(senderMenuItem, att);
                SaveAndTagFile(resourceName, att, mailItem.EntryID);
            }
        }

        public void SaveAndOpenAttachmentMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                MailItemAttachment mia = (MailItemAttachment)senderMenuItem.Tag;
                Outlook.MailItem mailItem = mia.GetMailItem();
                Outlook.Attachment att = mia.GetAttachment();
                string resourceName = ShowDialogAndGetPath(senderMenuItem, att);
                SaveAndTagFile(resourceName, att, mailItem.EntryID);
                Process.Start(resourceName);
            }
        }

        private void AddMenusFromJson(Button b, String json, Outlook.MailItem mailItem)
        {
            TagCommon.Documents docs = TagCommon.Utils.GetDocumentsForJson(json);
            List<DocumentInfo> relevantDocs = docs.RelevantDocuments;
            List<DocumentInfo> mruDocs = docs.MruDocuments;

            ContextMenuStrip menuStrip = new ContextMenuStrip();
            ToolStripMenuItem pdfItem = new ToolStripMenuItem();
            pdfItem.Text = "Documents";

            foreach (DocumentInfo di in relevantDocs)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = di.Name;
                pdfItem.DropDownItems.Add(item);
                AttachOpenAndAttachMenusToDocName(item, di.Name, mailItem);
            }
            /*
             * RE-ENGAGE THIS CODE IF WE ADD BACK IN MRUs
             * if (relevantDocs.Count > 0 && mruDocs.Count > 0)
            {
                ToolStripSeparator sep = new ToolStripSeparator();
                pdfItem.DropDownItems.Add(sep);
            }
            

            foreach (DocumentInfo di in mruDocs)
            {
                ToolStripMenuItem item = new ToolStripMenuItem();
                item.Text = "*" + di.Name;
                pdfItem.DropDownItems.Add(item);
                AttachOpenAndAttachMenusToDocName(item, di.Name, mailItem);
            }
            */
            menuStrip.Items.Add(pdfItem);
            b.ContextMenuStrip = menuStrip;
        }
        private void AddMenusToButtonFromBackend(Button b, Outlook.MailItem mailItem)
        {
            String json = Backend.DocsForTag(b.Text);
            AddMenusFromJson(b, json, mailItem);
        }


        private void AddMenusToButtonFromStub(Button b, Outlook.MailItem mailItem)// leave this in for testing in case needed
        {
            DocumentsMenuDataStub dataStub = new DocumentsMenuDataStub();
            String json = dataStub.GetData();
            AddMenusFromJson(b, json, mailItem);
        }

        private void AttachOpenAndAttachMenusToDocName(ToolStripMenuItem item, String docPath, Outlook.MailItem mailItem)
        {
            ToolStripMenuItem attach = new ToolStripMenuItem();
            attach.Tag = mailItem;
            ToolStripMenuItem open = new ToolStripMenuItem();
            open.Enabled = false;
            attach.Text = "Attach";
            open.Text = "Open";
            attach.Click += new System.EventHandler(this.AttachFileMenuItem_Click);
            open.Click += new System.EventHandler(this.OpenFileMenuItem_Click);
            item.DropDownItems.Add(attach);
            item.DropDownItems.Add(open);
        }

        public void AttachFileMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                Outlook.MailItem mi = (Outlook.MailItem)senderMenuItem.Tag;
                String path = senderMenuItem.OwnerItem.Text;
                logger.Debug("attaching file : " + path + NL);
                mi.Attachments.Add(path, Outlook.OlAttachmentType.olByValue, System.Reflection.Missing.Value, Path.GetFileName(path));
            }
        }

        public void OpenFileMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                String path = senderMenuItem.Text;
                logger.Debug("opening file : " + path + NL);
            }
        }
        public void RefreshTagButtons()
        {
            logger.Debug("calling RefreshTagButtons KEEP context on OTB " + this.tagBar.TagBarID + NL);
            this.tagBar.RemoveAllTagButtons();
            ExpressTagButtonsFromBackend(this.localTaggingContext);
        }
        public void NoAttachmentsMenuItem_Click(object sender, EventArgs e)
        {
            // NOOP
        }
        public void SaveAllAttachmentsMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = fbd.ShowDialog();
                string foldername = fbd.SelectedPath;
                MailItemAttachments mias = (MailItemAttachments)senderMenuItem.Tag;
                List<MailItemAttachment> miaList = mias.GetMIAs();
                foreach (MailItemAttachment mia in miaList)
                {
                    Outlook.Attachment att = mia.GetAttachment();
                    Outlook.MailItem mi = mia.GetMailItem();
                    logger.Debug("would save attachment : " + att.FileName + " for " + mi.EntryID + NL);
                    String path = Path.Combine(foldername, att.FileName);
                    att.SaveAsFile(path);
                    Backend.AddResource(Utils.RESOURCE_TYPE_FILE, path);
                    Utils.TagResourceForMailItem(mi.EntryID, path);
                }
            }
        }
        private string ShowDialogAndGetPath(ToolStripMenuItem menuItem, Outlook.Attachment att)
        {
            String attachmentName = att.DisplayName;
            logger.Debug("saving attachment : " + attachmentName + NL);
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save Attachment";
            sfd.FileName = att.FileName;
            sfd.Filter = "All files(*.*) | *.*";
            sfd.DefaultExt = System.IO.Path.GetExtension(att.FileName);

            sfd.ShowDialog();
            String resourceName = sfd.FileName;
            return resourceName;
        }
        private void SaveAndTagFile(string resourceName, Outlook.Attachment att, string entryID)
        {
            logger.Debug("resourceName : " + resourceName + "\n");
            att.SaveAsFile(resourceName);
            Backend.AddResource(Utils.RESOURCE_TYPE_FILE, resourceName);
            Utils.TagResourceForMailItem(entryID, resourceName);
        }

        public void AssociateTagWithCurrentResource(string tag)
        {
            Outlook.MailItem mi = this.localTaggingContext.GetTagNameSourceMailItem();
            OutlookTagUtils.AddTagToEmail(tag, mi, this.addin.InPlayApplication, this.addin.ExplorerTagBarHelper);
        }

        public void CreateNewTag(string tagName)
        {
            OutlookTagUtils.CreateNewTag(tagName, this.addin.Application, this.addin.ExplorerTagBar);
        }

        public void RemoveTagButton(string tag)
        {
            this.tagBar.RemoveTagButton(tag);
        }
    }
}
