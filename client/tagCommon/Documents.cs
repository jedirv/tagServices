using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class Documents
    {
        List<DocumentInfo> relevantDocuments;
        List<DocumentInfo> mruDocuments;

        public List<DocumentInfo> RelevantDocuments
        {
            get
            {
                return relevantDocuments;
            }

            set
            {
                relevantDocuments = value;
            }
        }

        public List<DocumentInfo> MruDocuments
        {
            get
            {
                return mruDocuments;
            }

            set
            {
                mruDocuments = value;
            }
        }
    }
    public class DocumentInfo
    {
        String name;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
    }
}
/*
 * 
 *  {
 *      "documents": 
 *          {
 *              "RelatedDocuments": [ { "Name":"docA1"   }, { "Name":"docA2"   }, { "Name":"docA3"   } ] ,
 *              "RecentDocuments" : [ { "Name":"docA4"   }, { "Name":"docA5"   }, { "Name":"docA6"   } ] ,
 *          }    
 *  }       
 * 
 * 
 * 
 * 
(tag)
	documents
		(doc)
			open
			attach
		(*doc)
			open
			attach
*/
