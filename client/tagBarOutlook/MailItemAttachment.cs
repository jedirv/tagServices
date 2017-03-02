using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Outlook = Microsoft.Office.Interop.Outlook;
namespace OutlookTagBar
{
    public class MailItemAttachment
    {
        private Outlook.MailItem mailItem;
        private Outlook.Attachment attachment;
        public MailItemAttachment(Outlook.MailItem mailItem, Outlook.Attachment attachment)
        {
            this.mailItem = mailItem;
            this.attachment = attachment;
        }
        public Outlook.MailItem GetMailItem()
        {
            return this.mailItem;
        }
        public Outlook.Attachment GetAttachment()
        {
            return this.attachment;
        }
    }
}
