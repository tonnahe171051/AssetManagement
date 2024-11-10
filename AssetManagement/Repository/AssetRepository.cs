using AssetManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public class AssetRepository : IAssetRepository
    {
        private readonly AssetService _assetService;

        public AssetRepository(AssetService assetService)
        {
            _assetService = assetService;
        }

        public async Task<List<Asset>> GetAllAssetsAsync() =>
            await _assetService.GetAllAssetsAsync();

        public async Task<Asset> GetAssetByIdAsync(int id) =>
            await _assetService.GetAssetByIdAsync(id);

        public async Task AddAssetAsync(Asset asset) =>
            await _assetService.AddAssetAsync(asset);

        public async Task UpdateAssetAsync(Asset asset) =>
            await _assetService.UpdateAssetAsync(asset);

        public async Task DeleteAssetAsync(int id) =>
            await _assetService.DeleteAssetAsync(id);
    }
}
