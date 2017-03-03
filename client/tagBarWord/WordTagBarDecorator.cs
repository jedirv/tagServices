using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagCommon;

namespace WordButtonTest
{
    public class WordTagBarDecorator : TagBarHelper
    {
        private string contextID;

        public void AssociateTagWithCurrentResource(string s)
        {

        }
        public void CreateNewTag(string s)
        {

        }

        public string GetContextID()
        {
            return this.contextID;
        }
        public void SetContextID(string id)
        {
            this.contextID = id;
        }

        public void RefreshTagButtons()
        {

        }

        public void RemoveTagButton(string tag)
        {

        }

        public void AddNewButton(string tag)
        {

        }
    }
}
