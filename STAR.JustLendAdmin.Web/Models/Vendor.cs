#nullable enable
using System;
using System.ComponentModel;

namespace STAR.JustLendAdmin.Web.Models
{
    public class Vendor
    {
        public string Id { get; set; } = string.Empty;
        public VendorType VendorType { get; set; }
        public GeneralData General { get; set; } = new GeneralData();
        public VendorContact Contact { get; set; } = new VendorContact();
        public VendorSystemData SystemData { get; set; } = new VendorSystemData();
        public VendorServiceInformation ServiceInformation { get; set; } = new VendorServiceInformation();
    }

    public class VendorViewModel
    {
        public Vendor Vendor { get; set; } = new Vendor();
        public ResponseCore Response { get; set; } = new ResponseCore();
    }
}
