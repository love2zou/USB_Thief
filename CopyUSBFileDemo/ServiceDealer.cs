using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace CopyUSBFileDemo
{
    public class ServiceDealer
    {
        /// <summary>
        /// 重启服务
        /// </summary>
        public static void RestartService()
        {
            var serviceName = ConfigurationManager.AppSettings["ServiceName"];
            string msg = string.Format("{1}:服务{0}发生异常，开始重新启动!", serviceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            LogerHelper.WriteOperateLog(msg);
            //停止服务
            StopService();
            //启动服务
            StartService(serviceName);            
        }

        /// <summary>  
        /// 启动某个服务  
        /// </summary>  
        /// <param name="serviceName"></param>  
        private static void StartService(string serviceName)
        {
            try
            {
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));//等待服务运行
                    }
                }
                string msg = string.Format("{1}:服务{0}启动完成!", serviceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                LogerHelper.WriteOperateLog(msg);
            }
            catch (Exception ex)
            {
                LogerHelper.WriteErrorLog(ex);
            }
        }

        /// <summary>  
        /// 停止某个服务  
        /// </summary>  
        /// <param name="serviceName"></param>  
        private static void StopService()
        {
            try
            {
                var serviceName = ConfigurationManager.AppSettings["ServiceName"];
                ServiceController[] services = ServiceController.GetServices();
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == serviceName)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 30));//等待服务停止
                    }
                }

                string  msg = string.Format("{1}:服务{0}停止完成!", serviceName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                LogerHelper.WriteOperateLog(msg);
            }
            catch (Exception ex)
            {
                LogerHelper.WriteErrorLog(ex);
            }
        }
    }
}
