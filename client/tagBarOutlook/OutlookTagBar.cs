using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using TagCommon;

namespace OutlookTagBar
{
    public partial class OutlookTagBar : UserControl
    {
        private static int tagBarIDSource = 0;
        private int tagBarID = -1;
        public OutlookTagBar(OutlookTagBarAddin addin, Outlook.MailItem mailItem)
        {
            this.addin = addin;
            InitializeComponent();
            tagBarID = tagBarIDSource;
            tagBarIDSource += 1;
        }
        private String NL = Environment.NewLine;
        private List<Button> tagButtons = new List<Button>();
        private OutlookTagBarAddin addin;
        private Outlook.MailItem mostRecentMailItem;  

        private void Button1_Click(object sender, EventArgs e)
        {

        }

        public void SetMostRecentEmailItem(Outlook.MailItem mailItem)
        {
            System.Diagnostics.Debug.Write("tagBar " + tagBarID + " now has email " + mailItem.EntryID + " with subject " + mailItem.Subject + NL);
            this.mostRecentMailItem = mailItem;
        }
        private Outlook.MailItem GetMostRecentEmailItem()
        {
            return this.mostRecentMailItem;
        }
        public void TagButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int imageWidth = clickedButton.Image.Width;
            MouseEventArgs mea = e as MouseEventArgs;
            if (mea.Location.X <= imageWidth)
            {
                OutlookTagUtils.RemoveTagFromEmail(clickedButton.Text, GetMostRecentEmailItem(), this.addin.InPlayApplication, this.addin.ExplorerTagBar);
            }
        }

        public void PositionButtons()
        {
            if (tagButtons.Count > 0)
            {
                int curOriginX = 300;
                foreach (Button button in tagButtons)
                {
                    int width = button.Size.Width;
                    System.Diagnostics.Debug.Write("curOriginX " + curOriginX + " originY " + 0 + " width " + width + NL);
                    Point newLocation = new Point(curOriginX, 0);
                    button.Location = newLocation;
                    curOriginX += width;
                    curOriginX += 5;
                }
            }
        }

        private bool IsButtonAlreadyPresent(String s)
        {
            foreach (Button button in tagButtons)
            {
                if (button.Text.Equals(s))
                {
                    return true;
                }
            }
            return false;
        }
        public void RefreshTagButtons(Outlook.MailItem mailItem)
        {
            RemoveAllTagButtons();
            ExpressTagButtonsFromBackend(mailItem);
        }
        public void ExpressTagButtonsFromBackend(Outlook.MailItem mailItem)
        {
            String entryID = mailItem.EntryID;
            String json = TagCommon.Backend.TagsForEmail(entryID);
            TagNames tagNames = TagCommon.Utils.GetTagNamesForJson(json);
            List<TagName> tags = tagNames.Tags;
            foreach (TagName tag in tags)
            {
                AddNewButton(tag.Name, mailItem);
            }
        }
        private void ButtonAddTag_Click(object sender, EventArgs e)
        {
            ComboBox cb = this.Controls["comboBoxTags"] as ComboBox;
            if (cb.Items.Count > 0)
            {
                String tag = cb.SelectedItem.ToString();
                Outlook.MailItem mi = GetMostRecentEmailItem();
                OutlookTagUtils.AddTagToEmail(tag, mi, this.addin.InPlayApplication, this.addin.ExplorerTagBar);
            }
        }
       
