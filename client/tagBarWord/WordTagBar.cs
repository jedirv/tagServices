using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using TagCommon;

namespace WordButtonTest
{
    public partial class WordTagBar : UserControl
    {
        private String NL = Environment.NewLine;
        private List<Button> tagButtons = new List<Button>();
        public WordTagBar()
        {
            InitializeComponent();
        }

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
                //System.Diagnostics.Debug.Write("button count: " + tagButtons.Count + NL);
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
                    //System.Diagnostics.Debug.Write("curOriginX " + curOriginX + " originY " + 0 + " width " + width + NL);
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
            /*
            System.Windows.Forms.ComboBox cb = this.Controls["comboBox1"] as System.Windows.Forms.ComboBox;
            if (cb.Items.Count > 0)
            {
                String selectionText = cb.SelectedItem.ToString();
                AddNewButton(selectionText);
            }
            */
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
                //loadPersonsMenuOnTag(newButton);
            LoadDocumentsMenuOnTag(newButton);
                //persistDocumentsJson();
            AddMenusToButton(newButton);
            return newButton;
        }
        /*
         * string json = @"{
  'Name': 'Bad Boys',
  'ReleaseDate': '1995-4-7T00:00:00',
  'Genres': [
    'Action',
    'Comedy'
  ]
}";

Movie m = JsonConvert.DeserializeObject<Movie>(json);

string name = m.Name;

         * */
        private void LoadDocumentsMenuOnTag(Button b)
        {
            DocumentsMenuDataStub dataStub = new DocumentsMenuDataStub();
            String json = dataStub.GetData();

            Documents docs = TagCommon.Utils.GetDocumentsForJson(json);
            foreach (DocumentInfo di in docs.RelevantDocuments)
            {
                System.Diagnostics.Debug.Write("deserialized relevant doc : " + di.Name + "\n");
            }

            foreach (DocumentInfo di in docs.MruDocuments)
            {
                System.Diagnostics.Debug.Write("deserialized recent doc : \n" + di.Name + "\n");
            }
        }
        private void LoadPersonsMenuOnTag(Button b)
        {
            PersonsMenuDataStub dataStub = new PersonsMenuDataStub();
            String json = dataStub.GetData();

            Persons persons = TagCommon.Utils.GetPersonsForJson(json);
            System.Diagnostics.Debug.Write("deserialized Persons : " + persons.People);

            /*
            Persons persons = new Persons();
            List<Person> peopleList = new List<Person>();
            Person nephro = getDummyPerson("Nephro");
            Person forrest = getDummyPerson("Forrest");
            peopleList.Add(nephro);
            peopleList.Add(forrest);
            persons.People = peopleList;
            string output = JsonConvert.SerializeObject(persons);
            System.Diagnostics.Debug.Write("serialized Persons : \n\n" + output);
            */
        }
        private void PersistDocumentsJson()
        {
            Documents docs = new TagCommon.Documents();
            List<DocumentInfo> docsRelevant = new List<DocumentInfo>();
            DocumentInfo docA1 = new DocumentInfo();
            DocumentInfo docA2 = new DocumentInfo();
            DocumentInfo docA3 = new DocumentInfo();
            docA1.Name = "documentA1";
            docA2.Name = "documentA2";
            docA3.Name = "documentA3";
            docsRelevant.Add(docA1);
            docsRelevant.Add(docA2);
            docsRelevant.Add(docA3);
            docs.RelevantDocuments = docsRelevant;

            List<DocumentInfo> docsRecent = new List<DocumentInfo>();
            DocumentInfo docA4 = new DocumentInfo();
            DocumentInfo docA5 = new DocumentInfo();
            DocumentInfo docA6 = new DocumentInfo();
            docA4.Name = "documentA4";
            docA5.Name = "documentA5";
            docA6.Name = "documentA6";
            docsRecent.Add(docA4);
            docsRecent.Add(docA5);
            docsRecent.Add(docA6);
            docs.MruDocuments = docsRecent;

            string output = TagCommon.Utils.SerializeObjectToString(docs);
            System.Diagnostics.Debug.Write("serialized Documents : \n\n" + output);
        }
        private Person GetDummyPerson(String name)
        {
            Person person = new TagCommon.Person();
            person.Name = name;

            List<DocumentInfo> docsFrom = new List<DocumentInfo>();
            DocumentInfo docA1 = new DocumentInfo();
            DocumentInfo docA2 = new DocumentInfo();
            DocumentInfo docA3 = new DocumentInfo();
            docA1.Name = "documentA1";
            docA2.Name = "documentA2";
            docA3.Name = "documentA3";
            docsFrom.Add(docA1);
            docsFrom.Add(docA2);
            docsFrom.Add(docA3);
            person.DocumentsReceivedFrom = docsFrom;

            List<DocumentInfo> docsTo = new List<DocumentInfo>();
            DocumentInfo docA4 = new DocumentInfo();
            DocumentInfo docA5 = new DocumentInfo();
            DocumentInfo docA6 = new DocumentInfo();
            docA4.Name = "documentA4";
            docA5.Name = "documentA5";
            docA6.Name = "documentA6";
            docsTo.Add(docA4);
            docsTo.Add(docA5);
            docsTo.Add(docA6);
            person.DocumentsSentTo = docsTo;


            List<EmailInfo> emailFrom = new List<EmailInfo>();
            EmailInfo emailA1 = new EmailInfo();
            EmailInfo emailA2 = new EmailInfo();
            EmailInfo emailA3 = new EmailInfo();
            emailA1.Name = "emailA1";
            emailA2.Name = "emailA2";
            emailA3.Name = "emailA3";
            emailFrom.Add(emailA1);
            emailFrom.Add(emailA2);
            emailFrom.Add(emailA3);
            person.EmailReceivedFrom = emailFrom;

            List<EmailInfo> emailTo = new List<EmailInfo>();
            EmailInfo emailA4 = new EmailInfo();
            EmailInfo emailA5 = new EmailInfo();
            EmailInfo emailA6 = new EmailInfo();
            emailA4.Name = "emailA4";
            emailA5.Name = "emailA5";
            emailA6.Name = "emailA6";
            emailTo.Add(emailA4);
            emailTo.Add(emailA5);
            emailTo.Add(emailA6);
            person.EmailSentTo = emailTo;
            return person;
        }
        private void AddMenusToButton(Button b)
        {
            ContextMenuStrip menuStrip = new ContextMenuStrip();
            ToolStripMenuItem pdfItem = new ToolStripMenuItem();
            pdfItem.Text = "Save as PDF";

            ToolStripMenuItem item1 = new ToolStripMenuItem();
            item1.Text = "folder1";
            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.Text = "folder2";
            ToolStripMenuItem item3 = new ToolStripMenuItem();
            item3.Text = "folder3";
            ToolStripSeparator sep = new ToolStripSeparator();
            ToolStripMenuItem itemMRU1 = new ToolStripMenuItem();
            itemMRU1.Text = "MRU folder1";
            ToolStripMenuItem itemMRU2 = new ToolStripMenuItem();
            itemMRU2.Text = "MRU folder2";
            ToolStripMenuItem itemMRU3 = new ToolStripMenuItem();
            itemMRU3.Text = "MRU folder3";
            pdfItem.DropDownItems.Add(item1);
            pdfItem.DropDownItems.Add(item2);
            pdfItem.DropDownItems.Add(item3);
            pdfItem.DropDownItems.Add(sep);
            pdfItem.DropDownItems.Add(itemMRU1);
            pdfItem.DropDownItems.Add(itemMRU2);
            pdfItem.DropDownItems.Add(itemMRU3);
            menuStrip.Items.Add(pdfItem);
            b.ContextMenuStrip = menuStrip;
        }
    }
}
