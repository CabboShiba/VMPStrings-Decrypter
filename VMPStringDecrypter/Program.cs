using dnlib.DotNet.Writer;
using dnlib.DotNet;
using System;
using System.Reflection;
using VMPStringDecrypter.Decrypter;

namespace VMPStringDecrypter
{
    internal class Program
    {
        public static Assembly asm;
        static void Main(string[] args)
        {
            try
            {
                Console.Title = $"[{DateTime.Now}] VMProtect String Decrypter by https://github.com/CabboShiba";
                //string path = args[0];
                string path = args[0];
                asm = Assembly.LoadFile(path);
                ModuleContext modCtx = ModuleDef.CreateModuleContext();
                ModuleDefMD module = ModuleDefMD.Load(path, modCtx);
                try
                {
                    Decrypt.DecryptStrings(module);
                }
                catch (Exception ex)
                {
                    Log("Error: " + ex.Message, "ERROR", ConsoleColor.Red);
                    Utils.Leave();
                }
                ModuleWriterOptions options = new ModuleWriterOptions(module);
                options.Logger = DummyLogger.NoThrowInstance;
                module.Write(path.Replace(".exe", "-decrypted.exe"), options);
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message, "ERROR", ConsoleColor.Red);
            }
            Utils.Leave();
        }


        public static void Log(string Data, string Type, ConsoleColor Color)
        {
            Console.ForegroundColor = Color;
            Console.WriteLine($"[{DateTime.Now.ToString("HH:mm:ss")} - {Type}] {Data}");
            Console.ResetColor();
        }
    }
}
