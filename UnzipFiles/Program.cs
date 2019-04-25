using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnzipFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            string startPath = ConfigurationManager.AppSettings["StartPath"];    //@D:\Files\Downloaded
            string outputPath = ConfigurationManager.AppSettings["OutputPath"];  //@D:\Files\Output
            string defaultFileName = ConfigurationManager.AppSettings["DefaultFileName"]; //Stat Zip Manuscript Blob
            var patientsDirectories = Directory.GetDirectories(startPath);

            foreach (var patientDir in patientsDirectories)
            {
                string finalPatientDir = outputPath + patientDir.Substring(patientDir.LastIndexOf("\\"));
                if (!Directory.Exists(finalPatientDir))
                {
                    Directory.CreateDirectory(finalPatientDir);
                }
                DirectoryInfo patientDirInfo = new DirectoryInfo(patientDir);
                FileInfo[] patientDirFileInfos = patientDirInfo.GetFiles();
                var files = Directory.GetFiles(patientDir);
                foreach (var fileInfo in patientDirFileInfos)
                {
                    int i = 1;
                    var newFileName = Path.GetFileName(fileInfo.FullName).Substring(0, Path.GetFileName(fileInfo.FullName).LastIndexOf('.'));
                    try
                    {
                        if (File.Exists(finalPatientDir + "\\" + defaultFileName))
                        {
                            File.Delete(finalPatientDir + "\\" + defaultFileName);
                        }
                        System.IO.Compression.ZipFile.ExtractToDirectory(fileInfo.FullName, finalPatientDir);
                        var subDirInfo = new DirectoryInfo(finalPatientDir);
                        FileInfo createdFileInfo = subDirInfo.GetFiles(defaultFileName).FirstOrDefault();

                        if (createdFileInfo != null)
                        {
                            if (File.Exists(finalPatientDir + "\\" + newFileName + ".docx"))
                            {
                                newFileName = newFileName + "_" + i;
                                i++;
                            }
                            File.Move(createdFileInfo.FullName, finalPatientDir + "\\" + newFileName + ".docx");
                            //File.Copy(createdFileInfo.FullName, @outputDirectory + "\\" + fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('.')) + ".docx");
                            //File.Delete(createdFileInfo.FullName);
                        }
                    }
                    catch (Exception ex)
                    {
                        var e = ex;
                        WriteLog("FolderName: " + patientDir + " , FileName: " + newFileName + ", Exception: " + ex.Message);

                    }
                }
            }
        }
        public static bool WriteLog(string strMessage)
        {
            try
            {
                FileStream objFilestream = new FileStream(string.Format("{0}\\{1}", ConfigurationManager.AppSettings["LogFilePath"],
                                                                "logs.txt"),
                                                            FileMode.Append, FileAccess.Write);
                StreamWriter objStreamWriter = new StreamWriter(objFilestream);
                objStreamWriter.WriteLine(strMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
