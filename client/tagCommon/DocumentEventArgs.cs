using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class DocumentEventArgs : EventArgs
    {
        private String path;
        public DocumentEventArgs(String path)
        {
            this.path = path;
        }
        public String getPath()
        {
            return this.path;
        }
    }
}
