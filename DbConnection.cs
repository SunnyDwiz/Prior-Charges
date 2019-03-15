using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace ServiceNowAppTool.Common
{
    public class DbConnection
    {
        private readonly string _connectionVmrx;
        private readonly string _connectionOnlineBilling;
        private readonly string _connectionPharmericaCommon;
        private readonly string _connectionDevOnlineBilling;
        private readonly string _connectionDocutrack;
        private readonly string _connectionEmanifest;
        private readonly string _connectionSNT;
        private readonly string _connectionPDO;
        private readonly string _connectionprod;
        private readonly string _connectionPriorAuth;
        public DbConnection()
        {
            _connectionVmrx = ConfigurationManager.ConnectionStrings["VmrxCon"].ConnectionString;
            _connectionOnlineBilling = ConfigurationManager.ConnectionStrings["OledbCon"].ConnectionString;
            _connectionDevOnlineBilling = ConfigurationManager.ConnectionStrings["DevOledbCon"].ConnectionString;
            _connectionPharmericaCommon = ConfigurationManager.ConnectionStrings["PharmericaCommonCon"].ConnectionString;
            _connectionDocutrack = ConfigurationManager.ConnectionStrings["Docutrack"].ConnectionString;
            _connectionEmanifest = ConfigurationManager.ConnectionStrings["Emanifest"].ConnectionString;
            _connectionSNT = ConfigurationManager.ConnectionStrings["SNT"].ConnectionString;
            _connectionPDO = ConfigurationManager.ConnectionStrings["PDO"].ConnectionString;
            _connectionprod = ConfigurationManager.ConnectionStrings["PRD_Connection"].ConnectionString;
            _connectionPriorAuth = ConfigurationManager.ConnectionStrings["PriorAuth"].ConnectionString;
        }

        protected SqlConnection GetDbConnection(string app)
        {
            string con = "";
            switch (app)
            {
                case "Vmrx":
                    con = _connectionVmrx;
                    break;
                case "OnlineBilling":
                    con = _connectionOnlineBilling;
                    break;
                case "DevOnlineBilling":
                    con = _connectionDevOnlineBilling;
                    break;
                case "PharmericaCommon":
                    con = _connectionPharmericaCommon;
                    break;
                case "Docutrack":
                    con = _connectionDocutrack;
                    break;
                case "Emanifest":
                    con = _connectionEmanifest;
                    break;
                case "SNT":
                    con = _connectionSNT;
                    break;
                case "PDO":
                    con = _connectionPDO;
                    break;
                case "PRD_Connection":
                    con = _connectionprod;
                    break;
                case "PriorAuth":
                    con = _connectionPriorAuth;
                    break;
            }
            return new SqlConnection(con);
        }

    }
}