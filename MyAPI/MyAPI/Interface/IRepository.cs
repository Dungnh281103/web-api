﻿using MyAPI.Dtos.Rating;
using System.Linq.Expressions;

namespace MyAPI.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(string id);
        //Task UpdateViewsAsync(string chapterId);
        Task SaveChangesAsync();
        Task<RatingResponseDto> UpdateRatingAsync(string storyId, Guid userId, double score, string review);
    }
}
