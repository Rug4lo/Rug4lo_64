using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Rug4lo_64
{
    public class rug4lo
    {
        private Process proc;

        //
        // Dll Imports
        //

        // ReadProcessMemory with the windows API, reference: https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-readprocessmemory
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int nSize, IntPtr lpNumberOfBytesRead);

        // WriteProcessMemory with the windows API, reference: https://learn.microsoft.com/en-us/windows/win32/api/memoryapi/nf-memoryapi-writeprocessmemory
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpbuffer, int nSize, IntPtr lpNumberOfBytesWritten);

        //
        // Start of main functions
        //

        // Simple functions for the process
        public rug4lo(string N_proc)
        {
            proc = SetProcess(N_proc);
        }

        public Process GetProcess()
        {
            return proc;
        }

        // Get the process name by using the firs coincidence
        public Process SetProcess(string N_proc)
        {
            proc = Process.GetProcessesByName(N_proc)[0];

            if (proc == null)
            {
                throw new InvalidOperationException("[!] The process was not found");
            }

            return proc;
        }

        // Get the base of the module
        public IntPtr GetModuleBase(string Module)
        {
            if (String.IsNullOrEmpty(Module))
            {
                throw new InvalidOperationException("[!] Module was not found");
            }

            if (proc == null)
            {
                throw new InvalidOperationException("[!] The process is not valid");
            }

            try
            {
                if (Module.Contains(".exe") && proc.MainModule != null) // If the module base is a .exe you can easily get it with "proc.MainModule"
                { 
                    return proc.MainModule.BaseAddress;
                }

                foreach (ProcessModule module in proc.Modules) // If the module base is not a .exe you need to iterate in all the modules of the process to get yours
                {
                    if (module.ModuleName == Module)
                    {
                        return module.BaseAddress;
                    }
                }
            }
            catch (Exception)
            {
                throw new InvalidOperationException("[!] Failed to find the module");
            }

            return IntPtr.Zero; 
        }

        //
        // Start of the ReadPointer funcions, all the cases for a 64 bit program
        //

        public IntPtr ReadPointer(IntPtr data) // Only data
        {
            byte[] array = new byte[8];
            bool success = ReadProcessMemory(proc.Handle, data, array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address: " + data);
            }

            return (IntPtr)BitConverter.ToInt64(array, 0);
        }

        public IntPtr ReadPointer(IntPtr data, int offset) // IntPtr Data and 32 bit Offset
        {
            byte[] array = new byte[8];
            bool success = ReadProcessMemory(proc.Handle, data + offset, array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address: " + data);
            }

            return (IntPtr)BitConverter.ToInt64(array, 0);
        }

        public IntPtr ReadPointer(long data, long offset) // 64 bit Data and 64 bit Offset
        {
            byte[] array = new byte[8];
            bool success = ReadProcessMemory(proc.Handle, (IntPtr)(data + offset), array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address: " + data);
            }

            return (IntPtr)BitConverter.ToInt64(array, 0);
        }

        public IntPtr ReadPointer(IntPtr data, long offset) // IntPtr Data and 64 bit Offset
        {
            byte[] array = new byte[8];
            bool success = ReadProcessMemory(proc.Handle, new IntPtr(data.ToInt64() + offset), array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address: " + data);
            }

            return (IntPtr)BitConverter.ToInt64(array, 0);
        }

        public IntPtr ReadPointer(IntPtr data, IntPtr offset) // IntPtr Data and IntPtr Offset
        {
            byte[] array = new byte[8];
            bool success = ReadProcessMemory(proc.Handle, new IntPtr(data.ToInt64() + offset.ToInt64()), array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address: " + data);
            }

            return (IntPtr)BitConverter.ToInt64(array, 0);
        }

        public IntPtr ReadPointer(IntPtr data, params int[] offsets) // IntPtr Data and array of 32 bit Offsets, you can add more than one offset without an array also because of params
        {
            IntPtr pointer = data;

            foreach (int offset in offsets)
            {
                pointer = ReadPointer(pointer, offset);
            }

            return pointer;
        }

        public IntPtr ReadPointer(IntPtr data, params long[] offsets) // IntPtr Data and array of 64 bit Offsets, you can add more than one offset without an array also because of params
        {
            IntPtr pointer = data;

            foreach (long offset in offsets)
            {
                pointer = ReadPointer(pointer, offset);
            }

            return pointer;
        }

        public IntPtr ReadPointer(IntPtr data, IntPtr offset1, int offset2) // IntPtr Data IntPtr Offset and 32 bit Offset
        {
            IntPtr addy2 = ReadPointer((IntPtr)((long)data + (long)offset1));
            return ReadPointer(addy2, offset2);
        }

        //
        // Start of the ReadBytes funcions, main function for the normal and unsigned read functions
        //

        // ReadBytes function, to use all types of data
        public byte[] ReadBytes(IntPtr data, int bytes) // Read the data in bytes
        {
            byte[] array = new byte[bytes];
            bool success = ReadProcessMemory(proc.Handle, data, array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address in bytes: " + data);
            }

            return array;
        }

        public byte[] ReadBytes(IntPtr data, int offset, int bytes) // Read the data in bytes adding an offset
        {
            byte[] array = new byte[bytes];
            bool success = ReadProcessMemory(proc.Handle, data + offset, array, array.Length, IntPtr.Zero);

            if (!success)
            {
                throw new InvalidOperationException("[!] Failed to read memory address in bytes: " + data);
            }

            return array;
        }

        //
        // Start of the normal read functions
        //

        public short ReadShort(IntPtr address) // Read the data with 16 bits
        {
            return BitConverter.ToInt16(ReadBytes(address, 2), 0);
        }
        public short ReadShort(IntPtr address, int offset) // Read the data with 16 bits and a offset
        {
            return BitConverter.ToInt16(ReadBytes(address + offset, 2), 0);
        }
        public int ReadInt(IntPtr address) // Read the data with 32 bits
        {
            return BitConverter.ToInt32(ReadBytes(address, 4), 0);
        }

        public int ReadInt(IntPtr address, int offset) // Read the data with 32 bits and a offset
        {
            return BitConverter.ToInt32(ReadBytes(address + offset, 4), 0);
        }

        public IntPtr Readlong(IntPtr address) // Read the data with 64 bits
        {
            return (IntPtr)BitConverter.ToInt64(ReadBytes(address, 8), 0);
        }

        public IntPtr Readlong(IntPtr address, int offset) // Read the data with 64 bits and a offset
        {
            return (IntPtr)BitConverter.ToInt64(ReadBytes(address + offset, 8), 0);
        }

        public float ReadFloat(IntPtr address) // Read the data from a float
        {
            return BitConverter.ToSingle(ReadBytes(address, 4), 0);
        }

        public float ReadFloat(IntPtr address, int offset) // Read the data from a float and a offset
        {
            return BitConverter.ToSingle(ReadBytes(address, 4), 0);
        }

        public double ReadDouble(IntPtr address) // Read the data from a double
        {
            return BitConverter.ToDouble(ReadBytes(address, 8), 0);
        }

        public double ReadDouble(IntPtr address, int offset) // Read the data from a double and a offset
        {
            return BitConverter.ToDouble(ReadBytes(address + offset, 4), 0);
        }

        public bool ReadBool(IntPtr address) // Read the data from a boolean
        {
            return BitConverter.ToBoolean(ReadBytes(address, 1), 0);
        }

        public bool ReadBool(IntPtr address, int offset) // Read the data from a boolean and a offset
        {
            return BitConverter.ToBoolean(ReadBytes(address + offset, 1), 0);
        }

        public string ReadString(IntPtr address, int length) // Read the data from a string
        {
            return Encoding.UTF8.GetString(ReadBytes(address, length));
        }

        public string ReadString(IntPtr address, int length, int offset) // Read the data from a string and a offset
        {
            return Encoding.UTF8.GetString(ReadBytes(address + offset, length));
        }

        public char ReadChar(IntPtr address, int length) // Read the data from a char
        {
            return BitConverter.ToChar(ReadBytes(address, length), 2);
        }

        public char ReadChar(IntPtr address, int length, int offset) // Read the data from a char and a offset
        {
            return BitConverter.ToChar(ReadBytes(address + offset, length), 2);
        }

        //
        // Start of the unsigned read functions
        //

        public ushort ReadUShort(IntPtr address) // Read the unsigned data with 16 bits
        {
            return BitConverter.ToUInt16(ReadBytes(address, 2), 0);
        }
        public ushort ReadUShort(IntPtr address, int offset) // Read the unsigned data with 16 bits and a offset
        {
            return BitConverter.ToUInt16(ReadBytes(address + offset, 2), 0);
        }
        public uint ReadUInt(IntPtr address) // Read the unsigned data with 32 bits
        {
            return BitConverter.ToUInt32(ReadBytes(address, 4), 0);
        }

        public uint ReadUInt(IntPtr address, int offset) // Read the unsigned data with 32 bits and a offset
        {
            return BitConverter.ToUInt32(ReadBytes(address + offset, 4), 0);
        }

        public IntPtr ReadUlong(IntPtr address) // Read the unsigned data with 64 bits
        {
            return (IntPtr)BitConverter.ToUInt64(ReadBytes(address, 8), 0);
        }

        public IntPtr ReadUlong(IntPtr address, int offset) // Read the unsigned data with 64 bits and a offset
        {
            return (IntPtr)BitConverter.ToUInt64(ReadBytes(address + offset, 8), 0);
        }

        //
        // Start of the WriteBytes funcions, main function for the normal and unsigned write functions
        //

        public bool WriteBytes(IntPtr address, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address, newbytes, newbytes.Length, IntPtr.Zero);
        }

        public bool WriteBytes(IntPtr address, int offset, byte[] newbytes)
        {
            return WriteProcessMemory(proc.Handle, address + offset, newbytes, newbytes.Length, IntPtr.Zero);
        }

        //
        // Start of the normal write functions (These functions return only a boolean value )
        //

        public bool WriteShort(IntPtr address, short data) // Write the data with 16 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteShort(IntPtr address, int offset, short data) // Write the data with 16 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool WriteInt(IntPtr address, int data) // Write the data with 32 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteInt(IntPtr address, int offset, int data) // Write the data with 32 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool Writelong(IntPtr address, long data) // Write the data with 64 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool Writelong(IntPtr address, int offset, long data) // Write the data with 64 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool WriteFloat(IntPtr address, float data) // Write the data from a float
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteFloat(IntPtr address, int offset, float data) // Write the data from a float and a offset
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteDouble(IntPtr address, double data) // Write the data from a double
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteDouble(IntPtr address, int offset, double data) // Write the data from a double and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool WriteBool(IntPtr address, bool data) // Write the data from a boolean
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteBool(IntPtr address, int offset, bool data) // Write the data from a boolean and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool WriteChar(IntPtr address, char data) // Write the data from a char
        {
            return WriteBytes(address, Encoding.UTF8.GetBytes(data.ToString()));
        }

        public bool WriteChar(IntPtr address, int offset, char data) // Write the data from a char and a offset
        {
            return WriteBytes(address + offset, Encoding.UTF8.GetBytes(data.ToString()));
        }

        public bool WriteString(IntPtr address, string data) // Write the data from a string
        {
            return WriteBytes(address, Encoding.UTF8.GetBytes(data));
        }

        public bool WriteString(IntPtr address, int offset, string data) // Write the data from a string and a offset
        {
            return WriteBytes(address + offset, Encoding.UTF8.GetBytes(data));
        }

        public bool WriteHexString(IntPtr address, string Hexbytes) // Write the data from a string with hexadecimal data
        {
            try
            {
                byte[] array = (from b in Hexbytes.Split(' ')
                                select Convert.ToByte(b, 16)).ToArray();

                return WriteBytes(address, array);
            }
            catch
            {
                throw new Exception("[!] Write error in WriteHexString function: " + Hexbytes);
            }
        }

        public bool WriteHexString(IntPtr address, int offset, string Hexbytes) // Write the data from a string with hexadecimal data and adding an offset
        {
            try
            {
                byte[] array = (from b in Hexbytes.Split(' ')
                                select Convert.ToByte(b, 16)).ToArray();

                return WriteBytes(address + offset, array);
            }
            catch
            {
                throw new Exception("[!] Write error in WriteHexString + offset function: " + Hexbytes);
            }
        }

        //
        // Start of the unsigned write functions
        //

        public bool WriteUShort(IntPtr address, ushort data) // Write the unsigned data with 16 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }
        public bool WriteUShort(IntPtr address, int offset, ushort data) // Write the unsigned data with 16 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }
        public bool WriteUInt(IntPtr address, uint data) // Write the unsigned data with 32 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteUInt(IntPtr address, int offset, uint data) // Write the unsigned data with 32 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }

        public bool WriteUlong(IntPtr address, ulong data) // Write the unsigned data with 64 bits
        {
            return WriteBytes(address, BitConverter.GetBytes(data));
        }

        public bool WriteUlong(IntPtr address, int offset, ulong data) // Write the unsigned data with 64 bits and a offset
        {
            return WriteBytes(address + offset, BitConverter.GetBytes(data));
        }
    }
}
