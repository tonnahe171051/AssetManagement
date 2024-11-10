using AssetManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public interface IAssetRepository
    {
        Task<List<Asset>> GetAllAssetsAsync();
        Task<Asset> GetAssetByIdAsync(int id);
        Task AddAssetAsync(Asset asset);
        Task UpdateAssetAsync(Asset asset);
        Task DeleteAssetAsync(int id);
    }
}
