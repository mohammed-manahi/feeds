using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Validations;

public class ImageFileValidation : ValidationAttribute
{
    private string[] _allowedExtensions = new string[] { ".jpg", ".png", ".gif" };
    private int _maxFileSize = 1024 * 1024 * 5; // 5 MB

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        string filename = value as string;
        if (filename == null)
        {
            return new ValidationResult("Please select a file.");
        }

        if (!_allowedExtensions.Contains(Path.GetExtension(filename).ToLower()))
        {
            return new ValidationResult($"Invalid file type. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        // Access the IFormFile object from model binding context
        var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
        var formFile = httpContextAccessor.HttpContext.Request.Form.Files[0];
        if (formFile.Length > _maxFileSize)
        {
            return new ValidationResult($"File size exceeds maximum of {_maxFileSize / 1024} KB.");
        }

        return ValidationResult.Success;
    }
}