using System;
using System.Collections.Generic;
using Word = Microsoft.Office.Interop.Word;
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
        private WordTagBarDecorator primaryTagBarDecorator;
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            //https://msdn.microsoft.com/en-us/library/microsoft.office.interop.word(v=office.15).aspx

            //this.Application.DocumentOpen   += new Word.ApplicationEvents4_DocumentOpenEventHandler(DocumentOpenHandler);
            // use the windowActivate handler because it fires at a time when we can catch it during startup
            this.Application.WindowActivate += new Word.ApplicationEvents4_WindowActivateEventHandler(WindowActivateHandler);
            this.Application.DocumentBeforeClose   += new Word.ApplicationEvents4_DocumentBeforeCloseEventHandler(DocumentCloseHandler);
            this.Application.DocumentOpen += new Word.ApplicationEvents4_DocumentOpenEventHandler(DocumentOpenHandler);
            //this.Application.DocumentBeforeSave += new Word.ApplicationEvents4_DocumentBeforeSaveEventHandler(DocumentBeforeSaveHandler);
            //Word.ApplicationEvents4_event.NewDocument = += new Word.ApplicationEvents4_NewDocumentEventHandler(DocumentBeforeSaveHandler);
            //this.Application.NewDocument += new Word.ApplicationEvents4_NewDocumentEventHandler(DocumentBeforeSaveHandler);
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
                TagBar tagBar = new TagBar();
                this.primaryTagBarDecorator = new WordTagBarDecorator();
                tagBar.SetTagBarHelper(this.primaryTagBarDecorator);
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
            /*
            foreach (String tagName in tags)
            {
                tagBar.AddNewButton(tagName);
            }
            */
        }
    }
   

       
    
}
