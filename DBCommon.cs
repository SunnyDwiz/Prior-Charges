
using ServiceNowAppTool.Models;
using System;
using System.Collections.Generic;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ServiceNowAppTool.Common
{
    public class DBCommon : DbConnection
    {
        DBQueries dbQueries = new DBQueries();
        public int GetNumwithQuery(string issue, string userId, string loginId)
        {
            string sqlQuery = "";

            switch (issue)
            {
                case "UnlockUser":
                    sqlQuery = string.Format(dbQueries.sqlunlockUserAccount, userId, loginId);
                    break;
                case "ActivateUser":
                    sqlQuery = string.Format(dbQueries.sqlActivateUser, userId, loginId);
                    break;
                case "CheckPendInvoiceNotif":
                    sqlQuery = string.Format(dbQueries.sqlCheckInvoiceNotifications);
                    break;
                case "SendInvoiceNotif":
                    sqlQuery = string.Format(dbQueries.sqlInvoiceNotifications, loginId);
                    break;
                case "UpdateResend":
                    sqlQuery = string.Format(userId);
                    break;
            }
            return ExcecuteReader(sqlQuery, "Vmrx");

        }
        public string GetValueWithQuery(string issue, string userId)
        {
            string sqlQuery = ""; string app = "";

            switch (issue)
            {
                case "RemoveDuplicates":
                    sqlQuery = string.Format(dbQueries.sqlRemoveDuplicates, userId);
                    app = "Vmrx";
                    break;
                case "CheckPendInvoiceNotif":
                    sqlQuery = string.Format(dbQueries.sqlCheckInvoiceNotifications);
                    app = "Vmrx";
                    break;


            }
            return GetValue(sqlQuery, app);
        }
        public List<InvoiceDownLoad> DownloadPDF(List<string> fileNames, InvoiceDownLoad info)
        {
            List<InvoiceDownLoad> lstInvoiceDownLoad = new List<Models.InvoiceDownLoad>();
            try
            {
                string query = "";
                var files = "";
                string sqlQuery = string.Format(dbQueries.sqlFacilityype, info.PharmacyCode, info.FacilityCode, info.CorpCode);
                int facType = ExcecuteScalar(sqlQuery, "PharmericaCommon");
                if (string.IsNullOrEmpty(info.PharmacyCode))
                {
                    sqlQuery = string.Format(dbQueries.sqlPharmacyCode, info.PharmacyCode, info.FacilityCode, info.CorpCode);
                    info.PharmacyCode = ExcecuteScalar(sqlQuery, "PharmericaCommon").ToString();
                }
                foreach (var file in fileNames)
                {
                    files = files + "'" + file + "',";
                }

                if (facType == 1)
                    query = string.Format(dbQueries.sqlPdmiInvoice, files.Remove(files.Length - 1), info.PharmacyCode + '-' + info.FacilityCode);
                else
                    query = string.Format(dbQueries.sqlFfsinvoicedownload, files.Remove(files.Length - 1), info.PharmacyCode + '-' + info.FacilityCode);
                using (var connection = GetDbConnection("OnlineBilling"))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        SqlDataReader dr1 = cmd.ExecuteReader();
                        while (dr1.Read())
                        {
                            InvoiceDownLoad InDld = new InvoiceDownLoad();
                            string fileName = string.Empty;
                            InDld.FileName = dr1["FileName"].ToString();
                            InDld.ID = dr1["ID"].ToString();
                            InDld.Archived = dr1["Archieve"].ToString();
                            InDld.isPdmi = facType;
                            if (facType == 0)
                                InDld.FfsOld = dr1["GetFile"].ToString();
                            string file = fileName;
                            lstInvoiceDownLoad.Add(InDld);
                        }
                        dr1.Close();
                    }
                }
            }
            catch (Exception msg)
            {

            }
            return lstInvoiceDownLoad;

        }
        public InvoiceDownLoad GetFile(int id, int isPdmi, string isffsOld, string archived)
        {
            InvoiceDownLoad InDld = new InvoiceDownLoad(); string sqlQuery = "";
            if (isPdmi == 1 && archived == "NARC")
            {
                sqlQuery = string.Format(dbQueries.sqlPdmiinvoiceblob, id);
            }
            else if (isPdmi == 1 && archived == "ARC")
            {
                sqlQuery = string.Format(dbQueries.sqlArchPdmiinvoiceblob, id);
            }
            else if (isPdmi == 0 && isffsOld == "NEW" && archived == "NARC")
            {
                sqlQuery = string.Format(dbQueries.sqlNewFfsinvoiceblob, id);
            }
            else if (isPdmi == 0 && isffsOld == "OLD" && archived == "NARC")
            {
                sqlQuery = string.Format(dbQueries.sqlOldFfsinvoiceblob, id);
            }
            else if (isPdmi == 0 && isffsOld == "NEW" && archived == "ARC")
            {
                sqlQuery = string.Format(dbQueries.sqlArchNewFfsinvoiceblob, id);
            }
            else if (isPdmi == 0 && isffsOld == "OLD" && archived == "ARC")
            {
                sqlQuery = string.Format(dbQueries.sqlArchOldFfsinvoiceblob, id);
            }
            using (var connection = GetDbConnection("OnlineBilling"))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, connection))
                {
                    connection.Open();
                    SqlDataReader dr1 = cmd.ExecuteReader();
                    while (dr1.Read())
                    {

                        string fileName = string.Empty;
                        InDld.FileName = dr1["FileName"].ToString();

                        InDld.BlobData = (byte[])dr1.GetValue(1);

                    }
                    dr1.Close();

                }
            }
            return InDld;
        }
        public int ExcecuteReader(string sqlQuery, string app)
        {
            try
            {

                using (var connection = GetDbConnection(app))
                {
                    using (var cmd = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        int res = Convert.ToInt32(cmd.ExecuteNonQuery());
                        return res;
                    }
                }
            }
            catch (SqlException ex)
            {
                return 0;
            }
            finally
            {

            }
        }
        public int ExcecuteNonQuery(string sqlQuery, string app)
        {
            try
            {

                using (var connection = GetDbConnection(app))
                {
                    using (var cmd = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        int res = Convert.ToInt32(cmd.ExecuteNonQuery());
                        return res;
                    }
                }
            }
            catch (SqlException ex)
            {
                return 0;
            }
        }
        internal int ExcecuteScalar(string sqlQuery, string app)
        {
            try
            {

                using (var connection = GetDbConnection(app))
                {
                    using (var cmd = new SqlCommand(sqlQuery, connection))
                    {
                        connection.Open();
                        int res = Convert.ToInt32(cmd.ExecuteScalar());
                        connection.Close();
                        return res;
                    }
                }
            }
            catch (SqlException ex)
            {
                return 0;
            }
        }
        internal List<UserActivity> UserActivity(string username, string app)
        {
            List<UserActivity> userList = new List<UserActivity>();
            try
            {
                string query = "";
                query = string.Format(dbQueries.sqluserActivity, username);
                var con = GetDbConnection(app);

                SqlCommand com = new SqlCommand(query, con);
                com.CommandType = CommandType.Text;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);
                con.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    userList.Add(
                        new UserActivity
                        {
                            AuthUser = Convert.ToString(dr["AuthUser"]),
                            CreatedDate = Convert.ToString(dr["CreatedDate"]),
                            IpAddress = Convert.ToString(dr["IpAddress"]),
                            BusinessRole = Convert.ToString(dr["BusinessRole"]),
                            FacilityId = Convert.ToString(dr["FacilityId"]),
                            ActionName = Convert.ToString(dr["ActionName"]),
                            ScriptName = Convert.ToString(dr["ScriptName"]),
                            IsActive = Convert.ToString(dr["IsActive"]),
                            IsLocked = Convert.ToString(dr["IsLocked"]),
                            UserId = Convert.ToInt16(dr["userid"])
                        }
                        );
                }

            }
            catch(Exception ex)
            {

            }
            return userList;
        }
        public string GetValue(string query, string app)
        {
            string value = "";
            try
            {
                using (var connection = GetDbConnection(app))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        //cmd.CommandTimeout = 90;
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                            value = dr[0].ToString();

                    }
                }
            }
            catch (Exception msg)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return value;
        }
        public string AddPharmacy(EmanifestPhmc pcm)
        {
            string result = "";
            try
            {
                string query = string.Format(dbQueries.sqlAddPh_Emanifest, pcm.Pharmacy, pcm.Pharmacyname, pcm.Address1, string.IsNullOrEmpty(pcm.Address2) ? "" : pcm.Address2, string.IsNullOrEmpty(pcm.City) ? "" : pcm.City,
                pcm.State, string.IsNullOrEmpty(pcm.Phone) ? "" : pcm.Phone, string.IsNullOrEmpty(pcm.Fax) ? "" : pcm.Fax, pcm.ZipCode, string.IsNullOrEmpty(pcm.DocuTrackEmail) ? "" : pcm.DocuTrackEmail);
                result = GetValue(query, "Emanifest");
            }
            catch (Exception ex)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return result;
        }
        public string AddPharmacy(DocuTrackPhmc pcm)
        {
            string result = "";
            try
            {
                string query = dbQueries.sqlAddPh_DocuTrack;
                SqlParameter[] sqlParameter = new SqlParameter[6];
                sqlParameter[0] = new SqlParameter(DBQueries.sqlPrm_phcode, System.Data.SqlDbType.VarChar);
                sqlParameter[0].Value = pcm.PharmericaPharmacyCode;
                sqlParameter[1] = new SqlParameter(DBQueries.sqlPrm_folderId, System.Data.SqlDbType.VarChar);
                sqlParameter[1].Value = pcm.DocuTrackFolderId;
                sqlParameter[2] = new SqlParameter(DBQueries.sqlPrm_connectionstring, System.Data.SqlDbType.VarChar);
                sqlParameter[2].Value = pcm.DocuTrackSqlConnectionString;
                sqlParameter[3] = new SqlParameter(DBQueries.sqlPrm_webServiceUri, System.Data.SqlDbType.VarChar);
                sqlParameter[3].Value = pcm.DocuTrackWebServiceUri;
                sqlParameter[4] = new SqlParameter(DBQueries.sqlPrm_active, System.Data.SqlDbType.VarChar);
                sqlParameter[4].Value = pcm.Active == true ? 1 : 0;
                sqlParameter[5] = new SqlParameter(DBQueries.sqlPrm_version, System.Data.SqlDbType.VarChar);
                sqlParameter[5].Value = pcm.DocuTrackVersion ?? (object)DBNull.Value;

                result = ExecuteSP("Docutrack", query, sqlParameter);
            }
            catch (Exception ex)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return result.ToString();
        }
        public string AddPharmacy(SNT objSnt, string loginId)
        {
            string result = "";
            try
            {
                string query = string.Format(dbQueries.sqlAddPh_SNT, loginId);
                SqlParameter[] sqlParameter = new SqlParameter[15];
                sqlParameter[0] = new SqlParameter(DBQueries.sqlPrm_phcode, System.Data.SqlDbType.VarChar);
                sqlParameter[0].Value = objSnt.PharmacyCode ?? (object)DBNull.Value;
                sqlParameter[1] = new SqlParameter(DBQueries.sqlprm_phname, System.Data.SqlDbType.VarChar);
                sqlParameter[1].Value = objSnt.PharmacyName ?? (object)DBNull.Value;
                sqlParameter[2] = new SqlParameter(DBQueries.sqlPrm_sqlconn, System.Data.SqlDbType.VarChar);
                sqlParameter[2].Value = objSnt.SqlConnection ?? (object)DBNull.Value;
                sqlParameter[3] = new SqlParameter(DBQueries.sqlPrm_regid, System.Data.SqlDbType.Int);
                sqlParameter[3].Value = objSnt.RegionId;
                sqlParameter[4] = new SqlParameter(DBQueries.sqlPrm_dstid, System.Data.SqlDbType.Int);
                sqlParameter[4].Value = objSnt.DistrictId;
                sqlParameter[5] = new SqlParameter(DBQueries.sqlPrm_pkifolder, System.Data.SqlDbType.VarChar);
                sqlParameter[5].Value = objSnt.pkiFolder;
                sqlParameter[6] = new SqlParameter(DBQueries.sqlPrm_siteorderthreshold, System.Data.SqlDbType.VarChar);
                sqlParameter[6].Value = objSnt.SiteOrderThreshhold;
                sqlParameter[7] = new SqlParameter(DBQueries.sqlPrm_docutrackcachelife, System.Data.SqlDbType.VarChar);
                sqlParameter[7].Value = objSnt.DocuTrackCacheLife;
                sqlParameter[8] = new SqlParameter(DBQueries.sqlPrm_serviceURI, System.Data.SqlDbType.VarChar);
                sqlParameter[8].Value = objSnt.ServiceURI ?? (object)DBNull.Value;
                sqlParameter[9] = new SqlParameter(DBQueries.sqlPrm_temptimezone, System.Data.SqlDbType.VarChar);
                sqlParameter[9].Value = objSnt.TempTimeZone ?? (object)DBNull.Value;
                sqlParameter[10] = new SqlParameter(DBQueries.sqlPrm_timezoneid, System.Data.SqlDbType.VarChar);
                sqlParameter[10].Value = objSnt.TimeZoneId;
                sqlParameter[11] = new SqlParameter(DBQueries.sqlPrm_deanumber, System.Data.SqlDbType.VarChar);
                sqlParameter[11].Value = objSnt.DeaNumber ?? (object)DBNull.Value;
                sqlParameter[12] = new SqlParameter(DBQueries.sqlprm_excludedmetrics, System.Data.SqlDbType.VarChar);
                sqlParameter[12].Value = objSnt.ExcludeFromMetrics == true ? 1 : 0;
                sqlParameter[13] = new SqlParameter(DBQueries.sqlPrm_enableforsurecostbeta, System.Data.SqlDbType.VarChar);
                sqlParameter[13].Value = objSnt.EnableForSureCostBeta == true ? 1 : 0;
                sqlParameter[14] = new SqlParameter(DBQueries.sqlPrm_uploadtosurecost, System.Data.SqlDbType.VarChar);
                sqlParameter[14].Value = objSnt.UploadToSureCost == true ? 1 : 0;

                result = ExecuteSP("SNT", query, sqlParameter);

            }
            catch (Exception ex)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return result.ToString();
        }
        public string AddPharmacy(PriorAuthPhmc objPrior, string loginId)
        {
            string result = "";
            try
            {
                string query = string.Format(dbQueries.sqlAddPharPriorAuth, loginId);
                SqlParameter[] sqlParameter = new SqlParameter[17];
                sqlParameter[0] = new SqlParameter(DBQueries.sqlPrm_phid, System.Data.SqlDbType.VarChar);
                sqlParameter[0].Value = objPrior.PHARMACY_CODE;
                sqlParameter[1] = new SqlParameter(DBQueries.sqlPrm_phcode, System.Data.SqlDbType.VarChar);
                sqlParameter[1].Value = objPrior.PHARMACY_CODE;
                sqlParameter[2] = new SqlParameter(DBQueries.sqlPrm_cid, System.Data.SqlDbType.VarChar);
                sqlParameter[2].Value = objPrior.CORP_ID;
                sqlParameter[3] = new SqlParameter(DBQueries.sqlPrm_regid, System.Data.SqlDbType.VarChar);
                sqlParameter[3].Value = objPrior.REGION_ID;
                sqlParameter[4] = new SqlParameter(DBQueries.sqlPrm_regname, System.Data.SqlDbType.VarChar);
                sqlParameter[4].Value = objPrior.REGION_NAME ?? (object)DBNull.Value;
                sqlParameter[5] = new SqlParameter(DBQueries.sqlPrm_addr1, System.Data.SqlDbType.VarChar);
                sqlParameter[5].Value = objPrior.ADDR_L1 ?? (object)DBNull.Value;
                sqlParameter[6] = new SqlParameter(DBQueries.sqlPrm_addr2, System.Data.SqlDbType.VarChar);
                sqlParameter[6].Value = objPrior.ADDR_L2 ?? (object)DBNull.Value;
                sqlParameter[7] = new SqlParameter(DBQueries.sqlPrm_city, System.Data.SqlDbType.VarChar);
                sqlParameter[7].Value = objPrior.CITY ?? (object)DBNull.Value;
                sqlParameter[8] = new SqlParameter(DBQueries.sqlPrm_state, System.Data.SqlDbType.VarChar);
                sqlParameter[8].Value = objPrior.STATE ?? (object)DBNull.Value;
                sqlParameter[9] = new SqlParameter(DBQueries.sqlPrm_plusfour, System.Data.SqlDbType.VarChar);
                sqlParameter[9].Value = objPrior.PLUS_FOUR ?? (object)DBNull.Value;
                sqlParameter[10] = new SqlParameter(DBQueries.sqlPrm_pnumber, System.Data.SqlDbType.VarChar);
                sqlParameter[10].Value = objPrior.PHONE_NUM ?? (object)DBNull.Value;
                sqlParameter[11] = new SqlParameter(DBQueries.sqlPrm_active, System.Data.SqlDbType.Bit);
                sqlParameter[11].Value = objPrior.ACTIVE_IND == true ? 1 : 0;
                //sqlParameter[12] = new SqlParameter(DBQueries.sqlprm_excludedmetrics, System.Data.SqlDbType.VarChar);
                //sqlParameter[12].Value = objPrior.ExcludeFromMetrics == true ? 1 : 0;
                sqlParameter[12] = new SqlParameter(DBQueries.sqlPrm_manualflag, System.Data.SqlDbType.VarChar);
                sqlParameter[12].Value = objPrior.MANUALFLAG == true ? 1 : 0;
                sqlParameter[13] = new SqlParameter(DBQueries.sqlPrm_faxnum, System.Data.SqlDbType.VarChar);
                sqlParameter[13].Value = objPrior.FAX_NUM ?? (object)DBNull.Value;
                sqlParameter[14] = new SqlParameter(DBQueries.sqlprm_phname, System.Data.SqlDbType.VarChar);
                sqlParameter[14].Value = objPrior.PHARMACY_NAME ?? (object)DBNull.Value;
                sqlParameter[15] = new SqlParameter(DBQueries.sqlPrm_phdesc, System.Data.SqlDbType.VarChar);
                sqlParameter[15].Value = objPrior.PHARMACY_NAME ?? (object)DBNull.Value;
                sqlParameter[16] = new SqlParameter(DBQueries.sqlPrm_zip, System.Data.SqlDbType.VarChar);
                sqlParameter[16].Value = objPrior.ZIP ?? (object)DBNull.Value;
                result = ExecuteSP("PriorAuth", query, sqlParameter);
            }
            catch (Exception ex)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return result;
        }
        public string AddPharmacyPharComm(PharmericaCommPhmc objPharComm, string loginId)
        {
            string result = "";
            try
            {
                string query = string.Format(dbQueries.sqlAddPh_Pmcomn, loginId);
                SqlParameter[] sqlParameter = new SqlParameter[20];
                sqlParameter[0] = new SqlParameter(DBQueries.sqlPrm_pharcode, System.Data.SqlDbType.VarChar);
                sqlParameter[0].Value = objPharComm.PharmacyCode ?? (object)DBNull.Value;
                sqlParameter[1] = new SqlParameter(DBQueries.sqlprm_pharname, System.Data.SqlDbType.VarChar);
                sqlParameter[1].Value = objPharComm.PharmacyName ?? (object)DBNull.Value;
                sqlParameter[2] = new SqlParameter(DBQueries.sqlPrm_generalManager, System.Data.SqlDbType.VarChar);
                sqlParameter[2].Value = objPharComm.GeneralManager ?? (object)DBNull.Value;
                sqlParameter[3] = new SqlParameter(DBQueries.sqlPrm_address, System.Data.SqlDbType.VarChar);
                sqlParameter[3].Value = objPharComm.Address ?? (object)DBNull.Value;
                sqlParameter[4] = new SqlParameter(DBQueries.sqlPrm_hours, System.Data.SqlDbType.VarChar);
                sqlParameter[4].Value = objPharComm.Hours ?? (object)DBNull.Value;
                sqlParameter[5] = new SqlParameter(DBQueries.sqlPrm_phone_area_Code, System.Data.SqlDbType.VarChar);
                sqlParameter[5].Value = objPharComm.phone_area_code ?? (object)DBNull.Value;
                sqlParameter[6] = new SqlParameter(DBQueries.sqlPrm_phonePrefix, System.Data.SqlDbType.VarChar);
                sqlParameter[6].Value = objPharComm.phone_prefix ?? (object)DBNull.Value;
                sqlParameter[7] = new SqlParameter(DBQueries.sqlPrm_phone_suffix, System.Data.SqlDbType.VarChar);
                sqlParameter[7].Value = objPharComm.phone_suffix ?? (object)DBNull.Value;
                sqlParameter[8] = new SqlParameter(DBQueries.sqlPrm_fax_area_code, System.Data.SqlDbType.VarChar);
                sqlParameter[8].Value = objPharComm.fax_area_code ?? (object)DBNull.Value;
                sqlParameter[9] = new SqlParameter(DBQueries.sqlPrm_fax_prefix, System.Data.SqlDbType.VarChar);
                sqlParameter[9].Value = objPharComm.fax_prefix ?? (object)DBNull.Value;
                sqlParameter[10] = new SqlParameter(DBQueries.sqlPrm_fax_suffix, System.Data.SqlDbType.VarChar);
                sqlParameter[10].Value = objPharComm.fax_suffix ?? (object)DBNull.Value;
                sqlParameter[11] = new SqlParameter(DBQueries.sqlPrm_ActiveFlag.ToString(), System.Data.SqlDbType.Bit);
                sqlParameter[11].Value = objPharComm.ActiveFlag == true ? 1 : 0;
                sqlParameter[12] = new SqlParameter(DBQueries.sqlPrm_Iteration.ToString(), System.Data.SqlDbType.Int);
                sqlParameter[12].Value = objPharComm.Iteration;

                sqlParameter[13] = new SqlParameter(DBQueries.sqlPrm_NCPDPID, System.Data.SqlDbType.VarChar);
                sqlParameter[13].Value = objPharComm.NCPDPID ?? (object)DBNull.Value;
                sqlParameter[14] = new SqlParameter(DBQueries.sqlPrm_NPI, System.Data.SqlDbType.VarChar);
                sqlParameter[14].Value = objPharComm.NPI ?? (object)DBNull.Value;
                sqlParameter[15] = new SqlParameter(DBQueries.sqlPrm_PharmacyEmail, System.Data.SqlDbType.VarChar);
                sqlParameter[15].Value = objPharComm.PharmacyEmail ?? (object)DBNull.Value;

                sqlParameter[16] = new SqlParameter(DBQueries.sqlPrm_CensusEmail, System.Data.SqlDbType.VarChar);
                sqlParameter[16].Value = objPharComm.CensusEmail ?? (object)DBNull.Value;

                sqlParameter[17] = new SqlParameter(DBQueries.sqlPrm_State, System.Data.SqlDbType.VarChar);
                sqlParameter[17].Value = objPharComm.State ?? (object)DBNull.Value;
                sqlParameter[18] = new SqlParameter(DBQueries.sqlprm_UseFoilBack, System.Data.SqlDbType.VarChar);
                sqlParameter[18].Value = objPharComm.UseOnlyFoilback ?? (object)DBNull.Value;
                sqlParameter[19] = new SqlParameter(DBQueries.sqlPrm_SubsidiaryPharmacy, System.Data.SqlDbType.VarChar);
                sqlParameter[19].Value = objPharComm.SubsidiaryPharmacy ?? (object)DBNull.Value;
                //sqlParameter[20] = new SqlParameter(DBQueries.sqlPrm_SubsidiaryPharmacy, System.Data.SqlDbType.VarChar);
                //sqlParameter[20].Value = loginId ?? (object)DBNull.Value;
                result = ExecuteSP("PharmericaCommon", query, sqlParameter);
            }
            catch (Exception ex)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }
            return result;
        }
        public string ExecuteSP(string app, string query, SqlParameter[] parameters)
        {
            string value = "";
            SqlConnection connection = new SqlConnection(); ;
            try
            {
                using (connection = GetDbConnection(app))
                {
                    using (SqlCommand sqlCmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        sqlCmd.CommandType = CommandType.Text;
                        sqlCmd.CommandText = query;
                        sqlCmd.Parameters.Clear();
                        if ((parameters != null))
                        {
                            for (int NCounter = 0; NCounter <= (parameters.Length - 1); NCounter++)
                            {
                                sqlCmd.Parameters.Add(parameters[NCounter]);
                            }
                        }
                        //cmd.CommandTimeout = 90;
                        SqlDataReader dr = sqlCmd.ExecuteReader();
                        if (dr.Read())
                            value = dr[0].ToString();

                    }
                }
            }
            catch (Exception msg)
            {
                return "Please try after some time, If it repeats please reach support team.";
            }

            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return value;
        }
        public List<Region> GetList(string request, string app)
        {
            List<Region> lstRegions = new List<Region>(); var connection = new SqlConnection();
            try
            {
                string query = "";
                if (request == "Regions")
                    query = dbQueries.sqlSNT_GetRegions;
                else if (request == "Districts")
                    query = dbQueries.sqlSNT_GetDistricts;
                else if (request == "TimeZones")
                    query = dbQueries.sqlSNT_GetTimeZones;
                else if (request == "PhrTimeZones")
                    query = dbQueries.sqlPhrTimeZones;
                else if (request == "Corps")
                    query = dbQueries.sqlCorprations;
                else if (request == "PDOPCodes")
                    query = dbQueries.sqlPdoPCodes;

                using (connection = GetDbConnection(app))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        //cmd.CommandTimeout = 90;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lstRegions.Add(
                                   new Region
                                   {
                                       Name = reader.GetString(1),
                                       Id = reader.GetInt32(0)
                                   });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return lstRegions;
        }
        public List<Pharmacy> GetList(string request, string app, String[] param = null)
        {
            List<Pharmacy> lstPharmacys = new List<Pharmacy>(); SqlConnection connection = new SqlConnection();
            try
            {
                string query = "";
                if (request == "Pharmacies")
                    query = dbQueries.sqlGetPharmaciesVmrx;
                else if (request == "Corporations")
                    query = dbQueries.sqlGetCorporationsVmrx;
                else if (request == "Facilities")
                    query = string.Format(dbQueries.sqlGetFacilitiesVmrx, param[0]);
                else if (request == "PDOPCodes")
                    query = dbQueries.sqlPdoPCodes;
                else if (request == "PharmaciesByCorp")
                    query = string.Format(dbQueries.sqlPCodesByCorp, param[0]);
                else if (request == "FacilitiesByCorp")
                    query = string.Format(dbQueries.sqlGetFacilitiesByCorpVmrx, param[0], param[1]);
                using (connection = GetDbConnection(app))
                {
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        connection.Open();
                        //cmd.CommandTimeout = 90;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            lstPharmacys.Add(
                                   new Pharmacy
                                   {
                                       Name = reader.GetString(1),
                                       Id = reader.GetString(0)
                                   });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return lstPharmacys;
        }
        public List<Pharmacy> GetListFromXml(string fromTagName)
        {
            List<Pharmacy> lstPharmacys = new List<Pharmacy>();
            try
            {
                string query = "";
                if (fromTagName == "DocSqlConnec")
                    query = dbQueries.sqlGetDocTraclSqlConnec;
                else if (fromTagName == "SntSqlConnection")
                    query = dbQueries.sqlsntSqlConnection;
                string[] connections = query.Split('>');

                foreach (var con in connections)
                {
                    lstPharmacys.Add(
                           new Pharmacy
                           {
                               Name = con.Split(',')[0],
                               Id = con.Split(',')[1],
                           });


                }
            }
            catch (Exception ex)
            {

            }
            return lstPharmacys;
        }
        public List<T> ConvertToList<T>(DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    if (columnNames.Contains(pro.Name))
                    {
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
                    }
                }
                return objT;
            }).ToList();
        }
        internal DataTable ExecteReader(string query, string app)
        {
            DataTable dataTable = new DataTable(); SqlConnection connection = new SqlConnection();
            try
            {
                using (connection = GetDbConnection(app))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch
            {
                return dataTable;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
    }
}