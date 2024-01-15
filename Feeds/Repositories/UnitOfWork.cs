using Feeds.Data;

namespace Feeds.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public IPostRepository PostRepository { get; set; }
    
    public ICommentRepository CommentRepository { get; set; }
    
    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        PostRepository = new PostRepository(dbContext);
        CommentRepository = new CommentRepository(dbContext);
    }
    
    public void Save()
    {
        _dbContext.SaveChanges();
    }
}