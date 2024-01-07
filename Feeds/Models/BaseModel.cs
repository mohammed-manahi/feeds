namespace Feeds.Models;

public class BaseModel
{
    // Base model for managing created on and updated on datetime fields
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    
}