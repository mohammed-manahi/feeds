using System.Linq.Expressions;
using Feeds.Models;

namespace Feeds.Repositories;

public interface IPostRepository
{
    public IEnumerable<Post> GetAll(string? includeRelations = null);

    public Post Get(Expression<Func<Post, bool>> filter, string? includeRelations = null);

    public void Add(Post post);

    public void Update(Post post);

    public void Remove(Post post);

    public void RemoveAll(IEnumerable<Post> posts);
}