using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using NLog;

namespace OutlookTagBar
{ 
    public class OutlookState
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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
            logger.Debug("### Most recent maybeID -" + mailItem.Subject + "- entryID <" + mailItem.EntryID + "> \n");
            if (null != mailItem.EntryID && !"".Equals(mailItem.EntryID))
            {
                mostRecentNavigatedToMailItemWithEntryID = mailItem;
                logger.Debug("### Most recent yes ID -" + mailItem.Subject + "- entryID <" + mailItem.EntryID + "> \n");
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
            logger.Debug("### SetMostRecentEventReply()\n");
            mostRecentEvent = Event.Reply;
        }
        public void SetMostRecentEventReplyAll()
        {
            logger.Debug("### SetMostRecentEventReplyAll()\n");
            mostRecentEvent = Event.ReplyAll;
        }
        public void SetMostRecentEventRead()
        {
            logger.Debug("### SetMostRecentEventRead()\n");
            mostRecentEvent = Event.Read;
        }
        public bool IsReply()
        {
            if (mostRecentEvent == Event.Reply)
            {
                logger.Debug("### GLOBAL was reply due to most recentEvent == Event.Reply\n");
                return true;
            }
            string entryID = mostRecentNavigatedToMailItem.EntryID;
            if (null == entryID || "".Equals(entryID))
            {
                logger.Debug("### GLOBAL was reply due to EntryID\n");
                return true;
            }
            return false;
        }
        public bool IsRead()
        {
            string entryID = mostRecentNavigatedToMailItem.EntryID;
            if (null != entryID)
            {
                logger.Debug("### GLOBAL was read due to non-null entryID\n");
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
