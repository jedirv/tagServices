using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookTagBar
{ 
    public class GlobalTaggingContext
    {
        private enum Event
        {
            Read,
            Reply,
            ReplyAll,
            Unknown
        }
        private Event mostRecentEvent = Event.Unknown;
        private Outlook.MailItem mostRecentNavigatedToMailItemWithEntryID = null;
        private Outlook.MailItem mostRecentNavigatedToMailItem = null;

       
        public void SetMostRecentNavigatedToMailItem(Outlook.MailItem mailItem)
        {
            mostRecentNavigatedToMailItem = mailItem;
            System.Diagnostics.Debug.Write("### Most recent maybeID -" + mailItem.Subject + "- entryID <" + mailItem.EntryID + "> \n");
            if (null != mailItem.EntryID && !"".Equals(mailItem.EntryID))
            {
                mostRecentNavigatedToMailItemWithEntryID = mailItem;
                System.Diagnostics.Debug.Write("### Most recent yes ID -" + mailItem.Subject + "- entryID <" + mailItem.EntryID + "> \n");
            }
        }
        public Outlook.MailItem GetReplyEmail()
        {
            if (IsReply())
            {
                return mostRecentNavigatedToMailItem;
            }
            else
            {
                return null;
            }
        }

        public Outlook.MailItem GetEmailBeingRepliedTo()
        {
            if (IsReply())
            {
                return mostRecentNavigatedToMailItemWithEntryID;
            }
            else
            {
                return null;
            }
        }

        public Outlook.MailItem GetEmailBeingRead()
        {
            if (IsRead())
            {
                return mostRecentNavigatedToMailItem;
            }
            else
            {
                return null;
            }
        }
        public void SetMostRecentEventReply()
        {
            System.Diagnostics.Debug.Write("### SetMostRecentEventReply()\n");
            mostRecentEvent = Event.Reply;
        }
        public void SetMostRecentEventReplyAll()
        {
            System.Diagnostics.Debug.Write("### SetMostRecentEventReplyAll()\n");
            mostRecentEvent = Event.ReplyAll;
        }
        public void SetMostRecentEventRead()
        {
            System.Diagnostics.Debug.Write("### SetMostRecentEventRead()\n");
            mostRecentEvent = Event.Read;
        }
        public bool IsReply()
        {
            if (mostRecentEvent == Event.Reply)
            {
                System.Diagnostics.Debug.Write("### GLOBAL was reply due to most recentEvent == Event.Reply\n");
                return true;
            }
            string entryID = mostRecentNavigatedToMailItem.EntryID;
            if (null == entryID || "".Equals(entryID))
            {
                System.Diagnostics.Debug.Write("### GLOBAL was reply due to EntryID\n");
                return true;
            }
            return false;
        }
        public bool IsRead()
        {
            string entryID = mostRecentNavigatedToMailItem.EntryID;
            if (null != entryID)
            {
                System.Diagnostics.Debug.Write("### GLOBAL was read due to non-null entryID\n");
                return true;
            }
            return false;
        }
        public bool IsExplorerInit()
        {
            if (null == mostRecentNavigatedToMailItemWithEntryID && null == mostRecentNavigatedToMailItem)
            {
                return true;
            }
            return false;
        }
    }
}
