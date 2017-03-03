using System;
using System.Collections.Generic;
using Word = Microsoft.Office.Interop.Word;
using Microsoft.Office.Tools;
using Microsoft.Office.Tools.Word;
using Office = Microsoft.Office.Core;
using TagCommon;

namespace WordButtonTest
{
    public class DocumentWindowWrapper
    {
        private Word.Document document;
        private CustomTaskPane taskPane;
        private List<String> tags;
        public DocumentWindowWrapper()
        {

        }
        public CustomTaskPane Wrap(WordTagBarAddin addin, Word.Document Doc, List<String> tags, TagBar tagBar)
        {
            this.document = Doc;
            this.tags = tags;
            Document vstoDoc = Globals.Factory.GetVstoObject(addin.Application.ActiveDocument);
            //System.Diagnostics.Debug.Write("ADDING taskPane (inspectorTagBar)\n");
            taskPane = Globals.WordTagBarAddin.CustomTaskPanes.Add(tagBar, "Tag Bar", this.document.ActiveWindow);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;

            tagBar.TagBarHelper.RefreshTagButtons();
            System.Windows.Forms.ComboBox cb = tagBar.Controls["comboBox1"] as System.Windows.Forms.ComboBox;

            /*
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
            */
            return taskPane;
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
