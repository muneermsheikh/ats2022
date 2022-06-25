using System.Linq;
using AutoMapper;
using core.ParamsAndDtos;
using infra.Data;

namespace api.Helpers
{
     public class CategoryNameFromCategoryIdResolver : IValueResolver<int, CVRefAndDeployDto, string>
     {
          private readonly ATSContext _context;
          public CategoryNameFromCategoryIdResolver(ATSContext context)
          {
               _context = context;
          }

          public string Resolve(int source, CVRefAndDeployDto destination, string destMember, ResolutionContext context)
          {
               var categoryId = _context.OrderItems.Where(x => x.Id == source).Select(x => x.CategoryId).FirstOrDefault();
               return _context.Categories.Where(x => x.Id == categoryId).Select(x => x.Name).FirstOrDefault();
          }
     }
}