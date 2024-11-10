using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.DataAccess
{
    public class FileService
    {
        private readonly AssetUnityManagementContext _context;
        private readonly string _uploadDirectory;

        public FileService(AssetUnityManagementContext context)
        {
            _context = context;
            _uploadDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Uploads"
            );
            Directory.CreateDirectory(_uploadDirectory);
        }

        public async Task<List<File>> GetAllFilesAsync()
        {
            return await _context.Files
                .Include(f => f.Asset)
                .Include(f => f.Tag)
                .ToListAsync();
        }

        public async Task<List<File>> GetFilesByAssetIdAsync(int assetId)
        {
            return await _context.Files
                .Include(f => f.Tag)
                .Where(f => f.AssetId == assetId)
                .ToListAsync();
        }
        public async Task<List<File>> GetFilesByNameAsync(int assetId, string searchTerm)
        {
            return await _context.Files
                .Include(f => f.Tag)
                .Where(f => f.AssetId == assetId &&
                            (string.IsNullOrEmpty(searchTerm) ||
                             f.Name.ToLower().Contains(searchTerm.ToLower())))
                .ToListAsync();
        }

        public async Task<File> GetFileByIdAsync(int id)
        {
            return await _context.Files
                .Include(f => f.Asset)
                .Include(f => f.Tag)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<File> UploadFileAsync(string sourceFilePath, int assetId)
        {
            var fileName = Path.GetFileName(sourceFilePath);
            var destinationPath = Path.Combine(_uploadDirectory, fileName);

            // Handle duplicate filenames
            int counter = 1;
            while (System.IO.File.Exists(destinationPath))
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var fileExt = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExt}({counter}){fileExt}";
                destinationPath = Path.Combine(_uploadDirectory, fileName);
                counter++;
            }

            // Copy file
            await Task.Run(() => System.IO.File.Copy(sourceFilePath, destinationPath));

            // Create file record
            var file = new File
            {
                Name = fileName,
                Path = destinationPath,
                UploadDate = DateTime.Now,
                AssetId = assetId
            };
            return file;
        }

        public async Task AddFileAsync(File file)
        {
            _context.Files.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileAsync(File file)
        {
            _context.Entry(file).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file != null)
            {
                _context.Files.Remove(file);
                await _context.SaveChangesAsync();
            }
        }
    }
}
