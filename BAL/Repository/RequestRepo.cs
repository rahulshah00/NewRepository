﻿using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace BAL.Repository
{
    public class RequestRepo : IRequestRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IFileOperations _insertfiles;
        public RequestRepo(ApplicationDbContext context, IPasswordHasher passwordHasher, IFileOperations insertfiles)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _insertfiles = insertfiles;
        }
        
        public void FRequest(FamilyFriendModel fmfr,string uniqueid,string path)
        {
            Request r = new()
            {
                Requesttypeid = 3,
                Firstname = fmfr.firstName,
                Lastname = fmfr.lastName,
                Phonenumber = fmfr.phone,
                Email = fmfr.email,
                Status = 1,
                Createddate = DateTime.Now
            };
            _context.Requests.Add(r);
            _context.SaveChanges();
            Requestclient rcl = new()
            {
                Requestid = r.Requestid,
                Firstname = fmfr.PatientModel.FirstName,
                Lastname = fmfr.PatientModel.LastName,
                Phonenumber = fmfr.PatientModel.PhoneNo,
                Email = fmfr.PatientModel.Email,
                Location = fmfr.PatientModel.City + fmfr.PatientModel.State,
                City = fmfr.PatientModel.City,
                State = fmfr.PatientModel.State,
                Zipcode = fmfr.PatientModel.ZipCode

            };
            _context.Requestclients.Add(rcl);
            _context.SaveChanges();
            if (fmfr.file!= null)
            {
                _insertfiles.insertfilesunique(fmfr.file, uniqueid, path);
                var filestring = Path.GetFileNameWithoutExtension(fmfr.file.FileName);
                var extensionstring = Path.GetExtension(fmfr.file.FileName);
                Requestwisefile rwf = new()
                {
                    Requestid = r.Requestid,
                    Filename = uniqueid + "$" + fmfr.file.FileName,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(rwf);
                _context.SaveChanges();
            }
        }
        public void CRequest(ConciergeModel cm)
        {
            Concierge c = new()
            {
                Conciergename = cm.ConFirstName + cm.ConLastName,
                Street = cm.ConStreet,
                City = cm.ConCity,
                State = cm.ConState,
                Zipcode = cm.ConZipCode,
                Createddate = DateTime.Now,

            };
            _context.Concierges.Add(c);
            _context.SaveChanges();
            Request req = new()
            {
                Requesttypeid = 4,
                Firstname = cm.ConFirstName,
                Lastname = cm.ConLastName,
                Phonenumber = cm.ConPhoneNo,
                Email = cm.ConEmail,
                Status = 1,
                Createddate = DateTime.Now,

            };
            _context.Requests.Add(req);
            _context.SaveChanges();
            Requestconcierge rc = new()
            {
                Requestid = req.Requestid,
                Conciergeid = c.Conciergeid
            };
            _context.Requestconcierges.Add(rc);
            _context.SaveChanges();
            Requestclient rcl = new()
            {
                Requestid = req.Requestid,
                Firstname = cm.PtFirstName,
                Lastname = cm.PtLastName,
                Phonenumber = cm.PtPhoneNo,
                Email = cm.PtEmail,
                Street = cm.ConStreet,
                City = cm.ConCity,
                State = cm.ConState,
                Zipcode = cm.ConZipCode
            };
            _context.Requestclients.Add(rcl);
            _context.SaveChanges();
        }
        public void PRequest(PatientModel pm, string uniqueid,string _path)
        {
            if (pm.Password != null)
            {
                //var newvm=new PatientModel();
                Aspnetuser user = new Aspnetuser();

                string id = Guid.NewGuid().ToString();
                user.Id = id;
                user.Email = pm.Email;
                user.Passwordhash = _passwordHasher.GenerateSHA256(pm.Password);
                user.Phonenumber = pm.PhoneNo;
                user.Username = pm.FirstName;
                user.Createddate = DateTime.Now;
                _context.Aspnetusers.Add(user);
                _context.SaveChanges();

                User user_obj = new User();
                user_obj.Aspnetuserid = user.Id;
                user_obj.Firstname = pm.FirstName;
                user_obj.Lastname = pm.LastName;
                user_obj.Email = pm.Email;
                user_obj.Mobile = pm.PhoneNo;
                user_obj.Street = pm.Street;
                user_obj.City = pm.City;
                user_obj.State = pm.State;
                user_obj.Zipcode = pm.ZipCode;
                user_obj.Createddate = DateTime.Now;
                user_obj.Createdby = id;
                _context.Users.Add(user_obj);
                _context.SaveChanges();

                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = pm.FirstName;
                request.Lastname = pm.LastName;
                request.Phonenumber = pm.PhoneNo;
                request.Email = pm.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = id;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = pm.FirstName;
                rc.Lastname = pm.LastName;
                rc.Phonenumber = pm.PhoneNo;
                rc.Location = pm.City + pm.State;
                rc.Email = pm.Email;
                rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                rc.Street = pm.Street;
                rc.City = pm.City;
                rc.State = pm.State;
                rc.Zipcode = pm.ZipCode;
                rc.Notes = pm.Symptoms;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();

                if (pm.File != null)
                {
                    //Guid myuuid = Guid.NewGuid();
                    //var filename = Path.GetFileName(model.File.FileName);
                    //var FinalFileName = $"{myuuid.ToString()}*{filename}";

                    _insertfiles.insertfilesunique(pm.File, uniqueid, _path);
                    var filestring = Path.GetFileNameWithoutExtension(pm.File.FileName);
                    var extensionstring = Path.GetExtension(pm.File.FileName);
                    Requestwisefile rwf = new()
                    {
                        Requestid = request.Requestid,
                        Filename = uniqueid + "$" + pm.File.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(rwf);
                    _context.SaveChanges();
                }
            }
            else
            {
                User user_obj = _context.Users.FirstOrDefault(u => u.Email == pm.Email);
                Request request = new Request();
                //change the fname, lname , and contact detials acc to the requestor
                request.Requesttypeid = 2;
                request.Userid = user_obj.Userid;
                request.Firstname = pm.FirstName;
                request.Lastname = pm.LastName;
                request.Phonenumber = pm.PhoneNo;
                request.Email = pm.Email;
                request.Createddate = DateTime.Now;
                request.Patientaccountid = user_obj.Aspnetuserid;
                request.Status = 1;
                request.Createduserid = user_obj.Userid;
                _context.Requests.Add(request);
                _context.SaveChanges();

                Requestclient rc = new Requestclient();
                rc.Requestid = request.Requestid;
                rc.Firstname = pm.FirstName;
                rc.Lastname = pm.LastName;
                rc.Phonenumber = pm.PhoneNo;
                rc.Location = pm.City + pm.State;
                rc.Email = pm.Email;
                rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                rc.Street = pm.Street;
                rc.City = pm.City;
                rc.State = pm.State;
                rc.Zipcode = pm.ZipCode;
                rc.Notes = pm.Symptoms;

                _context.Requestclients.Add(rc);
                _context.SaveChanges();
                if (pm.File != null)
                {

                    _insertfiles.insertfiles(pm.File, _path);
                    Requestwisefile rwf = new()
                    {
                        Requestid = request.Requestid,
                        Filename = pm.File.FileName,
                        Createddate = DateTime.Now,
                    };
                    _context.Requestwisefiles.Add(rwf);
                    _context.SaveChanges();
                }
            }
        }
        public void BRequest(BusinessModel bm)
        {
            Business bus = new()
            {
                Name = bm.BusinessName,
                Phonenumber = bm.BsPhoneNo,
                Createddate = DateTime.Now
            };
            _context.Businesses.Add(bus);
            _context.SaveChanges();

            Request req = new()
            {
                Requesttypeid = 1,
                Firstname = bm.BsFirstName,
                Lastname = bm.BsLastName,
                Phonenumber = bm.BsPhoneNo,
                Email = bm.BsEmail,
                Status = 1,
                Createddate = DateTime.Now,
            };
            _context.Requests.Add(req);
            _context.SaveChanges();
            Requestbusiness ReqBus = new()
            {
                Requestid = req.Requestid,
                Businessid = bus.Id,
            };

            _context.Requestbusinesses.Add(ReqBus);
            _context.SaveChanges();

            Requestclient rc = new()
            {
                Requestid = req.Requestid,
                Firstname = bm.PtFirstName,
                Lastname = bm.BsLastName,
                Phonenumber = bm.BsPhoneNo,
                Street = bm.Street,
                City = bm.city,
                State = bm.state,
                Zipcode = bm.zipcode
            };
            _context.Requestclients.Add(rc);
            _context.SaveChanges();
        }
    }
}