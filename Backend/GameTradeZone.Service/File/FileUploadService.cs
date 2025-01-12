using GameTradeZone.Service.Common.IServices;
using Microsoft.AspNetCore.Http;

namespace GameTradeZone.Service.File
{
    public class FileUploadService
    {
        private readonly IFtpDirectoryService _ftpDirectoryService;

        public FileUploadService(IFtpDirectoryService ftpDirectoryService)
        {
            _ftpDirectoryService = ftpDirectoryService;
        }

        public async Task<List<string>> UploadFiles(string folderName, int? id, List<IFormFile>? files)
        {
            var uploadedFilePaths = new List<string>();

            if (files == null || !id.HasValue)
            {
                return uploadedFilePaths;
            }

            var folder = $"public/{folderName}/{id}";

            foreach (var file in files)
            {
                var fileExt = Path.GetExtension(file.FileName);
                var stream = file.OpenReadStream();
                var fileName = $"{id}.{uploadedFilePaths.Count + 1}{fileExt}";

                var result = await _ftpDirectoryService.TransferToFtpDirectoryAsync(stream, folder, fileName);

                if (result.Succeed)
                {
                    uploadedFilePaths.Add($"{folder}/{fileName}");
                }
            }

            return uploadedFilePaths;
        }
    }
}
