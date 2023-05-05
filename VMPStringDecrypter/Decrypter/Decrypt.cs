using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using static dnlib.DotNet.Emit.OpCodes;
using static VMPStringDecrypter.Program;

namespace VMPStringDecrypter.Decrypter
{
    internal class Decrypt
    {
        public static int TotalStringsDecrypted = 0;
        public static void DecryptStrings(ModuleDefMD mod)
        {
            MethodDef StringMethod = null;
            string DecryptedString = null;
            UInt32 StringValue = 0;
            foreach(var type in mod.Types)
            {
                foreach(var method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        for(int i = 1; i < method.Body.Instructions.Count; i++)
                        {
                            if (method.Body.Instructions[i].OpCode == Call && method.Body.Instructions[i-1].OpCode == Ldc_I4)
                            {
                                if (method.Body.Instructions[i].Operand.ToString().Contains("System.UInt32") && method.Body.Instructions[i].Operand.ToString().Contains("System.String"))
                                {
                                    try
                                    {
                                        StringMethod = method.Body.Instructions[i].Operand as MethodDef;
                                        if (StringMethod.HasBody)
                                        {
                                            if (PatternMatch(StringMethod) == true)
                                            {
                                                Log($"Discovered String on Method: {method.Name} | Index: {i} | Invoking...", "INFO", ConsoleColor.Yellow);
                                                StringValue = Convert.ToUInt32(method.Body.Instructions[i - 1].GetLdcI4Value());
                                                DecryptedString = (string)asm.ManifestModule.ResolveMethod((int)StringMethod.MDToken.Raw).Invoke(null, new object[] { StringValue });
                                                Log("Decrypted String: " + DecryptedString, "SUCCESS", ConsoleColor.Green);
                                                TotalStringsDecrypted++;
                                                method.Body.Instructions[i].OpCode = Ldstr;
                                                method.Body.Instructions[i].Operand = DecryptedString;
                                                method.Body.Instructions[i - 1].OpCode = Nop;
                                            }
                                            else
                                            {
                                                Log("No match for: " + method.MDToken, "INFO", ConsoleColor.Red);
                                            }
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        Log("Error: " + ex.Message, "ERROR", ConsoleColor.Red);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static OpCode[] OpCodesPattern = { Newobj, Ldc_I4, Newarr, Dup, Ldc_I4_0, Ldarg_0, Box, Stelem_Ref, Ldc_I4, Call, Castclass, Ret };
        private static bool PatternMatch(MethodDef method)
        {
            bool IsMatch = true;
            if(method.Body.Instructions.Count == OpCodesPattern.Length)
            {
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    if (method.Body.Instructions[i].OpCode != OpCodesPattern[i])
                    {
                        IsMatch = false;
                        break;
                    }
                }
            }
            else
            {
                IsMatch = false;
            }
            return IsMatch;
        }
    }
}
