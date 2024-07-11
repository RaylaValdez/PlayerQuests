using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerQuests.Windows;

namespace PlayerQuests.Manager
{
    public class WindowManager : IDisposable
    {


        public void Dispose()
        {
            Plugin.WindowSystem.RemoveAllWindows();
        }
    }
}
