using System;
using System.Security.Cryptography;
using System.Text;

namespace nCentralDumpsterDiver
{
    public class StringGenerator
    {
        internal static readonly char[] chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        internal static readonly char[] lower = "abcdefghijklmnopqrstuvwxyz1234567890".ToCharArray();
        public static string GetUniqueIP()
        {
            byte[] data = new byte[17];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 4; i++)
            {
                var rnd = BitConverter.ToUInt32(data, ((i * 4)));
                var idx = rnd % Byte.MaxValue;
                result.Append($"{Convert.ToString(idx)}.");
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
        public static string GetUniqueMAC()
        {
            byte[] data = new byte[25];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                var rnd = BitConverter.ToUInt32(data, ((i * 4)));
                var idx = rnd % Byte.MaxValue;
                result.Append($"{idx.ToString("X2")}:");
            }
            result.Remove(result.Length - 1, 1);
            return result.ToString();
        }
        public static string GetUniqueString(int size,bool LowercaseOnly = false)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                if (LowercaseOnly)
                {
                    var idx = rnd % lower.Length;
                    result.Append(lower[idx]);
                }
                else
                {
                    var idx = rnd % chars.Length;
                    result.Append(chars[idx]);
                }
            }

            return result.ToString();
        }

        public static string GetUniqueStringBiased(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}