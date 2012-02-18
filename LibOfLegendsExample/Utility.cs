using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace LibOfLegends
{
    class Utility
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }
}

namespace System.Collections.Generic
{
    /// <summary>
    /// Represents a collection of useful extenions methods.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Translate a dictionary into a string for display.
        /// </summary>
        public static string PrettyPrint<K, V>(this IDictionary<K, V> dict)
        {
            if (dict == null)
                return "";
            string dictStr = "[";
            ICollection<K> keys = dict.Keys;
            int i = 0;
            foreach (K key in keys)
            {
                dictStr += key.ToString() + "=" + dict[key].ToString();
                if (i++ < keys.Count - 1)
                {
                    dictStr += ", ";
                }
            }
            return dictStr + "]";
        }
    }
}