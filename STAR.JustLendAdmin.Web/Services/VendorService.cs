#nullable enable
using Microsoft.Extensions.Logging;
using STAR.JustLendAdmin.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STAR.JustLendAdmin.Web.Services
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAsync(GetVendorRequest request);
        //Task<Vendor?> GetAsync(string id);
        Task<UpsertModelResponse<Vendor>> UpsertAsync(Vendor vendor);
    }

    public interface IDetailedVendorService
    {
        VendorType GetVendorType();
        Task<IEnumerable<Vendor>> GetAsync();
        Task<Vendor?> GetAsync(int id);
        Task<UpsertModelResponse<Vendor>> UpsertAsync(Vendor vendor);
    }

    public class VendorService : IVendorService
    {
        private ILogger<VendorService> log;
        private Dictionary<VendorType, IDetailedVendorService> services;

        public VendorService(ILogger<VendorService> log, IEnumerable<IDetailedVendorService> services)
        {
            this.log = log;
            this.services = services.ToDictionary(x => x.GetVendorType());
        }

        public async Task<IEnumerable<Vendor>> GetAsync(GetVendorRequest request)
        {
            if (services.ContainsKey(request.VendorType))
            {
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    return await services[request.VendorType].GetAsync();
                }
                else
                {
                    var vendors = new List<Vendor>();
                    var vendor = await services[request.VendorType].GetAsync(request.Id);
                    if (vendor != null)
                    {
                        vendors.Add(vendor);
                    }

                    return vendors;
                }
            }
            else
            {
                return new List<Vendor>();
            }
        }

        public async Task<UpsertModelResponse<Vendor>> UpsertAsync(Vendor vendor)
        {
            return await services[vendor.VendorType].UpsertAsync(vendor);
        }
    }

    public class FakeVendorService : IVendorService
    {
        private Vendor[] vendors = new[]
       {
            new Vendor{Id = "1", VendorType = VendorType.FourFiveZeroSix, General = new GeneralData{Name = "Fool Co"}},
            new Vendor{Id = "2", VendorType = VendorType.FourFiveZeroSix,General = new GeneralData{ Name = "Trash LLC"}},
        };


        public Task<IEnumerable<Vendor>> GetAsync(GetVendorRequest request)
        {
            var selected = from v in vendors
                           where v.VendorType == request.VendorType
                           select v;
            return Task.FromResult(selected);
        }

        public Task<Vendor?> GetAsync(string id)
        {
            return Task.FromResult<Vendor?>(vendors.FirstOrDefault(x => x.Id == id));
        }

        public Task<UpsertModelResponse<Vendor>> UpsertAsync(Vendor vendor)
        {
            if (string.IsNullOrEmpty(vendor.Id))
            {
                vendor.Id = Guid.NewGuid().ToString();
            }
            else
            {
                for (int i = 0; i < vendors.Length; i++)
                {
                    if (vendors[i].Id == vendor.Id)
                    {
                        vendors[i] = vendor;
                        break;
                    }
                }
            }
            var response = new UpsertModelResponse<Vendor> { Success = true, Model = vendor };
            return Task.FromResult(response);
        }
    }

    public class GetVendorRequest
    {
        public VendorType VendorType { get; set; } = VendorType.FourFiveZeroSix;
        public string? Id { get; set; }
    }
}
