using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public interface IFileRepository
    {
        Task<List<DataAccess.File>> GetAllFilesAsync();
        Task<List<DataAccess.File>> GetFilesByAssetIdAsync(int assetId);
        Task<DataAccess.File> GetFileByIdAsync(int id);
        Task<List<DataAccess.File>> GetFilesByNameAsync(int id, string searchTerm);
        Task<DataAccess.File> UploadFileAsync(string sourceFilePath, int assetId);
        Task AddFileAsync(DataAccess.File file);
        Task UpdateFileAsync(DataAccess.File file);
        Task DeleteFileAsync(int id);
    }
}
