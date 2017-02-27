using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using TagCommon;
using NLog;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon1();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace OutlookTagBar
{
    [ComVisible(true)]
    public class Ribbon1 : Office.IRibbonExtensibility
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private Office.IRibbonUI ribbon;

        public Ribbon1()
        {

        }
        public void mySaveAttachments(Office.IRibbonControl control, bool cancelDefault)
        {
            cancelDefault = false;
            /*
            var context = control.Context;

            Outlook.Explorer explorer = Globals.OutlookTagBarAddin.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = item as Outlook.MailItem;
                    Outlook.Attachments attachments = mailItem.Attachments;
                    if (attachments.Count == 0)
                    {
                        cancelDefault = true;
                        return;
                    }
                    StringBuilder sb = new StringBuilder();
                    int i = 1;
                    for (; i < attachments.Count; i++) {
                        Outlook.Attachment a = attachments[i];
                        sb.Append(a.FileName + ",");
                    }
                    sb.Append(attachments[i].FileName);
                    string fileListString = "" + sb;
                    
                    SaveFileDialog sfd = new SaveFileDialog();
                   
                    sfd.Title = "Testing Jed";
                    sfd.FileName = fileListString;
                    sfd.Filter = "All files(*.*) | *.*";

                    sfd.ShowDialog();
                    String nameListString = sfd.FileName;
                    System.Diagnostics.Debug.Write("snameListString : " + nameListString + "\n");
                    string[] names = nameListString.Split(',');
                    for (i = 0; i < names.Length; i++)
                    {
                        string name = names[i];
                        Outlook.Attachment a = attachments[i + 1];
                        System.Diagnostics.Debug.Write("save attachment as : " + name + "\n");
                        a.SaveAsFile(name);
                    }
                    
                    cancelDefault = true;
                }
                else
                {
                    cancelDefault = false;
                }
            }
            else
            {
                cancelDefault = false;
            }
            */
        }
        private bool isContextExplorer(object context)
        {
            try
            {
                Outlook.Explorer insp = (Outlook.Explorer)context;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        private bool isContextInspector(object context)
        {
            try
            {
                Outlook.Inspector insp = (Outlook.Inspector)context;
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        public void mySaveAttachAs(Office.IRibbonControl control, bool cancelDefault)
        {
            /*
            var context = control.Context;
            if (isContextInspector(context))
            {
                Outlook.Inspector insp = (Outlook.Inspector)context;
                //insp.Application.
                var currentItem = insp.CurrentItem;
                if (currentItem is Outlook.MailItem)
                {
                    Outlook.MailItem mi = (Outlook.MailItem)currentItem;
                   
                }
            }
            */
            //string typeName = GetTypeName(control);
            Outlook.Explorer explorer = Globals.OutlookTagBarAddin.Application.ActiveExplorer();
            if (explorer != null && explorer.Selection != null && explorer.Selection.Count > 0)
            {
                object item = explorer.Selection[1];
                if (item is Outlook.MailItem)
                {
                    Outlook.MailItem mailItem = item as Outlook.MailItem;
                    Outlook.Attachments attachments = mailItem.Attachments;
                    Outlook.Attachment a = attachments[1];
                    logger.Debug("a.displayName : " + a.DisplayName + "\n");
                    logger.Debug("a.pathName : " + a.PathName + "\n");
                    logger.Debug("a.fileName : " + a.FileName + "\n");
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Title = "Save Attachment";
                    sfd.FileName = a.FileName;
                    sfd.Filter = "All files(*.*) | *.*";
                    sfd.DefaultExt = System.IO.Path.GetExtension(a.FileName);

                    sfd.ShowDialog();
                    String resourceName = sfd.FileName;

                    logger.Debug("resourceName : " + resourceName + "\n");
                    a.SaveAsFile(sfd.FileName);
                    Backend.AddResource(Utils.RESOURCE_TYPE_FILE, resourceName);
                    Utils.TagResourceForMailItem(mailItem.EntryID, resourceName);
                    //a.SaveAsFile(@"C:\Users\sudo\Downloads");
                    cancelDefault = true;
                }
                else
                {
                    cancelDefault = false;
                }
            }
            else
            {
                cancelDefault = false;
            }
            // nlog
        }
        public static string GetTypeName(object comObj)
        {

            if (comObj == null)
                return String.Empty;

            if (!Marshal.IsComObject(comObj))
                //The specified object is not a COM object
                return String.Empty;

            IDispatch dispatch = comObj as IDispatch;
            if (dispatch == null)
                //The specified COM object doesn't support getting type information
                return String.Empty;

            System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo = null;
            try
            {
                try
                {
                    // obtain the ITypeInfo interface from the object
                    dispatch.GetTypeInfo(0, 0, out typeInfo);
                }
                catch (Exception ex)
                {
                    //Cannot get the ITypeInfo interface for the specified COM object
                    return String.Empty;
                }

                string typeName = "";
                string documentation, helpFile;
                int helpContext = -1;

                try
                {
                    //retrieves the documentation string for the specified type description 
                    typeInfo.GetDocumentation(-1, out typeName, out documentation,
                        out helpContext, out helpFile);
                }
                catch (Exception ex)
                {
                    // Cannot extract ITypeInfo information
                    return String.Empty;
                }
                return typeName;
            }
            catch (Exception ex)
            {
                // Unexpected error
                return String.Empty;
            }
            finally
            {
                if (typeInfo != null) Marshal.ReleaseComObject(typeInfo);
            }
        }
 
        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("OutlookTagBar.Ribbon1.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit http://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
    /// <summary>
    /// Exposes objects, methods and properties to programming tools and other
    /// applications that support Automation.
    /// </summary>
    [ComImport()]
    [Guid("00020400-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IDispatch
    {
        [PreserveSig]
        int GetTypeInfoCount(out int Count);

        [PreserveSig]
        int GetTypeInfo(
            [MarshalAs(UnmanagedType.U4)] int iTInfo,
            [MarshalAs(UnmanagedType.U4)] int lcid,
            out System.Runtime.InteropServices.ComTypes.ITypeInfo typeInfo);

        [PreserveSig]
        int GetIDsOfNames(
            ref Guid riid,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
            string[] rgsNames,
            int cNames,
            int lcid,
            [MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);

        [PreserveSig]
        int Invoke(
            int dispIdMember,
            ref Guid riid,
            uint lcid,
            ushort wFlags,
            ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
            out object pVarResult,
            ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
            IntPtr[] pArgErr);
    }
}
