using AssetManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByIdAsync(int id);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(int id);
        Task<bool> TagExistsAsync(string name, int? excludeId = null);
    }
}
