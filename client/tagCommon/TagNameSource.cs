using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    
    public class TagNameSource
    {
        private int index = 1;
        public TagNameSource()
        {
        
        }
        public List<String> GetNextTags(String caption)
        {
            List<String> list1 = new List<String>();
            list1.Add(caption + "tag" + index + "a");
            list1.Add(caption + "tag" + index + "b");
            list1.Add(caption + "tag" + index + "c");
            index++;
            return list1;
        }
    }
}
