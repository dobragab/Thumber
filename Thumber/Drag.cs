using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Thumber
{
    public static class Drag
    {
        static public bool HasFiles(this DragEventArgs lhs)
        {
            return lhs.Data.GetDataPresent(DataFormats.FileDrop);
        }
        static public string[] GetFiles(this DragEventArgs lhs)
        {
            string[] data = (string[])lhs.Data.GetData(DataFormats.FileDrop);
            return data ?? new string[0];
        }
    }

}
