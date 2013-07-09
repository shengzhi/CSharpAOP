using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace G.W.Y.Helper
{
    public static class FileHelper
    {
        /// <summary>
        /// GenerateFile 将字符串写成文件
        /// </summary>       
        public static void GenerateFile(string filePath, string text)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(text);
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
        /// <summary>
        /// GetFileContent 读取文本文件的内容
        /// </summary>       
        public static string GetFileContent(string file_path)
        {
            string result;
            if (!File.Exists(file_path))
            {
                result = null;
            }
            else
            {
                StreamReader streamReader = new StreamReader(file_path, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                streamReader.Close();
                result = text;
            }
            return result;
        }
        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>    
        public static void WriteBuffToFile(byte[] buff, int offset, int len, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(buff, offset, len);
            binaryWriter.Flush();
            binaryWriter.Close();
            fileStream.Close();
        }
        /// <summary>
        /// WriteBuffToFile 将二进制数据写入文件中
        /// </summary>   
        public static void WriteBuffToFile(byte[] buff, string filePath)
        {
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(buff);
            binaryWriter.Flush();
            binaryWriter.Close();
            fileStream.Close();
        }
        /// <summary>
        /// ReadFileReturnBytes 从文件中读取二进制数据
        /// </summary>      
        public static byte[] ReadFileReturnBytes(string filePath)
        {
            byte[] result;
            if (!File.Exists(filePath))
            {
                result = null;
            }
            else
            {
                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                byte[] array = binaryReader.ReadBytes((int)fileStream.Length);
                binaryReader.Close();
                fileStream.Close();
                result = array;
            }
            return result;
        }
   
        /// <summary>
        /// GetFileNameNoPath 获取不包括路径的文件名
        /// </summary>      
        public static string GetFileNameNoPath(string filePath)
        {
            return Path.GetFileName(filePath);
        }
        /// <summary>
        /// GetFileSize 获取目标文件的大小
        /// </summary>        
        public static ulong GetFileSize(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            return (ulong)fileInfo.Length;
        }
        /// <summary>
        /// 获取某个文件夹的大小。
        /// </summary>      
        public static ulong GetDirectorySize(string dirPath)
        {
            ulong result;
            if (!Directory.Exists(dirPath))
            {
                result = 0uL;
            }
            else
            {
                ulong num = 0uL;
                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
                FileInfo[] files = directoryInfo.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = files[i];
                    num += (ulong)fileInfo.Length;
                }
                DirectoryInfo[] directories = directoryInfo.GetDirectories();
                if (directories.Length > 0)
                {
                    for (int j = 0; j < directories.Length; j++)
                    {
                        num += FileHelper.GetDirectorySize(directories[j].FullName);
                    }
                }
                result = num;
            }
            return result;
        }
        /// <summary>
        /// ReadFileData 从文件流中读取指定大小的内容
        /// </summary>       
        public static void ReadFileData(FileStream fs, byte[] buff, int count, int offset)
        {
            int num;
            for (int i = 0; i < count; i += num)
            {
                num = fs.Read(buff, offset + i, count - i);
            }
        }
        /// <summary>
        /// GetFileDirectory 获取文件所在的目录路径
        /// </summary>       
        public static string GetFileDirectory(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }
        /// <summary>
        /// DeleteFile 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        /// <summary>
        /// EnsureExtendName 确保扩展名正确
        /// </summary>       
        public static string EnsureExtendName(string origin_path, string extend_name)
        {
            if (Path.GetExtension(origin_path) != extend_name)
            {
                origin_path += extend_name;
            }
            return origin_path;
        }
        public static void ClearDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            string[] array = files;
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                File.Delete(path);
            }
            array = Directory.GetDirectories(dirPath);
            for (int i = 0; i < array.Length; i++)
            {
                string dirPath2 = array[i];
                FileHelper.DeleteDirectory(dirPath2);
            }
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>        
        public static void DeleteDirectory(string dirPath)
        {
            string[] array = Directory.GetFiles(dirPath);
            for (int i = 0; i < array.Length; i++)
            {
                string path = array[i];
                File.Delete(path);
            }
            array = Directory.GetDirectories(dirPath);
            for (int i = 0; i < array.Length; i++)
            {
                string dirPath2 = array[i];
                FileHelper.DeleteDirectory(dirPath2);
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);
            directoryInfo.Refresh();
            if ((directoryInfo.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                directoryInfo.Attributes &= ~FileAttributes.ReadOnly;
            }
            directoryInfo.Delete();
        }
        /// <summary>
        /// 将某个文件夹下的多个文件和子文件夹移到另外一个目录中。
        /// </summary>
        /// <param name="oldParentDirectoryPath">移动之前文件和子文件夹所处的父目录路径</param>
        /// <param name="filesBeMoved">被移动的文件名称的集合</param>
        /// <param name="directoriesBeMoved">被移动的文件夹名称的集合</param>
        /// <param name="newParentDirectoryPath">移往的目标文件夹路径</param>
        public static void Move(string oldParentDirectoryPath, IEnumerable<string> filesBeMoved, IEnumerable<string> directoriesBeMoved, string newParentDirectoryPath)
        {
            if (filesBeMoved != null)
            {
                foreach (string current in filesBeMoved)
                {
                    string text = oldParentDirectoryPath + current;
                    if (File.Exists(text))
                    {
                        File.Move(text, newParentDirectoryPath + current);
                    }
                }
            }
            if (directoriesBeMoved != null)
            {
                foreach (string current in directoriesBeMoved)
                {
                    string text = oldParentDirectoryPath + current;
                    if (Directory.Exists(text))
                    {
                        Directory.Move(text, newParentDirectoryPath + current);
                    }
                }
            }
        }
        /// <summary>
        /// 拷贝多个文件和文件夹。
        /// </summary>
        /// <param name="sourceParentDirectoryPath">被拷贝的文件和文件夹所处的父目录路径</param>
        /// <param name="filesBeCopyed">被复制的文件名称的集合</param>
        /// <param name="directoriesCopyed">被复制的文件夹名称的集合</param>
        /// <param name="destParentDirectoryPath">目标目录的路径</param>
        public static void Copy(string sourceParentDirectoryPath, IEnumerable<string> filesBeCopyed, IEnumerable<string> directoriesCopyed, string destParentDirectoryPath)
        {
            bool flag = sourceParentDirectoryPath == destParentDirectoryPath;
            if (filesBeCopyed != null)
            {
                foreach (string current in filesBeCopyed)
                {
                    string text = current;
                    while (flag && File.Exists(destParentDirectoryPath + text))
                    {
                        text = "副本-" + text;
                    }
                    string text2 = sourceParentDirectoryPath + current;
                    if (File.Exists(text2))
                    {
                        File.Copy(text2, destParentDirectoryPath + text);
                    }
                }
            }
            if (directoriesCopyed != null)
            {
                foreach (string current in directoriesCopyed)
                {
                    string text = current;
                    while (flag && Directory.Exists(destParentDirectoryPath + text))
                    {
                        text = "副本-" + text;
                    }
                    string text2 = sourceParentDirectoryPath + current;
                    if (Directory.Exists(text2))
                    {
                        FileHelper.CopyDirectoryAndFiles(sourceParentDirectoryPath, current, destParentDirectoryPath, text);
                    }
                }
            }
        }
        /// <summary>
        /// 递归拷贝文件夹以及下面的所有文件
        /// </summary>       
        private static void CopyDirectoryAndFiles(string sourceParentDirectoryPath, string dirBeCopyed, string destParentDirectoryPath, string newDirName)
        {
            Directory.CreateDirectory(destParentDirectoryPath + newDirName);
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceParentDirectoryPath + dirBeCopyed);
            FileInfo[] files = directoryInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                File.Copy(fileInfo.FullName, destParentDirectoryPath + newDirName + "\\" + fileInfo.Name);
            }
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo directoryInfo2 = directories[i];
                FileHelper.CopyDirectoryAndFiles(sourceParentDirectoryPath + dirBeCopyed + "\\", directoryInfo2.Name, destParentDirectoryPath + newDirName + "\\", directoryInfo2.Name);
            }
        }
        /// <summary>
        /// 获取目标目录下以及其子目录下的所有文件（采用相对路径）。
        /// </summary>        
        public static List<string> GetOffspringFiles(string dirPath)
        {
            List<string> result = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            FileHelper.DoGetOffspringFiles(dirPath, dir, ref result);
            return result;
        }
        private static void DoGetOffspringFiles(string rootPath, DirectoryInfo dir, ref List<string> list)
        {
            FileInfo[] files = dir.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                list.Add(fileInfo.FullName.Substring(rootPath.Length));
            }
            DirectoryInfo[] directories = dir.GetDirectories();
            for (int i = 0; i < directories.Length; i++)
            {
                DirectoryInfo dir2 = directories[i];
                FileHelper.DoGetOffspringFiles(rootPath, dir2, ref list);
            }
        }
    }
}
