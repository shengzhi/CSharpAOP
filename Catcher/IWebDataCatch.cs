using System;
using System.Collections.Generic;
using System.Text;

namespace Basic.Catcher
{
    public interface IWebDataCatcher
    {
        string GetString(string urlstr, string refer);

        string Post(string url, string refer, string data);
    }
}
