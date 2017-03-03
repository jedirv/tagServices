using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TagCommon;
using NLog;

namespace TagCommon
{
    public partial class TagBar : UserControl
    {
        private String NL = Environment.NewLine;
        private List<Button> tagButtons = new List<Button>();
        private Logger logger = LogManager.GetCurrentClassLogger();
        private static int tagBarIDSource = 0;
        private int tagBarID = -1;
        private TagBarHelper tagBarHelper;
        public TagBar()
        {
            InitializeComponent();
            tagBarID = tagBarIDSource;
            tagBarIDSource += 1;
        }
        public int TagBarID
        {
            get
            {
                return this.tagBarID;
            }
        }
        public void SetTagBarHelper(TagBarHelper helper)
        {
            this.tagBarHelper = helper;
        }
        public TagBarHelper TagBarHelper
        {
            get
            {
                return this.tagBarHelper;
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
                    logger.Debug("curOriginX " + curOriginX + " originY " + 0 + " width " + width + NL);
                    System.Drawing.Point newLocation = new System.Drawing.Point(curOriginX, 0);
                    button.Location = newLocation;
                    curOriginX += width;
                    curOriginX += 5;
                }
            }
        }

        public bool IsButtonAlreadyPresent(String s)
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
      
        
        
        private void AddTag_Click(object sender, EventArgs e)
        {
            ComboBox cb = this.Controls["comboBoxTags"] as ComboBox;
            if (cb.Items.Count > 0)
            {
                String tag = cb.SelectedItem.ToString();
                if (!"".Equals(tag))
                {
                    this.tagBarHelper.AssociateTagWithCurrentResource(tag);
                }
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
        public void AddAndPositionTagButton(Button b)
        {
            string newButtonName = "tagButton" + (tagButtons.Count);
            this.Name = newButtonName;
            this.Controls.Add(b);
            tagButtons.Add(b);
            PositionButtons();
        }
        
        
        
        
        

        private void NewTagKeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    string newTagName = ((TextBox)sender).Text;
                    this.tagBarHelper.CreateNewTag(newTagName);
                    TextBox tb = sender as TextBox;
                    tb.Text = "";
                    break;
            }
        }

        public void LoadTagList(List<String> latestTags)
        {
            ComboBox cb = this.Controls["comboBoxTags"] as ComboBox;
            cb.Items.Clear();
            cb.Items.Add("");
            foreach (String tag in latestTags)
            {
                cb.Items.Add(tag);
            }
            if (cb.Items.Count > 0) {
                cb.SelectedIndex = 0;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
    
    
   
}
