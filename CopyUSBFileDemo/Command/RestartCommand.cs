using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CopyUSBFileDemo.Command
{
    /// <summary>
    /// 重启服务指令
    /// </summary>
    public class RestartCommand : ICommand
    {
        /// <summary>
        /// 重新启动服务
        /// </summary>
        public void DealCommand()
        {
            ServiceDealer.RestartService();
        }
    }
}
