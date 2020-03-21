using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FFT.Core.UI
{
    public class FancyListView : ListView
    {
        [DllImport("uxtheme", CharSet = CharSet.Unicode)]
        public static extern int SetWindowTheme(IntPtr hWnd, string textSubAppName, string textSubIdList);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr window, int message, int wParam, IntPtr lParam);

        public FancyListView()
        {
            this.DoubleBuffered = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            try
            {
                SetWindowTheme(Handle, "explorer", null);
            }
            catch { }
            base.OnHandleCreated(e);
        }
        public void SetGroupCollapse(GroupState state)
        {

            for (int i = 0; i <= this.Groups.Count; i++)
            {

                LVGROUP group = new LVGROUP();
                group.cbSize = Marshal.SizeOf(group);
                group.state = (int)state; // LVGS_COLLAPSIBLE 
                group.mask = 4; // LVGF_STATE 
                group.iGroupId = i;

                IntPtr ip = IntPtr.Zero;
                try
                {
                    ip = Marshal.AllocHGlobal(group.cbSize);
                    Marshal.StructureToPtr(group, ip, true);
                    SendMessage(this.Handle, 0x1000 + 147, i, ip); // #define LVM_SETGROUPINFO (LVM_FIRST + 147) 
                                                                   //SendMessage(this.Handle, 0x1000 + 147, i, ip);
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                }
                finally
                {
                    if (null != ip) Marshal.FreeHGlobal(ip);
                }
            }

        }
    }
    [StructLayout(LayoutKind.Sequential)]

    public struct LVGROUP
    {

        public int cbSize;

        public int mask;

        [MarshalAs(UnmanagedType.LPTStr)]

        public string pszHeader;

        public int cchHeader;

        [MarshalAs(UnmanagedType.LPTStr)]

        public string pszFooter;

        public int cchFooter;

        public int iGroupId;

        public int stateMask;

        public int state;

        public int uAlign;

    }

    public enum GroupState
    {

        COLLAPSIBLE = 8,

        COLLAPSED = 1,

        EXPANDED = 0

    }
}
