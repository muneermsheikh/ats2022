using System;
using System.Linq;
using System.Threading.Tasks;
using infra.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class CategoryExtensions
    {
        public static async Task<string> CategoryNameFromId(this ATSContext context, int id)
        {
            return await context.Categories.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
        }
        public static async Task<string> IndustryNameFromId(this ATSContext context, int id)
        {
            return await context.Industries.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefaultAsync();
        }

        public static T SafeCast<T>(this Nullable<T> t, T defaultValue = default(T)) where T : struct
        {
            return t.HasValue ? t.Value : defaultValue;
        }

       
        
    }
}