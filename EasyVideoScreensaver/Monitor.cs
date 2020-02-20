using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace EasyVideoScreensaver
{
    public class Monitor
    {
        //#region Dll imports

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //[ResourceExposure(ResourceScope.None)]
        //private static extern bool GetMonitorInfo
        //              (HandleRef hmonitor, [In, Out]MonitorInfoEx info);

        //[DllImport("user32.dll", ExactSpelling = true)]
        //[ResourceExposure(ResourceScope.None)]
        //private static extern bool EnumDisplayMonitors
        //     (HandleRef hdc, IntPtr rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        //[DllImport("Shcore.dll")]
        //[ResourceExposure(ResourceScope.None)]
        //private static extern IntPtr GetDpiForMonitor([In]IntPtr hmonitor, [In]DpiType dpiType, [Out]out uint dpiX, [Out]out uint dpiY);

        //private delegate bool MonitorEnumProc
        //             (IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

        //[StructLayout(LayoutKind.Sequential)]
        //private struct Rect
        //{
        //    public int left;
        //    public int top;
        //    public int right;
        //    public int bottom;
        //}

        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        //private class MonitorInfoEx
        //{
        //    internal int cbSize = Marshal.SizeOf(typeof(MonitorInfoEx));
        //    internal Rect rcMonitor = new Rect();
        //    internal Rect rcWork = new Rect();
        //    internal int dwFlags = 0;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        //    internal char[] szDevice = new char[32];
        //}

        //private const int MonitorinfofPrimary = 0x00000001;

        //public enum DpiType
        //{
        //    Effective = 0,
        //    Angular = 1,
        //    Raw = 2,
        //}

        //#endregion

        public static HandleRef NullHandleRef = new HandleRef(null, IntPtr.Zero);

        public System.Windows.Rect Bounds { get; private set; }
        public System.Windows.Rect WorkingArea { get; private set; }
        public string Name { get; private set; }

        public bool IsPrimary { get; private set; }

        public uint DpiX { get { return dpiX; } }
        public uint DpiY { get { return dpiY; } }

        private uint dpiX, dpiY;

        private Monitor(IntPtr monitor, IntPtr hdc)
        {
            var info = new NativeMethods.MonitorInfoEx();
            NativeMethods.GetMonitorInfo(new HandleRef(null, monitor), info);
            Bounds = new System.Windows.Rect(
                        info.rcMonitor.Left, info.rcMonitor.Top,
                        info.rcMonitor.Right - info.rcMonitor.Left,
                        info.rcMonitor.Bottom - info.rcMonitor.Top);
            WorkingArea = new System.Windows.Rect(
                        info.rcWork.Left, info.rcWork.Top,
                        info.rcWork.Right - info.rcWork.Left,
                        info.rcWork.Bottom - info.rcWork.Top);
            IsPrimary = ((info.dwFlags & NativeMethods.MonitorinfofPrimary) != 0);
            Name = new string(info.szDevice).TrimEnd((char)0);

            NativeMethods.GetDpiForMonitor(monitor, NativeMethods.DpiType.Effective, out dpiX, out dpiY);

        }

        public static IEnumerable<Monitor> AllMonitors
        {
            get
            {
                var closure = new MonitorEnumCallback();
                var proc = new NativeMethods.MonitorEnumProc(closure.Callback);
                NativeMethods.EnumDisplayMonitors(NullHandleRef, IntPtr.Zero, proc, IntPtr.Zero);
                return closure.Monitors.Cast<Monitor>();
            }
        }

        private class MonitorEnumCallback
        {
            public ArrayList Monitors { get; private set; }

            public MonitorEnumCallback()
            {
                Monitors = new ArrayList();
            }

            public bool Callback(IntPtr monitor, IntPtr hdc,
                           IntPtr lprcMonitor, IntPtr lparam)
            {
                Monitors.Add(new Monitor(monitor, hdc));
                return true;
            }
        }
    }
}
