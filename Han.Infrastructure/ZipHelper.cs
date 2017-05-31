using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

namespace Han.Infrastructure
{
    public class ZipHelper
    {
        #region 加压解压方法
        /// <summary>
        /// 功能：压缩文件（暂时只压缩文件夹下一级目录中的文件，文件夹及其子级被忽略）
        /// </summary>
        /// <param name="dirPath">被压缩的文件夹夹路径</param>
        /// <param name="zipFilePath">生成压缩文件的路径，为空则默认与被压缩文件夹同一级目录，名称为：文件夹名+.zip</param>
        /// <param name="err">出错信息</param>
        /// <returns>是否压缩成功</returns>
        public static bool ZipFile(string dirPath, string zipFilePath, out string err)
        {
            err = "";
            if (dirPath == string.Empty)
            {
                err = "要压缩的文件夹不能为空！";
                return false;
            }
            if (!Directory.Exists(dirPath))
            {
                err = "要压缩的文件夹不存在！";
                return false;
            }
            //压缩文件名为空时使用文件夹名＋.zip
            if (zipFilePath == string.Empty)
            {
                if (dirPath.EndsWith("\\"))
                {
                    dirPath = dirPath.Substring(0, dirPath.Length - 1);
                }
                zipFilePath = dirPath + ".zip";
            }

            try
            {
                string[] filenames = Directory.GetFiles(dirPath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zipFilePath)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 功能：解压zip格式的文件。
        /// </summary>
        /// <param name="zipFilePath">压缩文件路径</param>
        /// <param name="unZipDir">解压文件存放路径,为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹</param>
        /// <param name="err">出错信息</param>
        /// <returns>解压是否成功</returns>
        public static bool UnZipFile(string zipFilePath, string unZipDir, out string err)
        {
            err = "";
            if (zipFilePath == string.Empty)
            {
                err = "压缩文件不能为空！";
                return false;
            }
            if (!File.Exists(zipFilePath))
            {
                err = "压缩文件不存在！";
                return false;
            }
            //解压文件夹为空时默认与压缩文件同一级目录下，跟压缩文件同名的文件夹
            if (unZipDir == string.Empty)
                unZipDir = zipFilePath.Replace(Path.GetFileName(zipFilePath), Path.GetFileNameWithoutExtension(zipFilePath));
            if (!unZipDir.EndsWith("\\"))
                unZipDir += "\\";
            if (!Directory.Exists(unZipDir))
                Directory.CreateDirectory(unZipDir);

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
                {

                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string directoryName = Path.GetDirectoryName(theEntry.Name);
                        string fileName = Path.GetFileName(theEntry.Name);
                        if (directoryName.Length > 0)
                        {
                            Directory.CreateDirectory(unZipDir + directoryName);
                        }
                        if (!directoryName.EndsWith("\\"))
                            directoryName += "\\";
                        if (fileName != String.Empty)
                        {
                            using (FileStream streamWriter = File.Create(unZipDir + theEntry.Name))
                            {

                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);
                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }//while
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
            return true;
        }
        #endregion


        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="patch"></param>
        /// <param name="rarPatch"></param>
        /// <param name="rarName"></param>
        public static bool CompressRAR(string patch, string rarPatch, string rarName)
        {
            try
            {
                RegistryKey the_Reg = Registry.LocalMachine.OpenSubKey(
                   @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
                object the_Obj = the_Reg.GetValue("");
                string the_rar = the_Obj.ToString();
                the_Reg.Close();
                if (Directory.Exists(rarPatch) == false)
                {
                    Directory.CreateDirectory(rarPatch);
                }
                string ls = rarPatch + "\\" + rarName;

                string the_Info = "a -as -r " + ls + " * ";

                ProcessStartInfo the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = patch;//获取压缩包路径
                Process the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
                the_Process.WaitForExit();
                the_Process.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="unRarPatch">解压后的文件路径</param>
        /// <param name="rarPatch">需要解压的文件路径</param>
        /// <param name="rarName">文件名</param>
        /// <param name="err">错误信息</param>
        /// <returns></returns>
        public static void UnRar(string unRarPath, string rarPath, string rarName)
        {
            string fileFullName = Path.Combine(rarPath ,rarName);

            if (Directory.Exists(unRarPath) == false)
            {
                Directory.CreateDirectory(unRarPath);
            }

            RegistryKey registry = Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");

            ProcessStartInfo ProStartInfo = new ProcessStartInfo();
            ProStartInfo.FileName = registry.GetValue("").ToString();
            ProStartInfo.Arguments = "x " + rarName + " " + unRarPath + " -y"; ;
            ProStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ProStartInfo.WorkingDirectory = rarPath;//获取压缩包路径

            Process process = new Process();
            process.StartInfo = ProStartInfo;
            process.Start();
            process.WaitForExit();

            process.Close();
            registry.Close();
        }

        /// <summary>
        /// 判断是否安装RAR软件
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        private static bool Exists()
        {
            RegistryKey registry = Registry.LocalMachine.OpenSubKey(
                    @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");

            return !string.IsNullOrEmpty(registry.GetValue("").ToString());
        }
    }
}
