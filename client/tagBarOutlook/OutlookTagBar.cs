﻿using System;
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
        public OutlookTagBar()
        {
            InitializeComponent();
        }
        private String NL = Environment.NewLine;
        private List<Button> tagButtons = new List<Button>();
       

        private void Button1_Click(object sender, EventArgs e)
        {

        }


        public void TagButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            int imageWidth = clickedButton.Image.Width;
            MouseEventArgs mea = e as MouseEventArgs;
            if (mea.Location.X <= imageWidth)
            {
                this.Controls.Remove(clickedButton);
                tagButtons.Remove(clickedButton);
                System.Diagnostics.Debug.Write("button count: " + tagButtons.Count + NL);
                PositionButtons();
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
        private void ButtonAddTag_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.ComboBox cb = this.Controls["comboBox1"] as System.Windows.Forms.ComboBox;
            if (cb.Items.Count > 0)
            {
                String selectionText = cb.SelectedItem.ToString();
                AddNewButton(selectionText);
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
            ToolStripSeparator sep = new ToolStripSeparator();
            pdfItem.DropDownItems.Add(sep);

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
