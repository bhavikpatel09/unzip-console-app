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
            Console.WriteLine("Started with Total directories : " + patientsDirectories.Length);
            foreach (var patientDir in patientsDirectories)
            {
                string finalPatientDir = outputPath + patientDir.Substring(patientDir.LastIndexOf("\\"));
                Console.WriteLine("finalPatientDir : " + finalPatientDir);
                if (!string.IsNullOrEmpty(finalPatientDir))
                {
                    try
                    {
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
                            Console.WriteLine("newFileName (" + i + "): " + newFileName);
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
                                        Console.WriteLine("After File Exist newFileName (" + i + "): " + newFileName);
                                        i++;
                                    }
                                   string extension =  DetermineFileType(createdFileInfo.FullName);
                                    File.Move(createdFileInfo.FullName, finalPatientDir + "\\" + newFileName + extension);
                                    //File.Copy(createdFileInfo.FullName, @outputDirectory + "\\" + fileInfo.FullName.Substring(0, fileInfo.FullName.LastIndexOf('.')) + ".docx");
                                    //File.Delete(createdFileInfo.FullName);
                                }
                            }
                            catch (Exception ex)
                            {
                                var e = ex;
                                WriteLog("FolderName: " + patientDir + " , FileName: " + newFileName + ", Exception: " + ex.Message);
                                Console.WriteLine("FolderName: " + patientDir + " , FileName: " + newFileName + ", Exception: " + ex.Message);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var e = ex;
                        WriteLog("FolderName: " + patientDir + ", finalPatientDir: " + finalPatientDir + ", Exception: " + ex.Message);
                        Console.WriteLine("FolderName: " + patientDir + ", finalPatientDir: " + finalPatientDir + ", Exception: " + ex.Message);
                    }
                }
            }
            Console.WriteLine("Ended processing!!!");
            Console.ReadLine();
        }

        private static string DetermineFileType(string fullName)
        {
            string extension = "";
            var contentType = MimeTypes.GetContentType(fullName);
            if (contentType.StartsWith("application/pdf"))
            {
                extension = ".pdf";
            }
            else if (contentType.StartsWith("text/plain"))
            {
                extension = ".txt";
            }
            else if (contentType.StartsWith("text/xml"))
            {
                extension = ".xml";
            }
            else if (contentType.StartsWith("image/gif"))
            {
                extension = ".gif";
            }
            else if (contentType.StartsWith("image/jpeg"))
            {
                extension = ".jpg";
            }
            else if (contentType.StartsWith("image/png"))
            {
                extension = ".png";
            }
            else if (contentType.StartsWith("image/bmp"))
            {
                extension = ".bmp";
            }
            else if (contentType.StartsWith("text/html"))
            {
                extension = ".html";
            }
            else if (contentType.StartsWith("application/excel"))
            {
                extension = ".xlsx";
            }
            else
            {
                extension = ".docx";
            }
            /*
             "",
        "",
        "image/pjpeg",
        "",
        "image/x-png",
        
        "",*/
            return extension;
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
