using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.DataAccess
{
    public class AssetService
    {
        private readonly AssetUnityManagementContext _context;

        public AssetService(AssetUnityManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Asset>> GetAllAssetsAsync()
        {
            return await _context.Assets
                .Include(a => a.Files)
                .ToListAsync();
        }

        public async Task<Asset> GetAssetByIdAsync(int id)
        {
            return await _context.Assets
                .Include(a => a.Files)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task AddAssetAsync(Asset asset)
        {
            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAssetAsync(Asset asset)
        {
            var _asset = await _context.Assets.FindAsync(asset.Id);
            if (_asset != null)
            {
                _context.Assets.Update(asset);
                await _context.SaveChangesAsync();
            }

        }

        public async Task DeleteAssetAsync(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
        }

    }
}
