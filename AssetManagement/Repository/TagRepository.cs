using AssetManagement.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManagement.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly TagService _tagService;

        public TagRepository(TagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<List<Tag>> GetAllTagsAsync() =>
            await _tagService.GetAllTagsAsync();

        public async Task<Tag> GetTagByIdAsync(int id) =>
            await _tagService.GetTagByIdAsync(id);

        public async Task AddTagAsync(Tag tag) =>
            await _tagService.AddTagAsync(tag);

        public async Task UpdateTagAsync(Tag tag) =>
            await _tagService.UpdateTagAsync(tag);

        public async Task DeleteTagAsync(int id) =>
            await _tagService.DeleteTagAsync(id);

        public async Task<bool> TagExistsAsync(string name, int? excludeId = null) =>
            await _tagService.TagExistsAsync(name, excludeId);
    }
}
