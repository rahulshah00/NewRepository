﻿using DAL.ViewModels;
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
        public string? phone { get; set; }
        public string relation {  get; set; }
        public IFormFile? file {  get; set; }
    }
}
