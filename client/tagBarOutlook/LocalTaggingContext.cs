using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using TagCommon;
using NLog;

namespace OutlookTagBar
{
    public class LocalTaggingContext
    {
        private Logger logger = LogManager.GetCurrentClassLogger();
        private enum State
        {
            ExplorerInit,
            Reply,
            Read,
            Compose, 
            Unknown
        }
        private int contextID = -1;
        private static int contextIDNum = 1;
        private State state = State.Unknown;
        private Outlook.MailItem emailBeingRepliedTo = null;
        private Outlook.MailItem replyEmail = null;
        private Outlook.MailItem emailBeingRead = null;
        public LocalTaggingContext(GlobalTaggingContext globalTaggingContext)
        {
            contextID = contextIDNum++;
            if (globalTaggingContext.IsExplorerInit())
            {
                state = State.ExplorerInit;
            }
            else if (globalTaggingContext.IsReply())
            {
                state = State.Reply;
                replyEmail = globalTaggingContext.GetReplyEmail();
                emailBeingRepliedTo = globalTaggingContext.GetEmailBeingRepliedTo();
            }
            else if (globalTaggingContext.IsRead())
            {
                state = State.Read;
                emailBeingRead = globalTaggingContext.GetEmailBeingRead();
            }
            else
            {
                state = State.Compose;// TODO = not sure if this is correct - just putting for now
            }
            logger.Debug("$$$ context " + contextID +  " state " + state + " \n");
        }
        
        public Outlook.MailItem GetTagNameSourceMailItem()
        {
            if (isReply())
            {
                return GetEmailBeingRepliedTo();
            }
            else if (isRead())
            {
                return GetEmailBeingRead();
            }
            else
            {
                throw new TagServicesException("GetTagNameSourceMailItem only implemented for replay and read case so far");
            }
        }
        public Outlook.MailItem GetEmailBeingRead()
        {
            if (!isRead())
            {
                throw new TagServicesException("getEmailBeingRead called when not in read context");
            }
            return this.emailBeingRead;
        }
        public Outlook.MailItem GetReplyEmail()
        {
            if (!isReply())
            {
                throw new TagServicesException("getReplyEmail called when not in reply context");
            }
            return this.replyEmail;
        }
        public Outlook.MailItem GetEmailBeingRepliedTo()
        {
            if (!isReply())
            {
                throw new TagServicesException("getEmailBeingRepliedTo called when not in reply context");
            }
            return this.emailBeingRepliedTo;
        }
        public bool isReply()
        {
            
            if (state == State.Reply)
            {
                return true;
            }
            return false;
        }
        public bool isRead()
        {
            if (state == State.Read)
            {
                return true;
            }
            return false;
        }
        public bool isCompose()
        {
            if (state == State.Compose)
            {
                return true;
            }
            return false;
        }
        public bool isExplorerInit()
        {
            if (state == State.ExplorerInit)
            {
                return true;
            }
            return false;
        }
    }
}
