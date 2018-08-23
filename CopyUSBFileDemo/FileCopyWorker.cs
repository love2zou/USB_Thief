using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CopyUSBFileDemo
{
    public class FileCopyWorker
    {
        public const int WM_DEVICECHANGE = 0x219;//U盘插入后，OS的底层会自动检测到，然后向应用程序发送“硬件设备状态改变“的消息
        public const int DBT_DEVICEARRIVAL = 0x8000;  //就是用来表示U盘可用的。一个设备或媒体已被插入一块，现在可用。
        public const int DBT_CONFIGCHANGECANCELED = 0x0019;  //要求更改当前的配置（或取消停靠码头）已被取消。
        public const int DBT_CONFIGCHANGED = 0x0018;  //当前的配置发生了变化，由于码头或取消固定。
        public const int DBT_CUSTOMEVENT = 0x8006; //自定义的事件发生。 的Windows NT 4.0和Windows 95：此值不支持。
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;  //审批要求删除一个设备或媒体作品。任何应用程序也不能否认这一要求，并取消删除。
        public const int DBT_DEVICEQUERYREMOVEFAILED = 0x8002;  //请求删除一个设备或媒体片已被取消。
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004;  //一个设备或媒体片已被删除。
        public const int DBT_DEVICEREMOVEPENDING = 0x8003;  //一个设备或媒体一块即将被删除。不能否认的。
        public const int DBT_DEVICETYPESPECIFIC = 0x8005;  //一个设备特定事件发生。
        public const int DBT_DEVNODES_CHANGED = 0x0007;  //一种设备已被添加到或从系统中删除。
        public const int DBT_QUERYCHANGECONFIG = 0x0017;  //许可是要求改变目前的配置（码头或取消固定）。
        public const int DBT_USERDEFINED = 0xFFFF;  //此消息的含义是用户定义的

        public static string copyPath = ConfigurationManager.AppSettings["copyPath"];

        public static int totalCount = 0;//文件拷贝总数
        public static int successCount = 0;//文件拷贝成功个数
  
        public void Run()
        { 
            if (!Directory.Exists(copyPath))
            {
                Directory.CreateDirectory(copyPath);
            }

            DriveInfo[] s = DriveInfo.GetDrives();//读取计算机所有磁盘
            foreach (DriveInfo i in s)
            {
                if (i.DriveType == DriveType.Removable)//判断是否是可移动存储设备
                {
                    //LogerHelper.WriteOperateLog("**********开始读取U盘**********");
                    getDirectory(i.Name);
                    //LogerHelper.WriteOperateLog(string.Format("拷贝完成，拷贝文件总数：{0}个，拷贝成功：{1}个，拷贝失败：{2}个\n\r**********完成U盘内容拷贝**********", totalCount, successCount, totalCount - successCount));
                }
            }
        }

        /*
         * 获得指定路径下所有文件名
         * string path  文件路径
         */
        public static void getFileName(string path)
        {
            DirectoryInfo root = new DirectoryInfo(path);
            foreach (FileInfo f in root.GetFiles())
            {
                string newcopypath = copyPath + "\\" + path.Substring(3, path.Length - 3);//创建新的文件夹目录
                if (!Directory.Exists(newcopypath))
                {
                    Directory.CreateDirectory(newcopypath);
                }
                newcopypath = newcopypath + "\\" + f.Name;//生成新的文件
                if (!File.Exists(newcopypath))
                {
                    try
                    {
                        f.CopyTo(newcopypath);
                        successCount += 1;
                        totalCount += 1;
                        LogerHelper.WriteOperateLog(string.Format("拷贝成功，成功个数：{0},原路径：{1} => 存储新路径：{2}", successCount, f.FullName, newcopypath));
                    }
                    catch(Exception ex)
                    {
                        totalCount += 1;
                        LogerHelper.WriteOperateLog(string.Format("拷贝失败，失败个数：{0},原路径：{1} => 存储新路径：{2}，错误信息：{3}", totalCount - successCount, f.FullName, newcopypath, ex.Message));
                        continue;
                    }
                }
            }
        }

        //获得指定路径下所有子目录名
        public static void getDirectory(string path)
        {
            getFileName(path);
            DirectoryInfo root = new DirectoryInfo(path);
            //过滤系统的隐藏文件夹
            DirectoryInfo[] dirs = root.GetDirectories().Where(c => !(c.Attributes.ToString().IndexOf("Hidden") > -1 && c.Attributes.ToString().IndexOf("System") > -1)).ToArray();
            foreach (DirectoryInfo d in dirs)
            {
                getDirectory(d.FullName);
            }
        }   
    }
}
