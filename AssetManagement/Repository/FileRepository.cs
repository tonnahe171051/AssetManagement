using AssetManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly FileService _fileService;

        public FileRepository(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<List<File>> GetAllFilesAsync() =>
            await _fileService.GetAllFilesAsync();

        public async Task<List<File>> GetFilesByAssetIdAsync(int assetId) =>
            await _fileService.GetFilesByAssetIdAsync(assetId);

        public async Task<File> GetFileByIdAsync(int id) =>
            await _fileService.GetFileByIdAsync(id);

        public async Task<File> UploadFileAsync(string sourceFilePath, int assetId) =>
            await _fileService.UploadFileAsync(sourceFilePath, assetId);

        public async Task AddFileAsync(File file) =>
            await _fileService.AddFileAsync(file);

        public async Task UpdateFileAsync(File file) =>
            await _fileService.UpdateFileAsync(file);

        public async Task DeleteFileAsync(int id) =>
            await _fileService.DeleteFileAsync(id);

        public async Task<List<File>> GetFilesByNameAsync(int id, string searchTerm) =>
            await _fileService.GetFilesByNameAsync(id, searchTerm);
        
    }
}
