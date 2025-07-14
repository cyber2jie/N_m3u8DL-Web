using Hardware;
using Hardware.Info;
using NLog;
using System.Runtime.InteropServices;

namespace N_m3u8DL_Web.Util
{

    public class OsInfo
    {
        public OS Os { get; set; }

        public List<Memory> Memory{ get; set; }

        public List<CPU> Cpu { get; set; }


        public List<Drive> Driver { get; set; }


        public string Framework { get; set; }


    }
    public class OsInfoUtil
    {
        public static OsInfo Get()
        {

            Logger logger=NLog.LogManager.GetCurrentClassLogger();

            IHardwareInfo hardwareInfo = null;
            try
            {
                hardwareInfo= new HardwareInfo();

                hardwareInfo.RefreshOperatingSystem();
                hardwareInfo.RefreshMemoryList();
                hardwareInfo.RefreshCPUList();
                hardwareInfo.RefreshDriveList();


            }catch(Exception ex)
            {
                logger.Warn($"hardware info refresh error,{ex.Message}");
            }


            return new OsInfo
            {
                Os = hardwareInfo.OperatingSystem,
                Memory = hardwareInfo.MemoryList,
                Cpu = hardwareInfo.CpuList,
                Driver = hardwareInfo.DriveList,
                Framework = RuntimeInformation.FrameworkDescription
            };
        }

    }
}
