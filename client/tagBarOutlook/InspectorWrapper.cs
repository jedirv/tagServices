﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools;
using TagCommon;

namespace OutlookTagBar
{
    public class InspectorWrapper
    {
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
        private OutlookTagBar inspectorTagBar;
        public InspectorWrapper(OutlookTagBarAddin addin, Outlook.Inspector Inspector, Outlook.MailItem mailItem)
        {
            this.mailItem = mailItem;
            this.inspector = Inspector;
            ((Outlook.InspectorEvents_Event)inspector).Close +=
                new Outlook.InspectorEvents_CloseEventHandler(InspectorWrapper_Close);

            System.Diagnostics.Debug.Write("ADDING taskPane (inspectorTagBar)\n");
            inspectorTagBar = new OutlookTagBar(addin, new LocalTaggingContext(addin.GetGlobalTaggingContext()), false);
            inspectorTagBar.LoadTagList(Utils.GetLatestTagList());
            taskPane = Globals.OutlookTagBarAddin.CustomTaskPanes.Add(inspectorTagBar, "Inspector Tag Bar", this.inspector);
            taskPane.DockPosition = Office.MsoCTPDockPosition.msoCTPDockPositionTop;
            taskPane.Height = 57;
            taskPane.Visible = true;
            taskPane.VisibleChanged += new EventHandler(TaskPane_VisibleChanged);
        }
        public OutlookTagBar getTagBar()
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
                System.Diagnostics.Debug.Write("REMOVING taskPane\n");
                Globals.OutlookTagBarAddin.CustomTaskPanes.Remove(taskPane);
            }

            taskPane = null;
            if (inspector != null)
            {
                System.Diagnostics.Debug.Write("REMOVING InspectorEvents_CloseEventHandler\n");
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
