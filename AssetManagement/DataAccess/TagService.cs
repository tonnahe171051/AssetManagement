using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetManagement.DataAccess
{
    public class TagService
    {
        private readonly AssetUnityManagementContext _context;

        public TagService(AssetUnityManagementContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _context.Tags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task AddTagAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            var existingTag = await _context.Tags.FindAsync(tag.Id);
            if (existingTag != null)
            {
                _context.Entry(existingTag).CurrentValues.SetValues(tag);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteTagAsync(int id)
        {
            // Kiểm tra xem tag có đang được sử dụng không
            var hasFiles = await _context.Files
                .AnyAsync(f => f.TagId == id);

            if (hasFiles)
            {
                throw new InvalidOperationException("Cannot delete tag that is being used by files");
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

        // Phương thức bổ sung để kiểm tra tag tồn tại
        public async Task<bool> TagExistsAsync(string name, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Tags
                    .AnyAsync(t => t.Name.ToLower() == name.ToLower() && t.Id != excludeId.Value);
            }

            return await _context.Tags
                .AnyAsync(t => t.Name.ToLower() == name.ToLower());
        }
    }
}