﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Slugify;

namespace Feeds.Models;


public class Post : BaseModel
{
    private string _slug;

    [Key]
    [Required]
    public int Id { get; set; }
    
    [Required] 
    [Length(5, 150)] 
    public string Title { get; set; }

    [Required]
    public string Body { get; set; }

    public string Slug
    {
        get => _slug;
        set => _slug = Slugify(Title);
    }
    
    public string? Image { get; set; }

    public string Slugify(string title)
    {
        SlugHelperConfiguration config = new SlugHelperConfiguration();
        config.ForceLowerCase = true;
        SlugHelper helper = new SlugHelper(config);
        return helper.GenerateSlug(title);
    }
}