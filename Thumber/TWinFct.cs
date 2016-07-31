using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Thumber
{
    static class TWinFct
    {
        [DllImport("user32")]
        private static extern int LockWindowUpdate(IntPtr hwndLock);
        public static int LockControlUpdate(Control AControl)
        {
            return LockWindowUpdate(AControl.Handle);
        }

        public static int UnLockControlUpdate()
        {
            return LockWindowUpdate(IntPtr.Zero);
        }


        public static void SetProgressNoAnimation(this ProgressBar pb, int value)
        {
            // To get around the progressive animation, we need to move the 
            // progress bar backwards.
            if (value == pb.Maximum)
            {
                // Special case as value can't be set greater than Maximum.
                pb.Maximum = value + 1;     // Temporarily Increase Maximum
                pb.Value = value + 1;       // Move past
                pb.Maximum = value;         // Reset maximum
            }
            else
            {
                pb.Value = value + 1;       // Move past
            }
            pb.Value = value;               // Move to correct value
        }

        public static void PerformStepNoAnimation(this ProgressBar pb)
        {
            pb.SetProgressNoAnimation(pb.Value + pb.Step);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        public enum ProgressBarState
        {
            Normal = 0x0001, // PBST_NORMAL
            Error  = 0x0002, // PBST_ERROR
            Paused = 0x0003  // PBST_PAUSED
        }

        public static void SetState(this ProgressBar self, ProgressBarState state)
        {
            SendMessage(self.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                (uint)state, 
                0);
        }
    }
}
