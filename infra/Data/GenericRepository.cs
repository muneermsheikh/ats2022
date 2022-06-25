using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities;
using core.Interfaces;
using core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace infra.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
     {
          private readonly ATSContext _context;
          public GenericRepository(ATSContext context)
          {
               _context = context;
          }
          public void Add(T entity)
          {
               _context.Set<T>().Add(entity);
          }

          public async Task<int> CountAsync(ISpecification<T> spec)
          {
               return await ApplySpecification(spec).CountAsync();
          }

          public void Delete(T entity)
          {
               _context.Set<T>().Remove(entity);
          }

          public async Task<T> GetByIdAsync(int Id)
          {
               return await _context.Set<T>().FindAsync(Id);
          }

          public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
          {
               return await ApplySpecification(spec).FirstOrDefaultAsync();
          }

          public async Task<IReadOnlyList<T>> ListAllAsync()
          {
               return await _context.Set<T>().ToListAsync();
          }

          public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
          {
               return await ApplySpecification(spec).ToListAsync();
          }

          public void Update(T entity)
          {
               _context.Set<T>().Attach(entity);
               _context.Entry(entity).State = EntityState.Modified;
          }

          private IQueryable<T> ApplySpecification(ISpecification<T> spec)
          {
               return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
          }
     }
}