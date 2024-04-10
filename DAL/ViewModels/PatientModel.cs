using DAL.DataModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DAL.ViewModels
{
    public class PatientModel
    {
        
        public string? Symptoms { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; } 
        [Required]
        //[RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Entered phone format is not valid.")]
        //[DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }=string.Empty;
   
        public string? Street  { get; set; }

        public string? City { get; set; }

        public int? State { get; set; }
   
        public string? ZipCode { get; set; }
        
        public string CountryCode {  get; set; }=string.Empty;  

        public string? RoomSuite { get; set; }

        public IFormFile? File{ get; set; }
        public string? Password { get; set; }
        //[Compare("Password", ErrorMessage = "Password and Confirm-Password should have same value.")]
        public string? ConfirmPass{ get; set; }
        public List<Region>? Regions { get; set; }
        
    }
}
