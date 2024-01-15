using System.Linq.Expressions;
using Feeds.Data;
using Feeds.Models;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IEnumerable<Comment> GetAll(string? includeRelations = null)
    {
        IQueryable<Comment> queryable = _dbContext.Set<Comment>();
        queryable = IncludeRelation(queryable, includeRelations);
        return queryable.ToList();
    }
    
    public Comment Get(Expression<Func<Comment, bool>> filter, string? includeRelations)
    {
        IQueryable<Comment> queryable = _dbContext.Set<Comment>();
        queryable = queryable.Where(filter);
        queryable = IncludeRelation(queryable, includeRelations);
        return queryable.FirstOrDefault();
    }
    
    public void Add(Comment comment)
    {
        _dbContext.Comments.Add(comment);
    }

    public void Remove(Comment comment)
    {
        _dbContext.Comments.Remove(comment);
    }
    
    private IQueryable<Comment> IncludeRelation(IQueryable<Comment> queryable, string? includeRelations = null)
    {
        if (!string.IsNullOrEmpty(includeRelations))
        {
            foreach (var includeRelation in includeRelations.Split(new char[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
            {
                queryable = queryable.Include(includeRelation);
            }
        }
        return queryable;
    }
}