using Microsoft.EntityFrameworkCore;
using MyAPI.Db;
using MyAPI.Dtos.Rating;
using MyAPI.Interface;
using System.Linq.Expressions;

namespace MyAPI.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly MyDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MyDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual Task UpdateAsync(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

       
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        

        public Task<RatingResponseDto> UpdateRatingAsync(string storyId, Guid userId, double score, string review)
        {
            throw new NotImplementedException();
        }
    }
}
