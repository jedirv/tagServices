using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using TagCommon;
using NLog;

namespace OutlookTagBar
{
    public class InspectorWrapper
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        public static Dictionary<Outlook.Inspector, InspectorWrapper> inspectorWrappersValue =
            new Dictionary<Outlook.Inspector, InspectorWrapper>();
       
        public static Dictionary<Outlook.Inspector, InspectorWrapper> InspectorWrappers
        {
            get
            {
                return inspectorWrappersValue;
            }
        }

        private Outlook.Inspector inspector;
        private CustomTaskPane taskPane;
        private Outlook.MailItem mailItem;
        private TagBar inspectorTagBar;
        private OutlookTagBarDecorator inspectorTagBarDecorator;
        public InspectorWrapper(OutlookTagBarAddin addin, Outlook.Inspector Inspector, Outlook.MailItem mailItem)
        {
            this.mailItem = mailItem;
            this.inspector = Inspector;
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            logger.Info("ADDING taskPane (inspectorTagBar)\n");

            

            inspectorTagBar = new TagBar();
            inspectorTagBarDecorator = new OutlookTagBarDecorator(addin, inspectorTagBar, new LocalTaggingContext(addin.GetGlobalTaggingContext()));
            inspectorTagBar.SetTagBarHelper(this.inspectorTagBarDecorator);
            inspectorTagBar.LoadTagList(Utils.GetLatestTagList());
            taskPane = Globals.OutlookTagBarAddin.CustomTaskPanes.Add(inspectorTagBar, "Inspector Tag Bar", this.inspector);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;
            taskPane.VisibleChanged += new EventHandler(TaskPane_VisibleChanged);
        }
        public TagBar getTagBar()
        {
            return this.inspectorTagBar;
        }
        void TaskPane_VisibleChanged(object sender, EventArgs e)
        {
            //Globals.Ribbons[inspector].ManageTaskPaneRibbon.toggleButton1.Checked =
            //     taskPane.Visible;
        }

        void InspectorWrapper_Close()
        {
            if (taskPane != null)
            {
                logger.Info("REMOVING taskPane\n");
                Globals.OutlookTagBarAddin.CustomTaskPanes.Remove(taskPane);
            }

            taskPane = null;
            if (inspector != null)
            {
                logger.Info("REMOVING InspectorEvents_CloseEventHandler\n");
                InspectorWrappers.Remove(inspector);
                ((Outlook.InspectorEvents_Event)inspector).Close -=
                    new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            }
            inspector = null;
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
