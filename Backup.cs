using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp20
{
    public class Backup
    {
       public  static string path = @"C:\basf\dane\backup";
        public static void  CREATE_BACKUP(string filePath)
        {

            if (Directory.Exists(path))
            {
                File.Copy(filePath, path);
            }
            else
            {
                Directory.CreateDirectory("backup");
                File.Copy(filePath, path);
            }
        }

    }
}
