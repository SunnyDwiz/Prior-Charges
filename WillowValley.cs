using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Threading;
using ServiceNowAppTool.WillowValleyService;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using System.Web.Mvc;
using ServiceNowAppTool.Models;
using System.Xml.Linq;
using NLog;

namespace ServiceNowAppTool.Common
{
    public class WillowValley : DBCommon
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        DBQueries dbQueries = new DBQueries();
        public void ProcessFeed(string statementdate, string DestPath)
        {
            string FileSourcePath = string.Empty;
            string FileProcessingPath = string.Empty;
            FileSourcePath = ConfigurationManager.AppSettings["FileSourcePath"];
            FileProcessingPath = ConfigurationManager.AppSettings["FileProcessingPath"];

            string GettingFileName = ConfigurationManager.AppSettings["GettingFileName"];

            SqlConnection devObCon = GetDbConnection("DevOnlineBilling");
            SqlConnection prdcon = GetDbConnection("PRD_Connection");
            SqlCommand cmd = new SqlCommand(GettingFileName, prdcon);
            prdcon.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            try
            {
                if (dr.Read())
                {
                    string FileName = dr["FileName"].ToString();
                    cmd = new SqlCommand(string.Format(dbQueries.sqlCheckFeedInDev, FileName), devObCon);
                    if (devObCon.State == ConnectionState.Closed)
                        devObCon.Open();
                    dr = cmd.ExecuteReader();


                    if (dr.Read())
                    {
                        if (Convert.ToInt32(dr["result"]) == 1)
                            GeneratingReport(devObCon, DestPath);
                        else
                        {
                            List<string> processfiles = new List<string>();
                            while (dr.Read())
                            {
                                List<string> F1files = Directory.GetFiles(FileSourcePath)
                                    .Select(path => Path.GetFileName(path))
                                                 .Where(p => Path.GetFileName(p).StartsWith(FileName, StringComparison.OrdinalIgnoreCase))
                                                 .ToList();
                                if (F1files.Count > 0)
                                {
                                    foreach (var file in F1files)
                                    {
                                        File.Copy(FileSourcePath + file, FileProcessingPath + file);
                                    }

                                }
                                var processfiles1 = Directory.GetFiles(FileProcessingPath)
                                    .Select(path => Path.GetFileName(path))
                                                 .Where(p => Path.GetFileName(p).StartsWith(FileName, StringComparison.OrdinalIgnoreCase))
                                                 .ToList();

                                processfiles.AddRange(processfiles1);
                            }

                            prdcon.Close();


                            if (processfiles != null)
                            {
                                string execStatus = ConfigurationManager.AppSettings["Execstatus"];
                                string newexestatus;
                                bool isCompleted = false;
                                string lastExecStatus = string.Empty;
                                string newFile;
                                SqlCommand cmd1;

                                foreach (var file in processfiles)
                                {
                                    isCompleted = false;
                                    newFile = file.Remove(file.IndexOf('.')) + ".ready";
                                    File.Move(FileProcessingPath + file, FileProcessingPath + newFile);
                                    Thread.Sleep(300000);
                                    newexestatus = execStatus.Replace("$", newFile);
                                    cmd1 = new SqlCommand(newexestatus, devObCon);

                                    while (!isCompleted)
                                    {
                                        if (devObCon.State == ConnectionState.Closed)
                                            devObCon.Open();

                                        lastExecStatus = cmd1.ExecuteScalar().ToString();

                                        if (lastExecStatus == "EA006")
                                        {
                                            isCompleted = true;
                                            //Generating report from ssrs and rendering the report and downloading with replacing the name
                                            GeneratingReport(devObCon, DestPath);
                                            //Downloading xls files from prod
                                            // DownloadindExcel_Prod(prdcon);
                                        }
                                        else if (lastExecStatus == "EA007")
                                        {
                                            isCompleted = false;
                                        }
                                        else
                                        {
                                            isCompleted = false;
                                            Thread.Sleep(300000);
                                        }
                                    }
                                }
                                devObCon.Close();
                            }
                        }
                    }
                    else
                    {

                    }


                }
            }
            catch (Exception ex)
            {
                logger.Error("Error occured in willow valley process feed",ex);
            }

        }

