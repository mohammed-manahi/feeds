using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Feeds.Models;

public class Comment : BaseModel
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Body { get; set; }
    public int PostId { get; set; }
    
    [ValidateNever]
    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; }
    
    public string? ApplicationUserId { get; set; }
    
    [ValidateNever]
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }
}