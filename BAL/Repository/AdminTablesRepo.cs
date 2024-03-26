using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
    public class AdminTablesRepo : IAdminTables
    {
        private readonly ApplicationDbContext _context;
        public AdminTablesRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public AdminDashboardViewModel AdminDashboardView()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     email = rc.Email
                                 }).ToList();

            AdminRequestsViewModel arvm = new AdminRequestsViewModel();
            AdminDashboardViewModel advm = new()
            {
                adminRequests = adminRequests,
                Username = arvm.Name,
            };
            return advm;
        }

        public AdminDashboardViewModel GetActiveTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 4 || x.status == 5).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetConcludeTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email
                                 }
                               ).Where(x => x.status == 6).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetNewTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status
                                 }).Where(x => x.status == 1).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
                TotalPage = (int)Math.Ceiling(adminRequests.Count() / (double)pagesize)
            };
            return model;
        }
        public AdminDashboardViewModel GetPendingTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email
                                 }).Where(x => x.status == 2).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetToCloseTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email
                                 }).Where(x => x.status == 3 || x.status == 7 || x.status == 8).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();

            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }
        public AdminDashboardViewModel GetUnpaidTable(DashboardFilter filter)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (filter.page > 0)
            {
                pageNumber = filter.page;
            }
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 where (filter.RequestTypeFilter == 0 || r.Requesttypeid == filter.RequestTypeFilter)
                                 && (filter.RegionFilter == 0 || rc.Regionid == filter.RegionFilter)
                                 && (string.IsNullOrEmpty(filter.PatientSearchText) || (rc.Firstname + " " + rc.Lastname).ToLower().Contains(filter.PatientSearchText.ToLower()))
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022"),
                                     email = rc.Email
                                 }).Where(x => x.status == 9).Skip((pageNumber - 1) * pagesize).Take(pagesize).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return model;
        }

        AdminDashboardViewModel IAdminTables.AdminDashboard(string email)
        {
            List<Physician> physician = _context.Physicians.ToList();
            List<Region> regions = _context.Regions.ToList();
            List<Casetag> casetags = _context.Casetags.ToList();

            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

            AdminDashboardViewModel advm = new()
            {
                physician = physician,
                regions = regions,
                casetags = casetags,
                New = _context.Requests.Count(u => u.Status == 1),
                active = _context.Requests.Count(u => u.Status == 4 || u.Status == 5),
                pending = _context.Requests.Count(u => u.Status == 2),
                conclude = _context.Requests.Count(u => u.Status == 6),
                toclose = _context.Requests.Count(u => u.Status == 7 || u.Status == 3 || u.Status == 8),
                unpaid = _context.Requests.Count(u => u.Status == 9),
                Username = admin.Firstname + " " + admin.Lastname
            };
            return advm;
        }
    }
}

