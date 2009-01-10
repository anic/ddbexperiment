using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DistDBMS.UserInterface.Handler
{
    class FileUploader
    {
        /// <summary>
        /// 将文本文件读成string数组
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string[] ReadFileToString(string fileName)
        {
            List<string> result = new List<string>();

            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default);

                while (!sr.EndOfStream)
                    result.Add(sr.ReadLine());
                sr.Close();
            }
            return result.ToArray();
        }

        public static byte[] ReadFileToByte(string fileName)
        {

            FileStream pFileStream = null;

            byte[] pReadByte = new byte[0];

            try
            {

                pFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                BinaryReader r = new BinaryReader(pFileStream);

                r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开

                pReadByte = r.ReadBytes((int)r.BaseStream.Length);

                return pReadByte;

            }

            catch
            {

                return pReadByte;

            }

            finally
            {

                if (pFileStream != null)

                    pFileStream.Close();

            }

        }

    }
}
