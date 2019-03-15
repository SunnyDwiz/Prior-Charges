using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ServiceNowAppTool.Models
{
    public class InvoiceDownLoad
    {
        public string ID { get; set; }
        public string PharmacyCode { get; set; }

        public string FacilityCode { get; set; }
        public string CorpCode { get; set; }

        public string StatementDate { get; set; }

        public string FileName { get; set; }

        public String EndDate { get; set; }

        public byte[] BlobData { get; set; }

        public int isPdmi { get; set; }

        public string FfsOld { get; set; }

        public string Archived { get; set; }

        public List<Pharmacy> LstPharmacies { get; set; }

        public List<Pharmacy> LstCorporations { get; set; }
        public List<Pharmacy> LstFacilities { get; set; }
        
    }
}