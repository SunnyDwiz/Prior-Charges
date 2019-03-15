using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceNowAppTool.Common;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;
using ServiceNowAppTool.Models;

namespace ServiceNowAppTool.Controllers
{
   // [IsAuthenticatedAttribute]
    public class IncidentsController : Controller
    {
        //
        // GET: /Incidents/

        DBCommon dbCommon = new DBCommon();

        [HttpGet]
        public ActionResult RemoveDuplicates()
        {
            return View("UnmapDuplicatedFacilities");

        }

        [HttpPost]
        public ActionResult RemoveDuplicates(string userid)
        {
            string result = dbCommon.GetValueWithQuery("RemoveDuplicates", userid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult InvoiceDownload()
        {
            InvoiceDownLoad invDownload = new InvoiceDownLoad();
            invDownload.LstPharmacies = dbCommon.GetList("Pharmacies", "Vmrx", null);
            invDownload.LstCorporations = dbCommon.GetList("Corporations", "Vmrx", null);
            return View(invDownload);
        }
        [HttpPost]

        public ActionResult InvoiceDownload(InvoiceDownLoad invoiceDownload)
        {
            DateTime startDate = DateTime.ParseExact(invoiceDownload.StatementDate, "MM-dd-yyyy", null);
            DateTime endDate = DateTime.ParseExact(invoiceDownload.EndDate, "MM-dd-yyyy", null);
            List<string> fileNames = new List<string>();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                fileNames.Add("'+@accountno +'-" + date.ToString("MMddyyyy") + ".pdf");
            }
            var data = dbCommon.DownloadPDF(fileNames, invoiceDownload);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public FileContentResult DownloadInvoice(int id, int isPdmi, string isFfsOld, string isArch)
        {
            var result = dbCommon.GetFile(id, isPdmi, isFfsOld, isArch);
            return File(result.BlobData, "application/pdf", result.FileName);
        }
        [HttpGet]
        public ActionResult WillowValley()
        {
            WillowValley objWillowValley = new WillowValley();
            WillowValleyModel mdlWillowValleyModel = new WillowValleyModel();
            mdlWillowValleyModel.StatementDate = objWillowValley.GetStatementDate();
            String folder = Server.MapPath(string.Format("~/WillowValley/{0}/", mdlWillowValleyModel.StatementDate));
            mdlWillowValleyModel.Active = objWillowValley.CheckExistanceOfFolder(folder);
            mdlWillowValleyModel.IsEmailSent = objWillowValley.CheckExistance(mdlWillowValleyModel.StatementDate);
            return View(mdlWillowValleyModel);
        }

        public ActionResult WillowValleyDownload(WillowValleyModel mdlWillowValleyModel, string submit)
        {
            WillowValley objWillowValley = new WillowValley();
            mdlWillowValleyModel.StatementDate = objWillowValley.GetStatementDate();
            String folder = Server.MapPath(string.Format("~/WillowValley/{0}/", mdlWillowValleyModel.StatementDate));
            mdlWillowValleyModel.Active = objWillowValley.CheckExistanceOfFolder(folder);
            mdlWillowValleyModel.IsEmailSent = objWillowValley.CheckExistance(mdlWillowValleyModel.StatementDate);
            if (submit == "Download")
            {
                return GenarateZip(objWillowValley, folder, false);
            }
            else if (submit == "Process")
            {
                objWillowValley.ProcessFeed(mdlWillowValleyModel.StatementDate, folder);
                return View("WillowValley", mdlWillowValleyModel);
            }
            else if (submit == "Send Email" && mdlWillowValleyModel.IsEmailSent == false)
            {
                bool isEmailSent = WillowValleySendEmail(objWillowValley, folder);
                if (isEmailSent)
                {
                    objWillowValley.UpdateXml(mdlWillowValleyModel.StatementDate);
                    mdlWillowValleyModel.IsEmailSent = true;
                }
                ViewBag.Message = isEmailSent ? DBQueries.msgEmailSuccess: DBQueries.msgEmailFailed;
                return View("WillowValley", mdlWillowValleyModel);
            }
            else
                return View("WillowValley", mdlWillowValleyModel);


        }

        private ActionResult GenarateZip(WillowValley objWillowValley, string folder, bool sendEmail)
        {
            var filesCol = objWillowValley.GetFile(folder).ToList();
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
                    }
                }
                MemoryStream attachmentStream = new MemoryStream(memoryStream.ToArray());
                if (sendEmail) Email.SendEmail(attachmentStream, folder, true);
                return File(memoryStream.ToArray(), "application/zip", "WillowValley.zip");
            }
        }
        private bool WillowValleySendEmail(WillowValley objWillowValley, string folder)
        {
            var filesCol = objWillowValley.GetFile(folder).ToList();
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    for (int i = 0; i < filesCol.Count; i++)
                    {
                        ziparchive.CreateEntryFromFile(filesCol[i].FilePath, filesCol[i].FileName);
                    }
                }
                MemoryStream attachmentStream = new MemoryStream(memoryStream.ToArray());
                return Email.SendEmail(attachmentStream, folder, true);
            }
        }
        [HttpGet]
        public ActionResult InvoiceNotifications()
        {
            ViewBag.PendingNotificications = dbCommon.GetValueWithQuery("CheckPendInvoiceNotif", "");
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePendingInvoiceNotifications()
        {
            DBCommon commObj = new DBCommon();
            var result = commObj.GetNumwithQuery("SendInvoiceNotif", "", getUserId());
            string displayMsg = result > 0 ? DBQueries.msgUpPendingNotifSuccess : DBQueries.msgUpPendingNotifFailed;
            return Json(displayMsg, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ResendInvoiceNotifications()
        {
            InvNotifications invNotifications = new InvNotifications();
            invNotifications.LstPharmacies = dbCommon.GetList("Pharmacies", "Vmrx", null);
            return View(invNotifications);
        }
        [HttpPost]
        public ActionResult ResendInvoiceNotifications(InvNotifications ResendNotifications)
        {
            DBCommon commObj = new DBCommon();
            DBQueries dbQueries = new DBQueries();
            string query = string.Format(dbQueries.sqlResendInvNotifProcess, ResendNotifications.PharmacyCode, ResendNotifications.FacilityCode, ResendNotifications.Email);
            var dtresult = commObj.ExecteReader(query, "Vmrx"); var result = 0; var lastInvoiedDate = "";
            if (dtresult.Rows.Count > 0)
            {
                result = Convert.ToInt16(dtresult.Rows[0][0]); lastInvoiedDate = dtresult.Rows[0][1].ToString();
            }
            query = string.Format(dbQueries.sqlGetStatementDate, ResendNotifications.PharmacyCode, ResendNotifications.FacilityCode);
            string statementdate = commObj.GetValue(query, "OnlineBilling");
            var fresult = new { result, lastInvoiedDate, statementdate };
            return Json(fresult, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UpdateResendInvNotif(InvNotifications ResendNotifications)
        {
            DBCommon commObj = new DBCommon();
            DBQueries dbQueries = new DBQueries();
            string query = string.Format(dbQueries.sqlUpdateResendInvoiceNotif, ResendNotifications.PharmacyCode, ResendNotifications.FacilityCode, ResendNotifications.Email,getUserId());
            int result = commObj.GetNumwithQuery("UpdateResend", query, getUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserActivity()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserActivity(string userID)
        {
            DBCommon commObj = new DBCommon();
            var result = commObj.UserActivity(userID, "Vmrx");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UnlockUserAccount(string userID, string Action)
        {
            DBCommon commObj = new DBCommon(); int res = 0;
            if (Action == "Unlock")
                res = commObj.GetNumwithQuery("UnlockUser", userID, getUserId());
            else if (Action == "Activate")
                res = commObj.GetNumwithQuery("ActivateUser", userID, getUserId());
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PharmacyCreation(string view)
        {
            switch (view)
            {
                case "ViewMasterPhmc":
                    return View("ViewMasterPhmc", new ViewMasterPhmc());
                case "EcensusPhmc":
                    return View("EcensusPhmc", new EcensusPhmc());
                case "EmanifestPhmc":
                    return View("EmanifestPhmc", new EmanifestPhmc());
                case "PharmericaCommPhmc":
                    return View("PharmericaCommPhmc", new PharmericaCommPhmc());
                case "PriorAuthPhmc":
                    PriorAuthPhmc objPrior = new PriorAuthPhmc();
                    objPrior.ListRegions = dbCommon.GetList("Regions", "SNT");
                    objPrior.ListCorps = dbCommon.GetList("Corps", "SNT");
                    return View("PriorAuthPhmc", objPrior);

                case "RealTimeDataMartPhmc":
                    SNT objSNT = new SNT();
                    objSNT.ListSqlConnections = dbCommon.GetListFromXml("SntSqlConnection");
                    objSNT.ListCorps = dbCommon.GetList("Corps", "SNT");
                    objSNT.ListRegions = dbCommon.GetList("Regions", "SNT");
                    objSNT.Districts = dbCommon.GetList("Districts", "SNT");
                    objSNT.TimeZones = dbCommon.GetList("TimeZones", "SNT"); objSNT.PharTimeZones = dbCommon.GetList("PhrTimeZones", "SNT");
                    return View("RealTimeDataMartPhmc", objSNT);
                case "DocuTrackPhmc":
                    DocuTrackPhmc objDoc = new DocuTrackPhmc();
                    objDoc.SqlConnections = dbCommon.GetListFromXml("DocSqlConnec");
                    return View("DocuTrackPhmc", objDoc);
                default:
                    return View("404");
            }

        }
        [HttpPost]
        public ActionResult PharmacyCreation(FormCollection collection)
        {
            return View();

        }
        [HttpPost]
        public ActionResult AddPharPharmericaCommon(PharmericaCommPhmc pharmericaCommPhmc)
        {
            TempData["data"] = "";
            if (ModelState.IsValid)
            {
                TempData["data"] = dbCommon.AddPharmacyPharComm(pharmericaCommPhmc, getUserId());
            }
            if (TempData["data"].ToString() == DBQueries.msgPharmacyCreationSuccess)
                return RedirectToAction("PharmacyCreation", new { view = "PharmericaCommPhmc" });
            else
                return View("PharmericaCommPhmc", pharmericaCommPhmc);
        }

        public ActionResult AddPharmDocuTrack(DocuTrackPhmc docuTrackCommPhmc)
        {
            TempData["data"] = "";
            if (ModelState.IsValid)
                TempData["data"] = dbCommon.AddPharmacy(docuTrackCommPhmc);
            if (TempData["data"].ToString() == DBQueries.msgPharmacyCreationSuccess)
                return RedirectToAction("PharmacyCreation", new { view = "DocuTrackPhmc" });
            else
            {
                docuTrackCommPhmc.SqlConnections = dbCommon.GetListFromXml("DocSqlConnec");
                return View("DocuTrackPhmc", docuTrackCommPhmc);
            }

        }
        public ActionResult AddPharmEmanifest(EmanifestPhmc emainfest)
        {
            TempData["data"] = "";
            if (ModelState.IsValid)
            {
                TempData["data"] = dbCommon.AddPharmacy(emainfest);
            }
            if (TempData["data"].ToString() == DBQueries.msgPharmacyCreationSuccess)
                return RedirectToAction("PharmacyCreation", new { view = "EmanifestPhmc" });
            else
                return View("EmanifestPhmc", emainfest);
        }
        [HttpPost]
        public ActionResult AddSNTPharmacy(SNT snt)
        {
            TempData["data"] = "";
            if (ModelState.IsValid)
                TempData["data"] = dbCommon.AddPharmacy(snt, getUserId());
            if (TempData["data"].ToString() == DBQueries.msgPharmacyCreationSuccess)
                return RedirectToAction("PharmacyCreation", new { view = "RealTimeDataMartPhmc" });
            else
            {

                snt.ListCorps = dbCommon.GetList("Corps", "SNT");
                snt.ListSqlConnections = dbCommon.GetListFromXml("SntSqlConnection");
                snt.ListRegions = dbCommon.GetList("Regions", "SNT");
                snt.Districts = dbCommon.GetList("Districts", "SNT");
                snt.TimeZones = dbCommon.GetList("TimeZones", "SNT");
                snt.PharTimeZones = dbCommon.GetList("PhrTimeZones", "SNT");
                return View("RealTimeDataMartPhmc", snt);
            }
        }
        public ActionResult AddPriorAuthPharmacy(PriorAuthPhmc objPriorAuth)
        {
            TempData["data"] = "";

            if (ModelState.IsValid)
            {
                TempData["data"] = dbCommon.AddPharmacy(objPriorAuth, getUserId());
            }
            if (TempData["data"].ToString() == DBQueries.msgPharmacyCreationSuccess)
                return RedirectToAction("PharmacyCreation", new { view = "PriorAuthPhmc" });
            else
            {
                objPriorAuth.ListRegions = dbCommon.GetList("Regions", "SNT");
                //objPrior.Districts = dbCommon.GetList("Districts");
                objPriorAuth.ListCorps = dbCommon.GetList("Corps", "SNT"); //objPrior.PharTimeZones = dbCommon.GetList("PhrTimeZones");
                int REGION_ID = Convert.ToInt16(objPriorAuth.REGION_ID);
                objPriorAuth.REGION_NAME = objPriorAuth.ListCorps.Where(s => s.Id == REGION_ID).SingleOrDefault().Name;
                return View("PriorAuthPhmc", objPriorAuth);
            }

        }
        [HttpGet]
        public ActionResult AddPharmacist()//PDO
        {
            ViewBag.PharmacyCodes = dbCommon.GetList("PDOPCodes", "PDO", null);
            return View();
        }
        [HttpPost]
        public ActionResult CheckPharmacist(PDO pdo)//PDO
        {
            if (pdo != null)
            {
                DBQueries queryObj = new DBQueries();
                string sqlQuery = string.Format(queryObj.sqlCheckPDOPharmacist, pdo.PHARMACY_ID, pdo.FIRST_NAME, pdo.LAST_NAME);
                string result = dbCommon.GetValue(sqlQuery, "PDO");
                if (Convert.ToInt16(result) < 1)
                {
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ActivatePDOPharmacist(int id)
        {
            DBQueries queryObj = new DBQueries();
            string sqlQuery = string.Format(queryObj.sqlActivatePDOPharmacist, id, getUserId());
            var result = dbCommon.ExcecuteNonQuery(sqlQuery, "PDO");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddPharmacist(PDO pdo)//PDO
        {
            int result;
            if (pdo != null)
            {
                DBQueries queryObj = new DBQueries();
                string sqlQuery = string.Format(queryObj.AddPDOPharmacist, pdo.PHARMACY_ID, pdo.FIRST_NAME, pdo.LAST_NAME, getUserId());
                result = dbCommon.ExcecuteNonQuery(sqlQuery, "PDO");

                if (result == 1)
                {
                    return Json("Y", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchPharmacist()//PDO
        {
            ViewBag.PharmacyCodes = dbCommon.GetList("PDOPCodes", "PDO", null);
            return View();
        }
        public ActionResult DeactivatePDOPharmacist(int id)
        {
            DBQueries queryObj = new DBQueries();
            string sqlQuery = string.Format(queryObj.sqlDeactivatePDOPharmacist, id, getUserId());
            var result = dbCommon.ExcecuteNonQuery(sqlQuery, "PDO");
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SearchPharmacist(int id, string pharmacist)//PDO
        {
            List<PDO> list = null;
            if (id != 0)
            {
                DBQueries queryObj = new DBQueries();
                string sqlQuery = string.Format(queryObj.sqlGetPDOPharmacists, id, pharmacist);
                var result = dbCommon.ExecteReader(sqlQuery, "PDO");
                list = dbCommon.ConvertToList<PDO>(result);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetFacilities(string PharmacyCode)
        {
            string[] parms = new string[1];
            parms[0] = PharmacyCode;
           
            var facilities = dbCommon.GetList("Facilities", "Vmrx", parms);
            return Json(facilities, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFacilitiesByCorpandPhar(string PharmacyCode, string CorporationCode)
        {
            string[] parms = new string[2];
            parms[0] = PharmacyCode;
            parms[1] = CorporationCode;
            var facilities = dbCommon.GetList("FacilitiesByCorp", "Vmrx", parms);
            return Json(facilities, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetPharmacies(string CorpCode)
        {
            string[] parms = new string[1];
            parms[0] = CorpCode;
            var facilities = dbCommon.GetList("PharmaciesByCorp", "Vmrx", parms);
            return Json(facilities, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]

        public ActionResult ChangePassword(string cureentPwd, string newPwd)
        {
            var path = Server.MapPath(DBQueries.xmlCredentialsPath);
            Models.LoginModel user = null;
            using (LoginModel obj = new LoginModel())
            {
                var credentials = obj.readXml(path);
                user = credentials.Find(x => x.loginId == HttpContext.User.Identity.Name && x.password == cureentPwd);
            }
            if (user != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlNodeList aDateNodes = doc.SelectNodes("/user/UserDetails");
                foreach (XmlNode aDateNode in aDateNodes)
                {
                    if (aDateNode.FirstChild.InnerText == user.loginId)
                    {
                        aDateNode.SelectSingleNode("Password").InnerText = newPwd;
                        break;
                    }
                }
                doc.Save(path);
                return Json(DBQueries.msgChangePassSuccess, JsonRequestBehavior.AllowGet);
            }
            return Json(DBQueries.msgCurrentPwdNotMatch, JsonRequestBehavior.AllowGet);
        }

        private string getUserId()
        {
            string currentLoggedInUser=string.Empty;
            try
            {
                currentLoggedInUser = HttpContext.User.Identity.Name;                
            }
            catch (System.Exception e)
            {               

            }
            return currentLoggedInUser;

        }

    }
}