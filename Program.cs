using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libdebug;

namespace PS4OfflineAccountActivator
{
    class Program
    {
        public static PS4DBG ps4 = null;
        public static ulong executable = 0;
        public static ulong sceRegMgrGetInt_addr = 0;
        public static ulong sceRegMgrGetStr_addr = 0;
        public static ulong sceRegMgrGetBin_addr = 0;

        public static ulong sceRegMgrSetInt_addr = 0;
        public static ulong sceRegMgrSetStr_addr = 0;
        public static ulong sceRegMgrSetBin_addr = 0;

        public static Process p = null;
        public static ulong stub = 0;
        public static ulong GetIntNative(uint regId, out int intVal)
        {
            ulong errorCode = 0;

            var bufferAddr = ps4.AllocateMemory(p.pid, sizeof(int));
            ps4.WriteMemory<int>(p.pid, bufferAddr, 0);

            errorCode = ps4.Call(p.pid, stub, sceRegMgrGetInt_addr, regId, bufferAddr);
            int valueReturned = ps4.ReadMemory<int>(p.pid, bufferAddr);

            ps4.FreeMemory(p.pid, bufferAddr, sizeof(int));

            intVal = valueReturned;

            return errorCode;
        }

        public static ulong SetIntNative(uint regId, int intVal)
        {
            return ps4.Call(p.pid, stub, sceRegMgrSetInt_addr, regId, intVal);
        }

        public static ulong GetStrNative(uint regId, out string strVal, uint strSize)
        {
            ulong errorCode = 0;

            var blankArray = new byte[strSize];

            var bufferAddr = ps4.AllocateMemory(p.pid, (int)strSize);
            ps4.WriteMemory(p.pid, bufferAddr, blankArray);

            errorCode = ps4.Call(p.pid, stub, sceRegMgrGetStr_addr, regId, bufferAddr, strSize);
            var valueReturned = ps4.ReadMemory(p.pid, bufferAddr, (int)strSize);

            ps4.FreeMemory(p.pid, bufferAddr, (int)strSize);

            int len = Array.IndexOf(valueReturned, (byte)0);
            strVal = Encoding.UTF8.GetString(valueReturned, 0, len);


            return errorCode;
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        public static ulong SetStrNative(uint regId, string strVal, uint strSize)
        {
            ulong errorCode = 0;

            byte[] temp_bytes = Encoding.ASCII.GetBytes(strVal);
            byte[] bytes = Combine(temp_bytes, new byte[] { 0x0 });

            var bufferAddr = ps4.AllocateMemory(p.pid, bytes.Length);
            ps4.WriteMemory(p.pid, bufferAddr, bytes);

            errorCode = ps4.Call(p.pid, stub, sceRegMgrSetStr_addr, regId, bufferAddr, bytes.Length);

            ps4.FreeMemory(p.pid, bufferAddr, bytes.Length);
            return errorCode;
        }

        public static ulong GetBinNative(uint regId, out byte[] binVal, uint binSize)
        {
            ulong errorCode = 0;

            var blankArray = new byte[binSize];

            var bufferAddr = ps4.AllocateMemory(p.pid, (int)binSize);
            ps4.WriteMemory(p.pid, bufferAddr, blankArray);
            errorCode = ps4.Call(p.pid, stub, sceRegMgrGetBin_addr, regId, bufferAddr, binSize);
            binVal = ps4.ReadMemory(p.pid, bufferAddr, (int)binSize);
            ps4.FreeMemory(p.pid, bufferAddr, (int)binSize);

            return errorCode;
        }

        public static ulong SetBinNative(uint regId, byte[] binVal, uint binSize)
        {
            ulong errorCode = 0;
            var bufferAddr = ps4.AllocateMemory(p.pid, (int)binSize);
            ps4.WriteMemory(p.pid, bufferAddr, binVal);
            errorCode = ps4.Call(p.pid, stub, sceRegMgrSetBin_addr, regId, bufferAddr, binSize);
            ps4.FreeMemory(p.pid, bufferAddr, (int)binSize);
            return errorCode;
        }


        static void Main(string[] args)
        {
            Registry r = new Registry();

            // Put your PS4 IP address here
            ps4 = new PS4DBG("192.168.1.85");
            ps4.Connect();

            ProcessList pl = ps4.GetProcessList();

            p = pl.FindProcess("SceShellUI");

            ProcessMap pi = ps4.GetProcessMaps(p.pid);
            executable = 0;
            for (int i = 0; i < pi.entries.Length; i++)
            {
                MemoryEntry me = pi.entries[i];
                if (me.prot == 5)
                {
                    Console.WriteLine("executable base " + me.start.ToString("X"));
                    executable = me.start;
                    break;
                }
            }

            stub = ps4.InstallRPC(p.pid);

            sceRegMgrGetInt_addr = executable + 0x3ADF80;
            sceRegMgrGetStr_addr = executable + 0x81BC20;
            sceRegMgrGetBin_addr = executable + 0x81D6A0;

            sceRegMgrSetInt_addr = executable + 0x81DFB0;
            sceRegMgrSetStr_addr = executable + 0x821A10;
            sceRegMgrSetBin_addr = executable + 0x81D6B0;

            int outValue = 0;

            // A number from 1 to 16 
            int userNumber = 1;
            ulong errorCode = 0;

            string outString = null;
            byte[] psnAccountId = null;

            // Put your PSN account id here. Two different methods for obtaining your PSN account id:
            // 
            // 1. It's string you see when exporting (from an activated PS4) save data in the usb folder but byte reversed. Example: PS4\savedata\0102030405060708 (reversing it you get 0807060504030201)
            // 2. On a computer delete your browser cache. Press Ctrl+Shift+I to open the developer tools.
            //    Browse the PSN store on your computer and log in to your account. 
            //    Some of the JSON files the browser downloads contain an "accountId" field. It's a decimal number. Just convert it to hex and reverse the bytes.

            psnAccountId = new byte[] { 0x08, 0x07, 0x06, 0x05, 0x04, 0x03, 0x02, 0x01 };
            errorCode = SetBinNative((uint)r.KEY_account_id(userNumber), psnAccountId, Registry.SIZE_account_id);
            //errorCode = GetBinNative((uint)r.KEY_account_id(userNumber), out psnAccountId, Registry.SIZE_account_id);

            string text = "np";
            errorCode = SetStrNative((uint)r.KEY_NP_env(userNumber), text, (uint)text.Length);
            //errorCode = GetStrNative((uint)r.KEY_NP_env(userNumber), out outString, Registry.SIZE_NP_env); Console.WriteLine("SCE_REGMGR_ENT_KEY_USER_01_16_NP_env              {0} - {1}", userNumber, outString);

            errorCode = SetIntNative((uint)r.KEY_login_flag(userNumber), 6);
            //errorCode = GetIntNative((uint)r.KEY_login_flag(userNumber), out outValue); Console.WriteLine("SCE_REGMGR_ENT_KEY_USER_01_16_login_flag                      {0} - {1}", userNumber, outValue);


            ps4.Disconnect();

            Console.ReadKey();
        }
    }
}