        internal bool CheckExistanceOfFolder(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        public string GetStatementDate()
        {
            SqlConnection devObCon = GetDbConnection("PRD_Connection");
            string statementDate = ConfigurationManager.AppSettings["StatementDate"];
            SqlCommand cmd = new SqlCommand(statementDate, devObCon);
            SqlDataReader datareader;
            devObCon.Open();
            datareader = cmd.ExecuteReader();

            if (datareader.Read())
            {
                statementDate = Convert.ToDateTime(datareader["StatementDate"]).ToString("MM-dd-yyyy");
            }
            return statementDate;
        }

        public List<FileInfo> GetFile(string fileSavePath)
        {
            List<FileInfo> listFiles = new List<FileInfo>();
            //Path For download From Network Path.  

            DirectoryInfo dirInfo = new DirectoryInfo(fileSavePath);
            int i = 0;
            foreach (var item in dirInfo.GetFiles())
            {
                listFiles.Add(new FileInfo()
                {
                    FileId = i + 1,
                    FileName = item.Name,
                    FilePath = dirInfo.FullName + @"\" + item.Name
                });
                i = i + 1;
            }
            return listFiles;
        }

        public void GeneratingReport(SqlConnection devcon, string directoryPath)
        {
            ReportExecutionService rsForxls = new ReportExecutionService();
            byte[] bytes = null;
            string ReportPath = ConfigurationManager.AppSettings["ReportPath"];
            string format = "EXCEL";
            string historyID = null;
            string devInfo = @"<DeviceInfo><StreamRoot>/RSWebServiceXS/</StreamRoot><NoHeader>true</NoHeader></DeviceInfo>";
            string reportUserName = ConfigurationManager.AppSettings["ReportUserName"];
            string reportPassword = ConfigurationManager.AppSettings["ReportPassword"];
            string domain = ConfigurationManager.AppSettings["Domain"];
            NetworkCredential objCredentials = new NetworkCredential(reportUserName, reportPassword, domain);
            rsForxls.Credentials = objCredentials;
            //Assigning the parameters
            string statementDate = GetStatementDate();
            ParameterValue[] parameters = new ParameterValue[5];
            parameters[0] = new ParameterValue();
            parameters[0].Name = "AccountNumber";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key.StartsWith("AccountNumber"))
                {
                    parameters[0].Value = ConfigurationManager.AppSettings[key];
                    parameters[1] = new ParameterValue();
                    parameters[1].Name = "ReportType";
                    parameters[1].Value = "NEWFFS";
                    parameters[2] = new ParameterValue();
                    parameters[2].Name = "IncludeSalesTax";
                    parameters[2].Value = ConfigurationManager.AppSettings["IncludeSalesTax"];
                    parameters[3] = new ParameterValue();
                    parameters[3].Name = "StatementDate";
                    parameters[3].Value = statementDate;
                    parameters[4] = new ParameterValue();
                    parameters[4].Name = "IsExcel";
                    parameters[4].Value = ConfigurationManager.AppSettings["IsExcel"];
                    string extension;
                    string encoding;
                    string mimeType;
                    Warning[] warnings = null;
                    string[] streamIDs = null;
                    rsForxls.LoadReport(ReportPath, historyID);
                    rsForxls.SetExecutionParameters(parameters, "en-us");
                    try
                    {
                        bytes = rsForxls.Render(format, devInfo, out extension, out encoding, out mimeType, out warnings, out streamIDs);


                        string file = directoryPath;
                        if (parameters[0].Value == ConfigurationManager.AppSettings["AccountNumber1"])
                        {
                            file = file + "LakeSide.xls";
                        }
                        else if (parameters[0].Value == ConfigurationManager.AppSettings["AccountNumber2"])
                        {
                            file = file + "Meadow View.xls";

                        }
                        else if (parameters[0].Value == ConfigurationManager.AppSettings["AccountNumber3"])
                        {
                            file = file + "The Glen Snf.xls";
                        }
                        else if (parameters[0].Value == ConfigurationManager.AppSettings["AccountNumber4"])
                        {
                            file = file + "The Glen PC.xls";
                        }
                        byte[] fileData = bytes;
                        using (System.IO.FileStream fs = new System.IO.FileStream(file, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite))
                        {
                            using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs))
                            {
                                bw.Write(fileData);
                                bw.Close();
                            }
                        }
                    }
                    catch (SoapException e)
                    {
                        Console.WriteLine(e.Detail.OuterXml);
                    }
                }
            }

        }

        public void UpdateXml(string statmentDate)
        {
            dbQueries.CreateNewTag(statmentDate, "WillowValleyEmail");
        }
        public bool CheckExistance(string statmentDate)
        {
           return dbQueries.CheckExistace(statmentDate, "WillowValleyEmail");
        }
    }

    public class FileInfo
    {
        public int FileId
        {
            get;
            set;
        }
        public string FileName
        {
            get;
            set;
        }
        public string FilePath
        {
            get;
            set;
        }
    }
}