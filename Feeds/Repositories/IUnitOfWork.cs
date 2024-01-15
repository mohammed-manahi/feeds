namespace Feeds.Repositories;

public interface IUnitOfWork
{
    public IPostRepository PostRepository { get; set; }
    
    public ICommentRepository CommentRepository { get; set; }

    public void Save();
}