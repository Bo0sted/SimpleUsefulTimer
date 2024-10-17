using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.DataContracts;
using System.Windows;
using System.Windows.Input;

namespace SimpleUsefulTimer
{
    public class HotkeyResponderLoop
    {
        readonly TimerControl tc;

        public HotkeyResponderLoop(ref TimerControl tc)
        {
            this.tc = tc;
            
            SetHook();

        }
        ~HotkeyResponderLoop()
        {
            Unhook();
        }

        public enum WH : int
        {
            WH_KEYBOARD_LL = 13
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(WH idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        public void SetHook()
        {
            _proc = HookCallback;
            _hookID = SetWindowsHookEx(WH.WH_KEYBOARD_LL, _proc, IntPtr.Zero, 0);
        }

        public void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (tc.IsVisible) return CallNextHookEx(_hookID, nCode, wParam, lParam); 

            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT kbStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

                //0x0101 (WM_KEYUP): Triggered when a key is released.
               if ((wParam == (IntPtr)0x0101) &&
                    (kbStruct.vkCode == (int)tc._toggleTimerHotkey))
                {
                    TimerControl.ToggleTimer();
                }
               else if ((wParam == (IntPtr)0x0101) &&
                    (kbStruct.vkCode == (int)tc._resetTimerHotkey))
                {
                    TimerControl.ResetTimer();
                }
            }

            // Pass the key to the next hook
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

    }
}
