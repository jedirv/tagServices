using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public class Persons
    {
        List<Person> people;

        public List<Person> People
        {
            get
            {
                return people;
            }

            set
            {
                people = value;
            }
        }
    }
    public class Person
    {
        String name;
        List<DocumentInfo> documentsReceivedFrom;
        List<DocumentInfo> documentsSentTo;
        List<EmailInfo> emailReceivedFrom;
        List<EmailInfo> emailSentTo;

        public List<EmailInfo> EmailSentTo
        {
            get
            {
                return emailSentTo;
            }

            set
            {
                emailSentTo = value;
            }
        }

        public List<EmailInfo> EmailReceivedFrom
        {
            get
            {
                return emailReceivedFrom;
            }

            set
            {
                emailReceivedFrom = value;
            }
        }

        public List<DocumentInfo> DocumentsSentTo
        {
            get
            {
                return documentsSentTo;
            }

            set
            {
                documentsSentTo = value;
            }
        }

        public List<DocumentInfo> DocumentsReceivedFrom
        {
            get
            {
                return documentsReceivedFrom;
            }

            set
            {
                documentsReceivedFrom = value;
            }
        }

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
 
    public class EmailInfo
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
*/