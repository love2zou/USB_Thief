using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using CopyUSBFileDemo.Command;

namespace CopyUSBFileDemo
{
    public class Monitoring
    {
        public static Thread thread = null;
        public static Thread threadCommand = null;
        public static Thread threadVilidate = null;

        public static bool IsStoped = false;
        public static bool IsRestart = false;
        
        public static void MonitoringServiceStatus()
        {
            thread = new Thread(MiniServiceStatus);
            thread.Start();
        }


        /// <summary>
        /// 监听服务接收的指令信息
        /// </summary>
        public static void MonitoringServiceCommandMsg()
        {
            threadCommand = new Thread(MiniCommand);
            threadCommand.Start();
        }

        public static void ReStartService()
        {
            ICommand command = new RestartCommand();
            command.DealCommand();//发送重启服务指令
        }

        /// <summary>
        /// 监听服务接收的指令信息
        /// </summary>
        public static void FileMonitoringCommand(FileSystemWatcher curWatcher)
        {
            //FileStream fs = new FileStream(@"E:\log.txt", FileMode.OpenOrCreate, FileAccess.Write);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.BaseStream.Seek(0, SeekOrigin.End);
            //sw.WriteLine("WindowsService: Service Started" + DateTime.Now.ToString() + AppDomain.CurrentDomain.BaseDirectory + "\n");
            //sw.Flush();
            //sw.Close();
            //fs.Close();
            try
            {
                curWatcher = new FileSystemWatcher();
                curWatcher.BeginInit();
                curWatcher.Filter = "App.config";//只监视config文件的修改动作(例：fsw.Filter = "*.txt|*.doc|*.jpg")
                curWatcher.IncludeSubdirectories = false;
                curWatcher.Path = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug", "");
                LogerHelper.WriteOperateLog("监控文件路径：" + curWatcher.Path);
                curWatcher.Changed += new FileSystemEventHandler(OnFileChanged);
                curWatcher.EnableRaisingEvents = true;//开启监控
                curWatcher.EndInit();
            }
            catch (Exception ex)
            {
                LogerHelper.WriteErrorLog(ex);
            }

        }

        private static void OnFileChanged(Object source, FileSystemEventArgs e)
        {
            //采用临时禁用法解决触发两次事件的问题，只适合解决监控单个文件的处理
            if (source != null) 
            { 
                var watcher = source as FileSystemWatcher;
                watcher.EnableRaisingEvents = false; 
            
                Thread th = new Thread(new ThreadStart(             
                    delegate()            
                    {        
                         Thread.Sleep(1000); 
                         watcher.EnableRaisingEvents = true; 
                     } 
                 ));             
                 th.Start();
                 IsRestart = true;  
             } 
            LogerHelper.WriteOperateLog(string.Format("修改类型：{0}，配置文件{0}被修改", e.ChangeType, e.FullPath));
            LogerHelper.WriteOperateLog("是否重启服务：" + IsRestart);
            if (IsRestart)
            {
                //重启服务
                ICommand command = new RestartCommand();
                command.DealCommand();
                IsRestart = false;
            }
        }

        /// <summary>
        /// 监听服务状态
        /// </summary>
        private static void MiniServiceStatus()
        {
            var time = 5;
            int.TryParse(ConfigurationManager.AppSettings["IntervalTime"], out time);
            string serviceName = ConfigurationManager.AppSettings["ServiceName"];
            while (true)
            {
                try
                {
                    ServiceController sc = new ServiceController(serviceName);
                    if ((sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                         (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                    {
                        sc.Start();
                        string msg = string.Format("{1}:服务{0}发生异常，开始重新启动!", serviceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        LogerHelper.WriteOperateLog(msg);
                    }
                    sc.Refresh();                 
                }
                catch (Exception ex)
                {
                    LogerHelper.WriteErrorLog(ex);
                }
                if (thread.ThreadState == ThreadState.Running)
                {
                    Thread.Sleep(time * 1000);
                }
            }
        }

        /// <summary>
        /// 监听指令
        /// </summary>
        private static void MiniCommand()
        {
            try
            {
                var time = 2;
                int.TryParse(ConfigurationManager.AppSettings["ReadTime"], out time);
                while (!IsStoped)
                {
                    try
                    {
                        FileCopyWorker worker = new FileCopyWorker();
                        worker.Run();
                    }
                    catch (Exception e)
                    {
                        LogerHelper.WriteErrorLog(e);
                    }
                    if (threadCommand.ThreadState == ThreadState.Running)
                    {
                        Thread.Sleep(time * 1000);
                    }
                }
            }
            catch (Exception e)
            {
                LogerHelper.WriteErrorLog(e);
            }
        }
    }
}
