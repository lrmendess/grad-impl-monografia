using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Extensions
{
    public static class ScapFileUtils
    {
        public static void DeleteFile(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        public static async Task<bool> UploadFile(IFormFile file, string fileName)
        {
            if (file.Length <= 0)
            {
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

            if (System.IO.File.Exists(path))
            {
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return true;
        }

        public static string GenerateFileName(IFormFile formFile)
        {
            return ScapFileUtils.SanitizeFileName($"{ Guid.NewGuid() }_{ formFile.FileName }");
        }

        private static string SanitizeFileName(string name)
        {
            var invalids = System.IO.Path.GetInvalidFileNameChars();

            var sanitize = String.Join("_", name.Split(invalids, StringSplitOptions.RemoveEmptyEntries));

            return sanitize.TrimEnd('.');
        }
    }
}
