using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectUploads.ViewModels
{
    
        public class UploadEditViewModel : UploadCreateViewModel
        {
            public new int Id { get; set; }
            public string ExistingUploadPath { get; set; }
        }
    
}
