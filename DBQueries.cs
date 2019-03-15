using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace ServiceNowAppTool.Common
{
    public class DBQueries
    {
     
        public const string xmlCredentialsPath = "~/Common/Credentials.xml";
        public const string xmlDbQueriesPath = @"~/Common/DbQueries.xml";

        public string sqlRemoveDuplicates = GetData(xmlDbQueriesPath, "removedupicates");
        public string sqlFacilityype = GetData(xmlDbQueriesPath, "facilitytype");
        public string sqlPharmacyCode = GetData(xmlDbQueriesPath, "PharmacyCode");
        public string sqlPdmiInvoice = GetData(xmlDbQueriesPath, "pdmiinvoicedownload");
        public string sqlPdmiinvoiceblob = GetData(xmlDbQueriesPath, "pdmiinvoiceblob");
        public string sqlFfsinvoicedownload = GetData(xmlDbQueriesPath, "ffsinvoicedownload");
        public string sqlNewFfsinvoiceblob = GetData(xmlDbQueriesPath, "Newffsinvoicedownload");
        public string sqlOldFfsinvoiceblob = GetData(xmlDbQueriesPath, "Oldffsinvoicedownload");
        public string sqluserActivity = GetData(xmlDbQueriesPath, "useractivity");
        public string sqlunlockUserAccount = GetData(xmlDbQueriesPath, "UnlockUserAccount");
        public string sqlActivateUser = GetData(xmlDbQueriesPath, "ActivateUser");
        public string sqlInvoiceNotifications = GetData(xmlDbQueriesPath, "InvoiceNotifications");
        public string sqlCheckInvoiceNotifications = GetData(xmlDbQueriesPath, "CheckPendInvoiceNotifications");

        public string sqlArchPdmiinvoiceblob = GetData(xmlDbQueriesPath, "Archpdmiinvoiceblob");
        public string sqlArchNewFfsinvoiceblob = GetData(xmlDbQueriesPath, "ArchNewffsinvoicedownload");
        public string sqlArchOldFfsinvoiceblob = GetData(xmlDbQueriesPath, "ArchOldffsinvoicedownload");

        //WillowValley
        public string sqlCheckFeedInDev = GetData(xmlDbQueriesPath, "CheckFeedInDev");

        //Add Pharmacy
        public string sqlAddPh_Pmcomn = GetData(xmlDbQueriesPath, "AddPharPharmericaComn");
        public string sqlAddPh_DocuTrack = GetData(xmlDbQueriesPath, "AddPharDocuTrack");
        public string sqlAddPh_Emanifest = GetData(xmlDbQueriesPath, "AddPharEmanifest");
        public string sqlSNT_GetDistricts = GetData(xmlDbQueriesPath, "GetDistricts");
        public string sqlSNT_GetRegions = GetData(xmlDbQueriesPath, "GetRegions");
        public string sqlSNT_GetTimeZones = GetData(xmlDbQueriesPath, "GetTimeZones");
        public string sqlAddPh_PDO = GetData(xmlDbQueriesPath, "AddPDO");
        public string sqlAddPh_SNT = GetData(xmlDbQueriesPath, "AddPharSnt");
        public string sqlPhrTimeZones = GetData(xmlDbQueriesPath, "PhrTimeZones");
        public string sqlCorprations = GetData(xmlDbQueriesPath, "Corprations");
        public string sqlAddPharPriorAuth = GetData(xmlDbQueriesPath, "AddPharPriorAuth");
        public String sqlGetDocTraclSqlConnec = GetData(xmlDbQueriesPath, "DocutrackSqlConnection");
        public String sqlsntSqlConnection = GetData(xmlDbQueriesPath, "sntSqlConnection");
        //Add Pharmacist
        public string sqlActivatePDOPharmacist = GetData(xmlDbQueriesPath, "ActivatePDOPharmacist");
        public string sqlAddPh_Pharmacist = GetData(xmlDbQueriesPath, "AddPharmacist");
        public string sqlPdoPCodes = GetData(xmlDbQueriesPath, "GetPharmacies");
        public string sqlPCodesByCorp = GetData(xmlDbQueriesPath, "GetPharmaciesByCorp");
        public string AddPDOPharmacist = GetData(xmlDbQueriesPath, "AddPDOPharmacist");

        public string sqlCheckPDOPharmacist = GetData(xmlDbQueriesPath, "CheckPDOPharmacist");
        public string sqlGetPDOPharmacists = GetData(xmlDbQueriesPath, "GetPDOPharmacists");
        public string sqlDeactivatePDOPharmacist = GetData(xmlDbQueriesPath, "DeactivatePDOPharmacist");

        public string sqlGetCorporationsVmrx = GetData(xmlDbQueriesPath, "GetCorporationsVmrx");
        public string sqlGetPharmaciesVmrx = GetData(xmlDbQueriesPath, "GetPharmaciesVmrx");
        public string sqlGetFacilitiesVmrx = GetData(xmlDbQueriesPath, "GetFacilitiesVmrx");
        public string sqlGetFacilitiesByCorpVmrx = GetData(xmlDbQueriesPath, "GetFacilitiesByCorpVmrx");
        public string sqlResendInvNotifProcess = GetData(xmlDbQueriesPath, "ResendInvoiceNotificaionsProcess");
        public string sqlGetStatementDate = GetData(xmlDbQueriesPath, "GetStatementDate");
        public string sqlUpdateResendInvoiceNotif = GetData(xmlDbQueriesPath, "ResendInvoiceNotif");

        public const string sqlPrm_phid = "@PHARMACY_ID";
        public const string sqlPrm_phcode = "@PHARMACY_CODE";
        public const string sqlPrm_cid = "@CORP_ID";
        public const string sqlPrm_regid = "@Region_Id";
        public const string sqlPrm_regname = "@Region_Name";
        public const string sqlprm_phname = "@PHARMACY_NAME";
        public const string sqlPrm_phdesc = "@PHARMACY_DESC";
        public const string sqlPrm_addr1 = "@ADDR_L1";
        public const string sqlPrm_addr2 = "@ADDR_L2";
        public const string sqlPrm_city = "@CITY";
        public const string sqlPrm_state = "@STATE";
        public const string sqlPrm_zip = "@ZIP";
        public const string sqlPrm_plusfour = "@PLUS_FOUR";
        public const string sqlPrm_pnumber = "@PHONE_NUM";
        public const string sqlPrm_active = "@ACTIVE_IND";
        public const string sqlprm_excludedmetrics = "@ExcludeFromMetrics";
        public const string sqlPrm_manualflag = "@MANUALFLAG";
        public const string sqlPrm_faxnum = "@FAX_NUM";

        public const string sqlPrm_folderId = "@DocuTrackFolderId";
        public const string sqlPrm_connectionstring = "@DocuTrackSqlConnectionString";
        public const string sqlPrm_webServiceUri = "@DocuTrackWebServiceUri";
        public const string sqlPrm_version = "@Version";
        public const string sqlPrm_sqlconn = "@SqlConnection";
        public const string sqlPrm_dstid = "@DistrictId";
        public const string sqlPrm_pkifolder = "@pkiFolder";
        public const string sqlPrm_siteorderthreshold = "@SiteOrderThreshhold";
        public const string sqlPrm_docutrackcachelife = "@DocuTrackCacheLife";
        public const string sqlPrm_serviceURI = "@ServiceURI";
        public const string sqlPrm_temptimezone = "@TempTimeZone";
        public const string sqlPrm_timezoneid = "@TimeZoneId";
        public const string sqlPrm_deanumber = "@DeaNumber";
        public const string sqlPrm_enableforsurecostbeta = "@EnableForSureCostBeta";
        public const string sqlPrm_uploadtosurecost = "@UploadToSureCost";

        public const string sqlPrm_generalManager = "@GeneralManager";
        public const string sqlPrm_address = "@Address";
        public const string sqlPrm_hours = "@Hours";
        public const string sqlPrm_phone_area_Code = "@phone_area_code";
        public const string sqlPrm_phonePrefix = "@phone_prefix";
        public const string sqlPrm_phone_suffix = "@phone_suffix";
        public const string sqlPrm_fax_area_code = "@fax_area_code";
        public const string sqlPrm_fax_prefix = "@fax_prefix";
        public const string sqlPrm_fax_suffix = "@fax_suffix";
        public const string sqlPrm_Iteration = "@Iteration";
        public const string sqlPrm_CreateUserId = "@CreateUserId";
        public const string sqlPrm_CreateTimeStamp = "@CreateTimeStamp";
        public const string sqlPrm_ActiveFlag = "@ActiveFlag";
        public const string sqlPrm_updateUserID = "@UpdateUserId";
        public const string sqlPrm_UpdateTimeStamp = "@UpdateTimeStamp";
        public const string sqlPrm_NCPDPID = "@NCPDPID";
        public const string sqlPrm_NPI = "@NPI";
        public const string sqlPrm_CensusEmail = "@CensusEmail";
        public const string sqlPrm_State = "@State";
        public const string sqlPrm_SubsidiaryPharmacy = "@SubsidiaryPharmacy";
        public const string sqlPrm_PharmacyEmail = "@PharmacyEmail";
        public const string sqlPrm_pharcode = "@PHARMACYCODE";
        public const string sqlprm_pharname = "@PHARMACYNAME";
        public const string sqlprm_UseFoilBack = "@UseOnlyFoilback";


        //Display messages
        public const string msgEmailSuccess = "Email sent successfully!!!";
        public const string msgEmailFailed = "Email sending Failed. Please contact web support team.";
        public const string msgUpPendingNotifSuccess = "We identified an issue and it is fixed and invoices are sending now, Please verify pending count after 10 minutes.";
        public const string msgUpPendingNotifFailed = "Please verify count after 10 minutes, if count remains same  - please escalate to web support team with priority.";
        public const string msgPharmacyCreationSuccess="Added successfully.";
        public const string msgChangePassSuccess="Password changed successfully.";
        public const string msgCurrentPwdNotMatch = "Current password should be match.";

        public static string GetData(string xmlPath, string tagName)
        {
            string sqlQuery = "";
            string path = HttpContext.Current.Server.MapPath(xmlPath);
            var doc = XDocument.Load(path);
            sqlQuery = doc.Descendants("viewmaster")
    .Select(x => x.Element(tagName).Value).FirstOrDefault();
            return sqlQuery;
        }

        public bool CreateNewTag(string tagToAdd, string tagName)
        {
            string spath = HttpContext.Current.Server.MapPath(xmlDbQueriesPath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(spath);
            XmlNode nonFuel = xmlDoc.SelectSingleNode("//" + tagName);
            nonFuel.RemoveAll();

            XmlNode xmlRecordNo = xmlDoc.CreateTextNode(tagToAdd);

            nonFuel.AppendChild(xmlRecordNo);
            xmlDoc.Save(spath);
            return true;
        }

        public bool CheckExistace(string valueToCheck, string rootTagName)
        {
            string spath = HttpContext.Current.Server.MapPath(xmlDbQueriesPath);
            XDocument doc = XDocument.Load(spath);
            var result = doc.Descendants(rootTagName).FirstOrDefault();
            if (result.Value == valueToCheck)
                return true;
            else

                return false;
        }
    }
}