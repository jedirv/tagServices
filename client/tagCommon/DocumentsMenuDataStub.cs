using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class DocumentsMenuDataStub
    {
        public String GetData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"RelevantDocuments\":[{\"Name\":\"docA1\"},{\"Name\":\"docA2\"},{\"Name\":\"docA3\"}],");
            sb.Append("\"MruDocuments\":[{\"Name\":\"docA4\"},{\"Name\":\"docA5\"},{\"Name\":\"docA6\"}],");
            sb.Append("}");

            return "" + sb;
        }
    }
}
/*
 * {"RelevantDocuments":[{"Name":"documentA1"},{"Name":"documentA2"},{"Name":"documentA3"}],"MruDocuments":[{"Name":"documentA4"},{"Name":"documentA5"},{"Name":"documentA6"}]}new button name: tagButton1

 *  {
 *      "documents": 
 *          {
 *              "RelevantDocuments": [ { "Name":"docA1"   }, { "Name":"docA2"   }, { "Name":"docA3"   } ] ,
 *              "MruDocuments" : [ { "Name":"docA4"   }, { "Name":"docA5"   }, { "Name":"docA6"   } ] ,
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
