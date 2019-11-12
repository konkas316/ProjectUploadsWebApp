using ProjectUploads.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectUploads.ViewModels
{
    public class UploadCreateViewModel
    {
                
        [Required, MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }
        //[Display(Name = "Office Email")]
        //[RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$",
        //    ErrorMessage = "Invalid email format")]
        //[Required]
        public string Description { get; set; }

        //[Required]
        //public Dept? Department { get; set; }        
        public IFormFile Project { get; set; }
        //public int Id { get; internal set; }

        public string FileExtention { get; set; }

        public long Size { get; set; }

        public string Icon { get; set; }
        public int Length { get; internal set; }
    }
}
