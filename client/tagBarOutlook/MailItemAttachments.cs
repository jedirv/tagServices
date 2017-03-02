using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OutlookTagBar
{
    public class MailItemAttachments
    {
        List<MailItemAttachment> mias = new List<MailItemAttachment>();
        private Outlook.Attachment attachment;
        public MailItemAttachments()
        {
        }
        public void Add(MailItemAttachment mia)
        {
            this.mias.Add(mia);
        }
        public List<MailItemAttachment> GetMIAs()
        {
            return this.mias;
        }
    }
}
