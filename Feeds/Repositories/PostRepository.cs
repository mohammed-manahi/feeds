using System.Linq.Expressions;
using Feeds.Data;
using Feeds.Models;
using Microsoft.EntityFrameworkCore;

namespace Feeds.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Post> GetAll(string? includeRelations)
    {
        // Get all posts with optional relation inclusion 
        IQueryable<Post> queryable = _dbContext.Set<Post>();
        queryable = IncludeRelation(queryable, includeRelations);
        return queryable.ToList();
    }

    public Post Get(Expression<Func<Post, bool>> filter, string? includeRelations)
    {
        IQueryable<Post> queryable = _dbContext.Set<Post>();
        queryable = queryable.Where(filter);
        queryable = IncludeRelation(queryable, includeRelations);
        return queryable.FirstOrDefault();
    }

    public void Add(Post post)
    {
        _dbContext.Posts.Add(post);
    }

    public void Update(Post post)
    {
        _dbContext.Posts.Update(post);
    }

    public void Remove(Post post)
    {
        _dbContext.Posts.Remove(post);
    }

    public void RemoveAll(IEnumerable<Post> posts)
    {
        _dbContext.Posts.RemoveRange(posts);
    }

    private IQueryable<Post> IncludeRelation(IQueryable<Post> queryable, string? includeRelations = null)
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