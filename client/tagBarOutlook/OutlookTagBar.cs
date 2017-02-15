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

using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using TagCommon;

namespace OutlookTagBar
{
    public partial class OutlookTagBar : UserControl
    {
        public OutlookTagBar(OutlookTagBarAddin addin, Outlook.MailItem mailItem)
        {
            this.addin = addin;
            InitializeComponent();
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
                AddNewButton(tag.Name);
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
        public void AddNewButton(String name)
        {
            if (!IsButtonAlreadyPresent(name))
            {
                Button newButton = CreateButton(name);
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
        public Button CreateButton(String text)
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
            AddMenusToButtonFromBackend(newButton);
            return newButton;
        }
        private void AddMenusFromJson(Button b, String json)
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
                AttachOpenAndAttachMenusToDocName(item, di.Name);
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
                AttachOpenAndAttachMenusToDocName(item, di.Name);
            }
            menuStrip.Items.Add(pdfItem);
            b.ContextMenuStrip = menuStrip;
        }
        private void AddMenusToButtonFromBackend(Button b)
        {
            String json = Backend.DocsForTag(b.Text);
            AddMenusFromJson(b,json);
        }
        
        private void AddMenusToButtonFromStub(Button b)
        {
            DocumentsMenuDataStub dataStub = new DocumentsMenuDataStub();
            String json = dataStub.GetData();
            AddMenusFromJson(b,json);
        }
        private void AddAttachment()
        {
            /*Outlook.MailItem mail =
                this.Application.CreateItem
                (Outlook.OlItemType.olMailItem)
                as Outlook.MailItem;

            mail.Subject = "An attachment for you!";
            */
            OpenFileDialog attachment = new OpenFileDialog();

            attachment.Title = "Select a file to send";
            attachment.InitialDirectory = @"C:\Users\sudo";
            attachment.FileName = "junk.txt";
            attachment.ShowDialog();
        }
        private void AttachOpenAndAttachMenusToDocName(ToolStripMenuItem item, String docPath)
        {
            ToolStripMenuItem attach = new ToolStripMenuItem();
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
                String path = senderMenuItem.Text;
                System.Diagnostics.Debug.Write("attaching file : " + path + NL);
                AddAttachment();
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
            cb.SelectedIndex = 0;
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
