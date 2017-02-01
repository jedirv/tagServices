using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class PersonsMenuDataStub
    {
        public String GetData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append("\"People\":[");
            sb.Append("     {");
            sb.Append("         \"Name\":\"Forrest\",");
            sb.Append("         \"DocumentsReceivedFrom\":[{\"Name\":\"docA1\"},{\"Name\":\"docA2\"},{\"Name\":\"docA3\"}],");
            sb.Append("         \"DocumentsSentTo\":[{\"Name\":\"docA4\"},{\"Name\":\"docA5\"},{\"Name\":\"docA6\"}],");
            sb.Append("         \"EmailReceivedFrom\":[{\"Name\":\"emailA1from\"},{\"Name\":\"emailA2from\"},{\"Name\":\"emailA3from\"}],");
            sb.Append("         \"EmailSentTo\":[{\"Name\":\"emailA4to\"},{\"Name\":\"emailA5to\"},{\"Name\":\"emailA6to\"}]");
            sb.Append("     },");
            sb.Append("     {");
            sb.Append("         \"Name\":\"Nephro\",");
            sb.Append("         \"DocumentsReceivedFrom\":[{\"Name\":\"docB1\"},{\"Name\":\"docB2\"},{\"Name\":\"docB3\"}],");
            sb.Append("         \"DocumentsSentTo\":[{\"Name\":\"docB4\"},{\"Name\":\"docB5\"},{\"Name\":\"docB6\"}],");
            sb.Append("         \"EmailReceivedFrom\":[{\"Name\":\"emailB1from\"},{\"Name\":\"emailB2from\"},{\"Name\":\"emailB3from\"}],");
            sb.Append("         \"EmailSentTo\":[{\"Name\":\"emailB4to\"},{\"Name\":\"emailB5to\"},{\"Name\":\"emailB6to\"}]");
            sb.Append("     }");
            sb.Append("]}");

            return "" + sb;
        }
    }
}
/*
 * 
 *  {
 *      "persons": [
 *          {
 *              "name": "personAAA",
 *              "documentsRecievedFrom": [ { "name":"docA1"   }, { "name":"docA2"   }, { "name":"docA3"   } ] ,
 *              "documentsSentTo":       [ { "name":"docA4"   }, { "name":"docA5"   }, { "name":"docA6"   } ] ,
 *              "emailReceievedFrom":    [ { "name":"emailA1from" }, { "name":"emailA2from" }, { "name":"emailA3from" } ] ,       
 *              "emailSentTo":           [ { "name":"emailA4to" }, { "name":"emailA5to" }, { "name":"emailA6to" } ]
 *          } ,    
 *          {
 *              "name": "personBBB",
 *              "documentsRecievedFrom": [ { "name":"docB1"   }, { "name":"docB2"   }, { "name":"docB3"   } ] ,
 *              "documentsSentTo":       [ { "name":"docB4"   }, { "name":"docB5"   }, { "name":"docB6"   } ] ,
 *              "emailReceievedFrom":    [ { "name":"emailB1from" }, { "name":"emailB2from" }, { "name":"emailB3from" } ] ,       
 *              "emailSentTo":           [ { "name":"emailB4to" }, { "name":"emailB5to" }, { "name":"emailB6to" } ]
 *          }    
 * 
 * 
 * 
 * 
(person)
            documentsRecievedFrom
                (doc)
                    open
            documentsSentTo
                (doc)
                    open
            emailRecievedFrom
                (email)
                    open
            emailSentTo
                (email)
                    open
*/
