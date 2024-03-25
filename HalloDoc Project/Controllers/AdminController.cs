using DAL.ViewModels;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using DAL.DataContext;
using System.Text;
using BAL.Interfaces;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DocumentFormat.OpenXml.Wordprocessing;
using ClosedXML.Excel;

namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        private readonly IFileOperations _fileOperations;
        private readonly IEncounterForm _encounterForm;
        private readonly IAdmin _admin;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IEmailService emailService, IAdminTables adminTables, IAdminActions adminActions, IFileOperations fileOperations, IEncounterForm encounterForm, IAdmin admin)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _emailService = emailService;
            _adminActions = adminActions;
            _adminTables = adminTables;
            _fileOperations = fileOperations;
            _encounterForm = encounterForm;
            _admin = admin;
        }
        public IActionResult Index()
        {
            return View();
        }

        public enum RequestStatus
        {
            Unassigned = 1,
            Accepted = 2,
            Cancelled = 3,
            MDEnRoute = 4,
            MDOnSite = 5,
            Conclude = 6,
            CancelledByPatient = 7,
            Closed = 8,
            Unpaid = 9,
            Clear = 10,
            Block = 11,
        }

        public enum DashboardStatus
        {
            New = 1,
            Pending = 2,
            Active = 3,
            Conclude = 4,
            ToClose = 5,
            Unpaid = 6,
        }

        public enum RequestType
        {
            Business = 1,
            Patient = 2,
            Family = 3,
            Concierge = 4
        }

        public enum AllowRole
        {
            Admin = 1,
            Patient = 2,
            Physician = 3
        }
        public IActionResult Access()
        {
            return View();
        }
        //Delete, DeleteAll, ViewUploads, SendOrders(Get) methods are not converted to three tier.
        public IActionResult ViewCase(int requestid)
        {
            if (ModelState.IsValid)
            {
                ViewCaseViewModel vc = _adminActions.ViewCaseAction(requestid);
                return View(vc);
            }
            return View();
        }
        public ActionResult AssignCase()
        {
            return Ok();
        }
        public ActionResult BlockCase(String blockreason)
        {

            return Ok();
        }
        public IActionResult ViewNotes(int requestid)
        {
            ViewCaseViewModel vn = new ViewCaseViewModel();
            return View();
        }
        public ActionResult TransferNotes()
        {
            return Ok();
        }
        public IActionResult AdminDBView()
        {
            AdminDashboardViewModel advm = _adminTables.AdminDashboardView();
            return View(advm);
        }
        public static string GetDOB(Requestclient reqcli)
        {
            string dob = reqcli.Intyear + "-" + reqcli.Strmonth + "-" + reqcli.Intdate;
            if (reqcli.Intyear == null || reqcli.Strmonth == null || reqcli.Intdate == null)
            {
                return " ";
            }

            string dobdate = DateTime.Parse(dob).ToString("MMM dd, yyyy");

            return dobdate;
        }
        public IActionResult Partners()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ProviderLocation()
        {
            return View();
        }
        public IActionResult Providers()
        {
            return View();
        }
        public IActionResult Records()
        {
            return View();
        }
        public IActionResult AdminDashboard()
         {
            //var email = HttpContext.Session.GetString("Email");
            //AdminDashboardViewModel advm = _adminTables.AdminDashboard(email);
            //return View(advm,this.ExcelFile());
            var email = HttpContext.Session.GetString("Email");
            AdminDashboardViewModel advm = _adminTables.AdminDashboard(email);
            var excelData = this.ExcelFile();
            advm.excelfiles = excelData;
            return View(advm);
        }

        public ActionResult ExportToExcel()
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("EmployeeList");

            // Add headers
            var headers = new string[] { "Name", "PhoneNo", "Email", "Requestid", "Status", "Address", "RequestTypeId", "UserID" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
            }

            // Get employee data (assuming GetEmployeeList() returns a list of employees)
            var records = ExcelFile();
            for (int i = 0; i < records.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = records[i].Name;
                worksheet.Cell(i + 2, 2).Value = records[i].PhoneNo;
                worksheet.Cell(i + 2, 3).Value = records[i].email;
                worksheet.Cell(i + 2, 4).Value = records[i].requestid;
                worksheet.Cell(i + 2, 5).Value = records[i].status;
                worksheet.Cell(i + 2, 6).Value = records[i].address;
                worksheet.Cell(i + 2, 7).Value = records[i].requesttypeid;
                worksheet.Cell(i + 2, 8).Value = records[i].userid;
            }

            // Prepare the response
            MemoryStream stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            // Return the Excel file
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RequestDataExcel.xlsx");

        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, string AssignPhysician, string AssignDescription)
        {
            _adminActions.AssignCaseAction(RequestId, AssignPhysician, AssignDescription);
            return Ok();
        }
        [HttpPost]
        public ActionResult CancelCase(int requestid, string Reason, string Description)
        {
            _adminActions.CancelCaseAction(requestid, Reason, Description);
            return Ok();
        }
        [HttpPost]
        public IActionResult BlockCase(int requestid, string blocknotes)
        {
            _adminActions.BlockCaseAction(requestid, blocknotes);
            return Ok();
        }
        [HttpPost]
        public IActionResult TransferCase(int RequestId, string TransferPhysician, string TransferDescription)
        {
            var email = HttpContext.Session.GetString("Email");
            var admin = _context.Admins.FirstOrDefault(x => x.Email == email);
            _adminActions.TransferCase(RequestId, TransferPhysician, TransferDescription, admin.Adminid);
            return Ok();
        }
        [HttpPost]
        public bool ClearCaseModal(int requestid)
        {
            string AdminEmail = HttpContext.Session.GetString("Email");
            //Admin admin = _context.Admins.GetFirstOrDefault(a => a.Email == AdminEmail);
            return _adminActions.ClearCaseModal(requestid);
        }
        [HttpPost]
        public void SendLink(string FirstName, string LastName, string Email)
        {
            var AgreementLink = Url.Action("patient_submit_request_screen", "Guest", new { }, Request.Scheme);
            _emailService.SendEmailWithLink(FirstName, LastName, Email, AgreementLink);

        }
        public IActionResult CreateRequestAdminDashboard()
        {
            CreateRequestViewModel model = new CreateRequestViewModel()
            {
                regions = _context.Regions.ToList(),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult CreateRequestAdminDashboard(CreateRequestViewModel model)
        {
            var user = _context.Requests.FirstOrDefault(x => x.Email == model.email);
            if (user != null)
            {
                //var newvm=new PatientModel();
                Aspnetuser newUser = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                newUser.Id = id;
                newUser.Email = model.email;
                newUser.Phonenumber = model.phoneno;
                newUser.Username = model.firstname;
                newUser.Createddate = DateTime.Now;
                _context.Aspnetusers.Add(newUser);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = newUser.Id;
                user_obj.Firstname = model.firstname;
                user_obj.Lastname = model.lastname;
                user_obj.Email = model.email;
                user_obj.Mobile = model.phoneno;
                user_obj.Street = model.street;
                user_obj.City = model.city;
                user_obj.State = model.state;
                user_obj.Zipcode = model.zipcode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = model.firstname;
                request.Lastname = model.lastname;
                request.Phonenumber = model.phoneno;
                request.Email = model.email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = id;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = model.firstname;
                rc.Lastname = model.lastname;
                rc.Phonenumber = model.phoneno;
                rc.Location = model.city + model.state;
                rc.Email = model.email;
                rc.Address = model.street + " " + model.city + " " + model.state + " " + model.zipcode;
                rc.Street = model.street;
                rc.City = model.city;
                rc.State = model.state;
                rc.Zipcode = model.zipcode;
                rc.Notes = model.adminNotes;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();


            }
            else
            {
                User user_obj = _context.Users.FirstOrDefault(u => u.Email == model.email);
                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Firstname = model.firstname;
                request.Lastname = model.lastname;
                request.Phonenumber = model.phoneno;
                request.Email = model.email;
                request.Createddate = DateTime.Now;
                request.Status = 1;
                //request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = model.firstname;
                rc.Lastname = model.lastname;
                rc.Phonenumber = model.phoneno;
                rc.Location = model.city + " " + model.state;
                rc.Email = model.email;
                rc.Address = model.city + ", " + model.street + ", " + model.state + ", " + model.zipcode;
                rc.Street = model.street;
                rc.City = model.city;
                rc.State = model.state;
                rc.Zipcode = model.zipcode;
                rc.Notes = model.adminNotes;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();

            }
            return RedirectToAction("CreateRequestAdminDashboard");
        }
        [HttpPost]
        public IActionResult NewTable(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetNewTable(filter);
            model.currentPage = pageNumber;

            return PartialView("NewTable", model);
        }
        [HttpPost]
        public IActionResult ActiveTable(int page,int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetActiveTable(filter);
            return PartialView("ActiveTable", model);
        }
        [HttpPost]
        public IActionResult PendingTable(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetPendingTable(filter);
            model.currentPage = pageNumber;

            return PartialView("PendingTable", model);
        }
        [HttpPost]
        public IActionResult ConcludeTable(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetConcludeTable(filter);
            return PartialView("ConcludeTable", model);
        }
        [HttpPost]
        public IActionResult ToCloseTable(int page ,int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetToCloseTable(filter);
            return PartialView("ToCloseTable", model);
        }
        [HttpPost]
        public IActionResult UnpaidTable(int page, int region, int type, string search)
        {
            int pagesize = 5;
            int pageNumber = 1;
            if (page > 0)
            {
                pageNumber = page;
            }
            DashboardFilter filter = new DashboardFilter()
            {
                PatientSearchText = search,
                RegionFilter = region,
                RequestTypeFilter = type,
                pageNumber = pageNumber,
                pageSize = pagesize,
                page = page,
            };
            AdminDashboardViewModel model = _adminTables.GetUnpaidTable(filter);
            return PartialView("UnpaidTable", model);
        }
        public IActionResult EncounterForm(int requestId, EncounterFormViewModel EncModel)
        {
            EncModel = _encounterForm.EncounterFormGet(requestId);
            return View(EncModel);
        }
        [HttpPost]
        public IActionResult EncounterForm(EncounterFormViewModel model)
        {
            _encounterForm.EncounterFormPost(model.requestId, model);
            return EncounterForm(model.requestId, model);
        }
        public IActionResult AdminProfile()
        {
            var email = HttpContext.Session.GetString("Email");
            AdminProfileViewModel model = new AdminProfileViewModel();
            if (email != null)
            {
                model = _admin.AdminProfileGet(email);
            }
            return View("AdminProfile", model);
        }
        [HttpPost]
        public IActionResult AdminInfoPost(AdminProfileViewModel apvm)
        {
            _admin.AdminInfoPost(apvm);
            return AdminProfile();
        }
        [HttpPost]
        public IActionResult BillingInfoPost(AdminProfileViewModel apvm)
        {
            _admin.BillingInfoPost(apvm);
            return AdminProfile();
        }
        [HttpPost]
        public IActionResult PasswordPost(AdminProfileViewModel apvm)
        {
            var email = HttpContext.Session.GetString("Email");
            _admin.PasswordPost(apvm, email);
            return AdminProfile();
        }
        public IActionResult DeleteFile(int fileid, int requestid)
        {
            var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
            fileRequest.Isdeleted = true;

            _context.Update(fileRequest);
            _context.SaveChanges();

            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult DeleteAllFiles(int requestid)
        {
            var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            for (int i = 0; i < request.Count; i++)
            {
                request[i].Isdeleted = true;
                _context.Update(request[i]);
            }
            _context.SaveChanges();
            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult ViewUploads(int requestid)
        {

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
            var requests = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            ViewUploadsViewModel uploads = new()
            {
                ConfirmationNo = requests.Confirmationnumber,
                Patientname = user.Firstname + " " + user.Lastname,
                RequestID = requestid,
                Requestwisefiles = requestFile
            };
            return View(uploads);
        }
        public IActionResult SetPath(int requestid)
        {
            var path = _environment.WebRootPath;
            return SendMail(requestid, path);
        }
        public IActionResult SendMail(int requestid, string path)
        {
            _emailService.SendEmailWithAttachments(requestid, path);
            return RedirectToAction("ViewUploads", "Admin", new { requestid = requestid });
        }
        [HttpPost]
        public IActionResult SendAgreement(int RequestId, string PhoneNo, string email)
        {
            if (ModelState.IsValid)
            {
                var AgreementLink = Url.Action("ReviewAgreement", "Guest", new { ReqId = RequestId }, Request.Scheme);
                //----------------------------------
                _emailService.SendAgreementLink(RequestId, AgreementLink, email);
                return RedirectToAction("AdminDashboard", "Guest");
            }
            return View();
        }
        public IActionResult BusinessData(int BusinessId)
        {
            var result = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == BusinessId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewUploadsViewModel uploads)
        {
            if (uploads.File != null)
            {
                var uniqueid = Guid.NewGuid().ToString();
                var path = _environment.WebRootPath;
                _fileOperations.insertfilesunique(uploads.File, uniqueid, path);

                var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                var extensionstring = Path.GetExtension(uploads.File.FileName);
                Requestwisefile requestwisefile = new()
                {
                    Filename = uniqueid + "$" + uploads.File.FileName,
                    Requestid = uploads.RequestID,
                    Createddate = DateTime.Now
                };
                _context.Update(requestwisefile);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewUploads", new { requestid = uploads.RequestID });
        }
        public IActionResult CloseCase(int requestid)
        {
            CloseCaseViewModel model = _adminActions.CloseCaseGet(requestid);
            return View(model);
        }
        [HttpPost]
        public IActionResult CloseCase(CloseCaseViewModel model, int requestid)
        {
            _adminActions.CloseCasePost(model, requestid);
            return CloseCase(requestid);
        }
        public IActionResult CloseInstance(int reqid)
        {
            var user = _context.Requests.FirstOrDefault(x => x.Requestid == reqid);
            user.Status = 9;
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("AdminDashboard", "Admin");
        }
        public IActionResult SendOrders(int requestid)
        {
            List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.ToList();
            List<Healthprofessionaltype> healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();
            SendOrderViewModel model = new SendOrderViewModel()
            {
                requestid = requestid,
                healthprofessionals = healthprofessionals,
                healthprofessionaltype = healthprofessionaltypes
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult SendOrders(int requestid, SendOrderViewModel sendOrder)
        {
            _adminActions.SendOrderAction(requestid, sendOrder);
            return SendOrders(requestid);
        }
        public List<ExcelFileViewModel> ExcelFile()
        {
            var RequestData = (from r in _context.Requests
                               join rc in _context.Requestclients
                               on r.Requestid equals rc.Requestid
                               select new ExcelFileViewModel()
                               {
                                   Name = rc.Firstname + " " + rc.Lastname,
                                   PhoneNo = rc.Phonenumber,
                                   email = rc.Email,
                                   requestid = r.Requestid,
                                   status = r.Status,
                                   address = rc.Address,
                                   requesttypeid = r.Requesttypeid
                               }).ToList();
            return RequestData;
        }
        public List<Healthprofessional> filterVenByPro(string ProfessionId)
        {
            var result = _context.Healthprofessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            return result;
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login_page", "Guest");
        }

    }
}