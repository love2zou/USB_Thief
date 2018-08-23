using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace WindwoInstalTool
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string sysDisk = System.Environment.SystemDirectory.Substring(0, 3);
            string dotNetPath = sysDisk + @"WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
            //因为当前用的是4.0的环境  
            string serviceEXEPath = ConfigurationManager.AppSettings["ServicePath"]; //把服务的exe程序拷贝到了当前运行目录下，所以用此路径  
            string serviceInstallCommand = string.Format(@"{0}  {1}", dotNetPath, serviceEXEPath); //安装服务时使用的dos命令  
            string serviceUninstallCommand = string.Format(@"{0} -U {1}", dotNetPath, serviceEXEPath); //卸载服务时使用的dos命令  



            if (File.Exists(dotNetPath))
            {
                string[] cmd = new string[] { serviceUninstallCommand };
                string ss = Cmd(cmd);
                Console.WriteLine("卸载成功！");
                CloseProcess("cmd.exe");
            }


        }


        /// <summary>  
        /// 运行CMD命令  
        /// </summary>  
        /// <param name="cmd">命令</param>  
        /// <returns></returns>  
        public static string Cmd(string[] cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;
            for (int i = 0; i < cmd.Length; i++)
            {
                p.StandardInput.WriteLine(cmd[i].ToString());
            }
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strRst;
        }


        /// <summary>  
        /// 关闭进程  
        /// </summary>  
        /// <param name="ProcName">进程名称</param>  
        /// <returns></returns>  
        public static bool CloseProcess(string ProcName)
        {
            bool result = false;
            System.Collections.ArrayList procList = new System.Collections.ArrayList();
            string tempName = "";
            int begpos;
            int endpos;
            foreach (System.Diagnostics.Process thisProc in System.Diagnostics.Process.GetProcesses())
            {
                tempName = thisProc.ToString();
                begpos = tempName.IndexOf("(") + 1;
                endpos = tempName.IndexOf(")");
                tempName = tempName.Substring(begpos, endpos - begpos);
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill(); // 当发送关闭窗口命令无效时强行结束进程  
                    result = true;
                }
            }
            return result;
        }
    }
}