        public void RemoveTagButton(String tagName)
        {
            Button buttonToRemove = null;
            foreach (Button b in tagButtons)
            {
                if (b.Text.Equals(tagName))
                {
                    buttonToRemove = b;
                }
            }
            if (null != buttonToRemove)
            {
                this.Controls.Remove(buttonToRemove);
                tagButtons.Remove(buttonToRemove);
                System.Diagnostics.Debug.Write("button count: " + tagButtons.Count + NL);
                PositionButtons();
            }
        }
        public void AddNewButton(String name, Outlook.MailItem mailItem)
        {
            if (!IsButtonAlreadyPresent(name))
            {
                Button newButton = CreateButton(name, mailItem);
                this.Controls.Add(newButton);
                tagButtons.Add(newButton);
                PositionButtons();
            }
        }
        public void RemoveAllTagButtons()
        {
            foreach (Button button in tagButtons)
            {
                this.Controls.Remove(button);
            }
            tagButtons.Clear();
        }
        public Button CreateButton(String text, Outlook.MailItem mailItem)
        {
            Button newButton = new Button();
            newButton.Image = Image.FromFile("C:\\Users\\sudo\\Downloads\\Close_icon-16-square.png");
            newButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            newButton.ImageAlign = ContentAlignment.MiddleLeft;
            newButton.TextAlign = ContentAlignment.MiddleRight;
            newButton.Click += new EventHandler(TagButton_Click);
            String newButtonName = "tagButton" + (tagButtons.Count);
            System.Diagnostics.Debug.Write("new button name: " + newButtonName + NL);
            newButton.Name = newButtonName;
            newButton.Text = text;
            newButton.AutoSize = true;
            newButton.FlatStyle = FlatStyle.Flat;
            newButton.FlatAppearance.BorderSize = 1;
            newButton.FlatAppearance.BorderColor = Color.DarkGray;
            //addMenusToButtonFromStub(newButton);
            AddMenusToButtonFromBackend(newButton, mailItem);
            AddAttachmentsMenu(newButton, mailItem);
            return newButton;
        }
        private void AddAttachmentsMenu(Button b, Outlook.MailItem mailItem)
        {
            ContextMenuStrip menuStrip = b.ContextMenuStrip;
            if (null == menuStrip)
            {
                menuStrip = new ContextMenuStrip();
                b.ContextMenuStrip = menuStrip;
            }
            ToolStripMenuItem attachmentsItem = new ToolStripMenuItem();
            attachmentsItem.Text = "Attachments";
            menuStrip.Items.Add(attachmentsItem);
            Outlook.Attachments attachments = mailItem.Attachments;

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
                attachmentsItem.DropDownItems.Add(attachmentItem);
            }
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
                    System.Diagnostics.Debug.Write("would save attachment : " + att.FileName + " for " + mi.EntryID + NL);
                    String path = Path.Combine(foldername ,att.FileName);
                    att.SaveAsFile(path);
                    Backend.AddResource(Utils.RESOURCE_TYPE_FILE, path);
                    Utils.TagResourceForMailItem(mi.EntryID, path);
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
                String attachmentName = att.DisplayName;
                System.Diagnostics.Debug.Write("saveing attachment : " + attachmentName + NL);
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "Save Attachment";
                sfd.FileName = att.FileName;
                sfd.Filter = "All files(*.*) | *.*";
                sfd.DefaultExt = System.IO.Path.GetExtension(att.FileName);

                sfd.ShowDialog();
                String resourceName = sfd.FileName;

                System.Diagnostics.Debug.Write("resourceName : " + resourceName + "\n");
                att.SaveAsFile(sfd.FileName);
                Backend.AddResource(Utils.RESOURCE_TYPE_FILE, resourceName);
                Utils.TagResourceForMailItem(mailItem.EntryID, resourceName);
            }
        }
        private void AddMenusFromJson(Button b, String json, Outlook.MailItem mailItem)
        {
            Documents docs = TagCommon.Utils.GetDocumentsForJson(json);
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
            if (relevantDocs.Count > 0 && mruDocs.Count > 0)
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
            menuStrip.Items.Add(pdfItem);
            b.ContextMenuStrip = menuStrip;
        }
        private void AddMenusToButtonFromBackend(Button b, Outlook.MailItem mailItem)
        {
            String json = Backend.DocsForTag(b.Text);
            AddMenusFromJson(b,json, mailItem);
        }
        
        private void AddMenusToButtonFromStub(Button b, Outlook.MailItem mailItem)
        {
            DocumentsMenuDataStub dataStub = new DocumentsMenuDataStub();
            String json = dataStub.GetData();
            AddMenusFromJson(b,json, mailItem);
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
                System.Diagnostics.Debug.Write("attaching file : " + path + NL);
                mi.Attachments.Add(path, Outlook.OlAttachmentType.olByValue, System.Reflection.Missing.Value, Path.GetFileName(path));
            }
        }
        public void OpenFileMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem senderMenuItem = sender as ToolStripMenuItem;
            if (senderMenuItem != null)
            {
                String path = senderMenuItem.Text;
                System.Diagnostics.Debug.Write("opening file : " + path + NL);
            }
        }


        private void NewTagKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    OutlookTagUtils.CreateNewTag(((TextBox)sender).Text, this.addin.Application, this.addin.ExplorerTagBar);
                    break;
            }
        }


        public void LoadTagList(List<String> latestTags)
        {
            ComboBox cb = this.Controls["comboBoxTags"] as ComboBox;
            cb.Items.Clear();
            foreach (String tag in latestTags)
            {
                cb.Items.Add(tag);
            }
            if (cb.Items.Count > 0) {
                cb.SelectedIndex = 0;
            }
        }
    }
    public class MailItemAttachment
    {
        private Outlook.MailItem mailItem;
        private Outlook.Attachment attachment;
        public MailItemAttachment(Outlook.MailItem mailItem, Outlook.Attachment attachment)
        {
            this.mailItem = mailItem;
            this.attachment = attachment;
        }
        public Outlook.MailItem GetMailItem()
        {
            return this.mailItem;
        }
        public Outlook.Attachment GetAttachment()
        {
            return this.attachment;
        }
    }
    public class MailItemAttachments
    {
        List<MailItemAttachment> mias = new List<MailItemAttachment>();
        private Outlook.Attachment attachment;
        public MailItemAttachments()
        {
        }
        public void Add(MailItemAttachment mia)
        {
            this.mias.Add(mia);
        }
        public List<MailItemAttachment> GetMIAs()
        {
            return this.mias;
        }
    }
    public class DocumentEventArgs : EventArgs
    {
        private String path;
        public DocumentEventArgs(String path)
        {
            this.path = path;
        }
        public String getPath()
        {
            return this.path;
        }
    }
}
