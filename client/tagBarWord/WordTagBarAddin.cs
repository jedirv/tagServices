using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using System.Windows.Forms;
using TagCommon;

using Microsoft.Office.Tools;

/*
 *  when document opens, we'll need to find the right tags for that document
 *  and put them in the relevant tagBar
 *  we'll be creating the tagBar for each open so no problem there.
 *  when document new
 */
namespace WordButtonTest
{
    public partial class WordTagBarAddin
    {
        //https://msdn.microsoft.com/en-us/library/microsoft.office.interop.word.aspx for list of delegates and events
        private Dictionary<Word.Document, DocumentWindowWrapper> wordWrappersDict =
           new Dictionary<Word.Document, DocumentWindowWrapper>();
        public Dictionary<Word.Document, DocumentWindowWrapper> WordWrappers
        {
            get
            {
                return wordWrappersDict;
            }
        }
        private WordTagBar myPane;
        private TagCommon.TagNameSource tagNameSource = new TagCommon.TagNameSource();
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //this.Application.DocumentOpen   += new Word.ApplicationEvents4_DocumentOpenEventHandler(DocumentOpenHandler);
            // use the windowActivate handler because it fires at a time when we can catch it during startup
            this.Application.WindowActivate += new Word.ApplicationEvents4_WindowActivateEventHandler(WindowActivateHandler);
            this.Application.DocumentBeforeClose   += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(DocumentCloseHandler);
        }
        void WindowActivateHandler(Word.Document doc, Word.Window window)
        {
            if (!(wordWrappersDict.ContainsKey(doc)))
            {
                ListCaptionsForTaskPaneWindows("before");
                // remove redundant TaskPane if present
                String caption = GetCaptionStringFromDoc(doc);
                RemoveTaskPanesIfTheirWindowHasThisCaption(caption);

                List<String> tags = tagNameSource.GetNextTags(caption);
                WordTagBar tagBar = new WordTagBar();
                DocumentWindowWrapper dww = new DocumentWindowWrapper();
                CustomTaskPane ctp = dww.Wrap(this, doc, tags, tagBar);
               
                wordWrappersDict.Add(doc, dww);
                System.Diagnostics.Debug.Write("window caption for new taskPane " + window.Caption + "\n");
                ListCaptionsForTaskPaneWindows("after");
            }
        }
        String GetCaptionStringFromDoc(Word.Document doc)
        {
            String name = doc.Name;
            char[] delims = { '.' };
            String[] nameParts = name.Split(delims);
            String caption = nameParts[0];
            return caption;
        }
        void RemoveTaskPanesIfTheirWindowHasThisCaption(String caption)
        {
            String normalizedCaption = caption.Replace(" [Compatibility Mode]","");
            List<CustomTaskPane> redundantTaskPanes = new List<CustomTaskPane>();
            for (int i = this.CustomTaskPanes.Count; i > 0; i--)
            {
                CustomTaskPane curTp = this.CustomTaskPanes[i - 1];
                Word.Window curTpWindow = (Word.Window)curTp.Window;
                String curTpWindowCaption = curTpWindow.Caption;
                String normalizedcurTpWindowCaption = curTpWindowCaption.Replace(" [Compatibility Mode]", "");
                if (normalizedcurTpWindowCaption.Equals(normalizedCaption))
                {
                    redundantTaskPanes.Add(curTp);
                }
            }
            foreach (CustomTaskPane tp in redundantTaskPanes)
            {
                this.CustomTaskPanes.Remove(tp);
            }
        }
        void DocumentOpenHandler(Word.Document doc)
        {
            System.Diagnostics.Debug.Write("DocumentOpen event fired\n");
        }
        void DocumentCloseHandler(Word.Document doc, ref bool Cancel)
        {
            System.Diagnostics.Debug.Write("DocumentClose event fired\n");
        }
        private void ListCaptionsForTaskPaneWindows(String context)
        {
            try
            {
                for (int i = this.CustomTaskPanes.Count; i > 0; i--)
                {
                    CustomTaskPane ctp = this.CustomTaskPanes[i - 1];
                    Word.Window ctpWindow = (Word.Window)ctp.Window;
                    System.Diagnostics.Debug.Write(context + "   i " + i + " tcp WindowCaption: " + ctpWindow.Caption + "\n");
                }
            }
            catch(Exception ex)
            {

            }
            
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
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
       

        public static void ExpressTagButtons(WordTagBar tagBar, List<String> tags)
        {
            foreach (String tagName in tags)
            {
                tagBar.AddNewButton(tagName);
            }
        }
    }
    public class DocumentWindowWrapper
    {
        private Word.Document document;
        private CustomTaskPane taskPane;
        private List<String> tags;
        public DocumentWindowWrapper()
        {
            
        }
        public CustomTaskPane Wrap(WordTagBarAddin addin, Word.Document Doc, List<String> tags, WordTagBar tagBar)
        {
            this.document = Doc;
            this.tags = tags;
            Document vstoDoc = Globals.Factory.GetVstoObject(addin.Application.ActiveDocument);
            //System.Diagnostics.Debug.Write("ADDING taskPane (inspectorTagBar)\n");
            taskPane = Globals.WordTagBarAddin.CustomTaskPanes.Add(tagBar, "Tag Bar", this.document.ActiveWindow);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;
            
            WordTagBarAddin.ExpressTagButtons(tagBar, tags);
            System.Windows.Forms.ComboBox cb = tagBar.Controls["comboBox1"] as System.Windows.Forms.ComboBox;

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
