using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace WindowsFormsApplication1
{
    internal class HWID
    {
        public static string getUniqueID(string drive)
        {
            if (drive == string.Empty)
                foreach (var compDrive in DriveInfo.GetDrives())
                    if (compDrive.IsReady)
                    {
                        drive = compDrive.RootDirectory.ToString();
                        break;
                    }

            if (drive.EndsWith(":\\"))
                drive = drive.Substring(0, drive.Length - 2);

            var volumeSerial = getVolumeSerial(drive);
            var cpuID = getCPUID();

            //Mix them up and remove some useless 0's
            return cpuID.Substring(13) + cpuID.Substring(1, 4) + volumeSerial + cpuID.Substring(4, 4);
        }

        public static string getVolumeSerial(string drive)
        {
            var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            disk.Get();

            var volumeSerial = disk["VolumeSerialNumber"].ToString();
            disk.Dispose();

            return volumeSerial;
        }

        public static string getCPUID()
        {
            var cpuInfo = "";
            var managClass = new ManagementClass("win32_processor");
            var managCollec = managClass.GetInstances();

            foreach (ManagementObject managObj in managCollec)
                if (cpuInfo == "")
                {
                    cpuInfo = managObj.Properties["processorID"].Value.ToString();
                    break;
                }

            return cpuInfo;
        }

        public static string GetMachineGuid()
        {
            var location = @"SOFTWARE\Microsoft\Cryptography";
            var name = "MachineGuid";

            using (var localMachineX64View = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
            )
            {
                using (var rk = localMachineX64View.OpenSubKey(location))
                {
                    if (rk == null)
                        throw new KeyNotFoundException(
                            string.Format("Key Not Found: {0}", location));

                    var machineGuid = rk.GetValue(name);
                    if (machineGuid == null)
                        throw new IndexOutOfRangeException(
                            string.Format("Index Not Found: {0}", name));

                    return machineGuid.ToString();
                }
            }
        }
    }
}