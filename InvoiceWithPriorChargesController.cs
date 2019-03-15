using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ServiceNowAppTool.Models;
using ServiceNowAppTool.Common;

namespace ServiceNowAppTool.Controllers
{
    public class InvoiceWithPriorChargesController : Controller
    {
        //DbCommon have  List<Pharmacy> GetList(string request, string app, String[] param = null) to fetch pharmacyList
        DBCommon db = new DBCommon();
        [HttpGet]
        public ActionResult PriorCharge()
        {   //Model Class 'InvoiceDownLoad' contains LstPharmacies property.
            InvoiceDownLoad IncPriorDownload = new InvoiceDownLoad();
            IncPriorDownload.LstPharmacies = db.GetList("Pharmacies", "Vmrx",null);
            return View(IncPriorDownload);
        }
        [HttpPost]
        public ActionResult PriorCharge(FormCollection form)
        {
            return View();
        }
    }
}