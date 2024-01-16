using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Feeds.Models;

public class PostTag
{
    [Key]
    public int Id { get; set; }
    
    public int PostId { get; set; }
    
    [ForeignKey(nameof(PostId))]
    [ValidateNever]
    public Post Post { get; set; }
    
    public int TagId { get; set; }
    
    [ForeignKey(nameof(TagId))]
    [ValidateNever]
    public Tag Tag { get; set; }
    
    
}