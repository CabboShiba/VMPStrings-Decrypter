using System;
using System.Diagnostics;
using VMPStringDecrypter.Decrypter;

namespace VMPStringDecrypter
{
    internal class Utils
    {
        public static void Leave()
        {
            Console.WriteLine();
            Program.Log($"Succesfully decrypted {Decrypt.TotalStringsDecrypted} VMP Strings.", "INFO", ConsoleColor.Cyan);
            Program.Log("Press enter to leave...", "INFO", ConsoleColor.Yellow);
            Console.ReadLine();
            Process.GetCurrentProcess().Kill();
        }
    }
}
