using System.Linq.Expressions;
using Feeds.Models;

namespace Feeds.Repositories;

public interface ICommentRepository
{
    public IEnumerable<Comment> GetAll(string? includeRelations = null);
    
    public void Add(Comment comment);
    
    public Comment Get(Expression<Func<Comment, bool>> filter, string? includeRelations = null);
    public void Remove(Comment comment);
}