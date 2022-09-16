using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TfGuiTool.Utils
{
    internal class SimpleLogUtils
    {
        const string LOG = "log.txt";

        static public void Write(string log)
        {
            if (!File.Exists(LOG)) File.Create(LOG).Close();
            File.AppendAllText(LOG, "LOG START\r\n" + DateTime.Now.ToString() + "\r\n");
            File.AppendAllText(LOG, log + "\r\n");
            File.AppendAllText(LOG, "LOG END\r\n");
        }
    }
}
