using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class FamilyFriendModel
    {
        public PatientModel PatientModel { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string? FriendFamilyPhoneNo { get; set; }
        public string relation {  get; set; }
        public List<Region>? PatientRegions { get; set; }
        public string? FriendFamilyCountryCode {  get; set; }
    }
}
