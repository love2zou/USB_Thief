using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace WindwoInstalTool
{
    public class LogerHelper
    {
        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteErrorLog(string message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Logger\\ErrorLog.txt";
            WriteFile(path, "错误信息：" + message);
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorLog(Exception ex)
        {
            try
            {
                string message = string.Empty;
                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;

                }
                else
                {
                    message = ex.Message;
                }
                if (ex.StackTrace != null)
                {
                    message += ex.StackTrace;
                }
                WriteErrorLog(message);

            }
            catch (Exception ex1)
            {

            }
        }

        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteErrorLog(Exception ex, string messageout)
        {
            try
            {
                string message = string.Empty;

                if (ex.InnerException != null)
                {
                    message = ex.InnerException.Message;
                }
                else
                {
                    message = ex.Message;
                }
                message += messageout;
                if (ex.StackTrace != null)
                {
                    message += ex.StackTrace;
                }
                WriteErrorLog(message);

            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteOperateLog(string message)
        {
            bool isNeedOperateLog = true;
            if (isNeedOperateLog)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Logger\\OperateLog.txt";
                WriteFile(path, "操作信息：" + message);
            }
        }

        /// <summary>
        /// 写操作日志
        /// </summary>
        /// <param name="message"></param>
        public static void WriteOperateLog(StringFormat message)
        {
            bool isNeedOperateLog = false;
            if (isNeedOperateLog)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "Logger\\OperateLog.txt";
                WriteFile(path, "操作信息：" + message);
            }
        }



        private static void WriteFile(string path, string message)
        {
            try
            {
                path = path.Replace(".txt", DateTime.Now.ToString("yyyyMMdd") + ".txt");
                StreamWriter write = null;
                if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Logger"))
                {
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Logger");
                }
                if (!File.Exists(path))
                {
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    write = new StreamWriter(fs);
                }
                else
                {
                    write = File.AppendText(path);
                }
                write.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + message);
                write.Flush();
                write.Close();
            }
            catch (Exception ex)
            {
            }
        }





    }
}