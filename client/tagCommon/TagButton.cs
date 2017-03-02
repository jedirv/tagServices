using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace TagCommon
{
    public class TagButton : Button
    {
        public TagButton(string text) {
            this.Image = Image.FromFile(@"..\..\Close_icon-16-square.png");
            this.TextImageRelation = TextImageRelation.ImageBeforeText;
            this.ImageAlign = ContentAlignment.MiddleLeft;
            this.TextAlign = ContentAlignment.MiddleRight;
            this.Text = text;
            this.AutoSize = true;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 1;
            this.FlatAppearance.BorderColor = Color.DarkGray;
        }
        
    }
}
