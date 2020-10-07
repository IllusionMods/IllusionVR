using System.Diagnostics;
using System.Linq;

namespace VRGIN.Helpers
{
    public static class SteamVRDetector
    {
        private static bool FilterInvalidProcesses(Process p)
        {
            try
            {
                return p.ProcessName != null;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsRunning => Process.GetProcesses().Where(FilterInvalidProcesses).Any(process => process.ProcessName == "vrcompositor");
    }
}
