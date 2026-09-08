using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PMD
{
    public static class Win32SetupApiComPortFinder
    {
        private const uint DIGCF_PRESENT = 0x00000002;
        private const uint DIGCF_ALLCLASSES = 0x00000004;

        private const uint SPDRP_HARDWAREID = 0x00000001;

        private const uint DICS_FLAG_GLOBAL = 0x00000001;
        private const uint DIREG_DEV = 0x00000001;
        private const uint KEY_READ = 0x20019;

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr SetupDiGetClassDevs(
            IntPtr classGuid,
            string enumerator,
            IntPtr hwndParent,
            uint flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiEnumDeviceInfo(
            IntPtr deviceInfoSet,
            uint memberIndex,
            ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetDeviceRegistryProperty(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            uint property,
            out uint propertyRegDataType,
            byte[] propertyBuffer,
            uint propertyBufferSize,
            out uint requiredSize);

        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern IntPtr SetupDiOpenDevRegKey(
            IntPtr deviceInfoSet,
            ref SP_DEVINFO_DATA deviceInfoData,
            uint scope,
            uint hwProfile,
            uint keyType,
            uint samDesired);

        [DllImport("setupapi.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int RegQueryValueEx(
            IntPtr hKey,
            string lpValueName,
            int lpReserved,
            out uint lpType,
            byte[] lpData,
            ref uint lpcbData);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(IntPtr hKey);

        public static List<string> FindCandidatePorts(ushort vid, ushort pid)
        {
            string vidPid = string.Format("VID_{0:X4}&PID_{1:X4}", vid, pid);
            var result = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            IntPtr deviceInfoSet = SetupDiGetClassDevs(IntPtr.Zero, "USB", IntPtr.Zero, DIGCF_PRESENT | DIGCF_ALLCLASSES);
            if (deviceInfoSet == INVALID_HANDLE_VALUE)
            {
                return result;
            }

            try
            {
                uint index = 0;
                while (true)
                {
                    SP_DEVINFO_DATA deviceInfoData = new SP_DEVINFO_DATA();
                    deviceInfoData.cbSize = (uint)Marshal.SizeOf(typeof(SP_DEVINFO_DATA));

                    if (!SetupDiEnumDeviceInfo(deviceInfoSet, index++, ref deviceInfoData))
                    {
                        break;
                    }

                    if (!DeviceMatchesVidPid(deviceInfoSet, ref deviceInfoData, vidPid))
                    {
                        continue;
                    }

                    string portName = ReadPortName(deviceInfoSet, ref deviceInfoData);
                    if (string.IsNullOrWhiteSpace(portName))
                    {
                        continue;
                    }

                    if (!portName.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (seen.Add(portName))
                    {
                        result.Add(portName);
                    }
                }
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return result;
        }

        private static bool DeviceMatchesVidPid(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData, string vidPid)
        {
            uint requiredSize;
            uint propertyType;

            bool ok = SetupDiGetDeviceRegistryProperty(
                deviceInfoSet,
                ref deviceInfoData,
                SPDRP_HARDWAREID,
                out propertyType,
                null,
                0,
                out requiredSize);

            if (!ok && Marshal.GetLastWin32Error() != ERROR_INSUFFICIENT_BUFFER)
            {
                return false;
            }

            if (requiredSize == 0)
            {
                return false;
            }

            byte[] buffer = new byte[requiredSize];
            ok = SetupDiGetDeviceRegistryProperty(
                deviceInfoSet,
                ref deviceInfoData,
                SPDRP_HARDWAREID,
                out propertyType,
                buffer,
                (uint)buffer.Length,
                out requiredSize);

            if (!ok)
            {
                return false;
            }

            string ids = Encoding.Unicode.GetString(buffer);
            string[] split = ids.Split(new[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < split.Length; i++)
            {
                if (split[i].IndexOf(vidPid, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static string ReadPortName(IntPtr deviceInfoSet, ref SP_DEVINFO_DATA deviceInfoData)
        {
            IntPtr hKey = SetupDiOpenDevRegKey(deviceInfoSet, ref deviceInfoData, DICS_FLAG_GLOBAL, 0, DIREG_DEV, KEY_READ);
            if (hKey == IntPtr.Zero || hKey == INVALID_HANDLE_VALUE)
            {
                return null;
            }

            try
            {
                uint type;
                uint dataSize = 0;
                int rc = RegQueryValueEx(hKey, "PortName", 0, out type, null, ref dataSize);
                if (rc != 0 && rc != ERROR_INSUFFICIENT_BUFFER)
                {
                    return null;
                }

                if (dataSize == 0)
                {
                    return null;
                }

                byte[] data = new byte[dataSize];
                rc = RegQueryValueEx(hKey, "PortName", 0, out type, data, ref dataSize);
                if (rc != 0)
                {
                    return null;
                }

                return Encoding.Unicode.GetString(data).TrimEnd('\0');
            }
            finally
            {
                RegCloseKey(hKey);
            }
        }
    }
}
