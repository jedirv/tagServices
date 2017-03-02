using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagCommon
{
    public interface TagBarHelper
    {
        void AssociateTagWithCurrentResource(string s);
        void CreateNewTag(string s);

        string GetContextID();
        void SetContextID(string id);

        void RefreshTagButtons();

        void RemoveTagButton(string tag);

        void AddNewButton(string tag);
    }
}
