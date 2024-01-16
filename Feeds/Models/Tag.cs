using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Feeds.Models;

public class Tag
{
    [Key]
    public int Id { get; set; }
    
    public string TagName { get; set; }
    
    
    [ValidateNever]
    public ICollection<PostTag> PostTags { get; set; }
}