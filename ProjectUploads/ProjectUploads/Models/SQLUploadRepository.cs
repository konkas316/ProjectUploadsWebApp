using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectUploads.Models
{
    public class SQLUploadRepository : IUploadRepository
    {
        private readonly AppDbContext context;
        private readonly ILogger<SQLUploadRepository> logger;

        public SQLUploadRepository(AppDbContext context,
            ILogger<SQLUploadRepository> logger)
        {
            this.context = context;
            this.logger = logger;
        }
        public Upload Add(Upload upload)
        {
            context.Uploads.Add(upload);
            context.SaveChanges();
            return upload;
        }

        public Upload Delete(int Id)
        {
            Upload upload = context.Uploads.Find(Id);
            if (upload != null)
            {
                context.Uploads.Remove(upload);
                context.SaveChanges();
            }
            return upload;
        }

        public IEnumerable<Upload> GetAllUploads()
        {
            return context.Uploads;
        }

        public Upload GetUpload(int Id)
        {
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning Log");
            logger.LogError("Error Log");
            logger.LogCritical("Critical Log");

            return context.Uploads.Find(Id);
        }

        public Upload Update(Upload uploadChanges)
        {
            var upload = context.Uploads.Attach(uploadChanges);
            upload.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return uploadChanges;
        }
    }
}
