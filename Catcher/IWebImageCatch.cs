using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Basic.Catcher
{
    interface IWebImageCatcher
    {
        Bitmap GetImg(string url, string refer);
    }
}
