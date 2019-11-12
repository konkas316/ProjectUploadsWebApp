using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ProjectUploads.Models
{
    public interface IUploadRepository
    {
        Upload GetUpload(int Id);
        IEnumerable<Upload> GetAllUploads();
        Upload Add(Upload upload);
        Upload Update(Upload uploadChanges);
        Upload Delete(int Id);
        
    }
}
