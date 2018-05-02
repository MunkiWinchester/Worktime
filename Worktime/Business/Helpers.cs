using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Worktime.Business
{
    internal static class Helpers
    {
        /// <summary>
        ///     Returns a Secure string from the source string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return null;

            var result = new SecureString();
            foreach (var c in source)
                result.AppendChar(c);
            return result;
        }

        /// <summary>
        ///     Returns a string from the source secure string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUnsecureString(this SecureString source)
        {
            var valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = Marshal.SecureStringToGlobalAllocUnicode(source);
                return Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }

        /// <summary>
        ///     Returns the md5 hash of a string
        /// </summary>
        /// <param name="pw"></param>
        /// <returns></returns>
        public static string GetHash(string pw)
        {
            return string.IsNullOrWhiteSpace(pw) ? string.Empty : GetMd5(pw);
        }

        /// <summary>
        ///     Returns the md5 hash of a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string GetMd5(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var textToHash = Encoding.UTF8.GetBytes(input);
            var result = md5.ComputeHash(textToHash);

            var s = new StringBuilder();
            foreach (var b in result)
                s.Append(b.ToString("x2").ToLower());

            return s.ToString();
        }

        /// <summary>
        ///     Returns the calculated percentage of the actual and the regular time
        /// </summary>
        /// <param name="actualTime"></param>
        /// <param name="regularTime"></param>
        /// <returns></returns>
        public static double CalculatePercentage(TimeSpan actualTime, TimeSpan regularTime)
        {
            if (actualTime.TotalMinutes.Equals(0d) || regularTime.TotalMinutes.Equals(0d))
                return 0;

            var result = actualTime.TotalMinutes / regularTime.TotalMinutes;
            result = result * 100;
            return result;
        }
    }
}