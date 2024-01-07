namespace Feeds.Repositories;

public interface IUnitOfWork
{
    public IPostRepository PostRepository { get; set; }

    public void Save();
}