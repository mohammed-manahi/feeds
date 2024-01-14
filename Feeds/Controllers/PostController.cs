using System.Diagnostics;
using System.Security.Claims;
using Feeds.Data;
using Feeds.Models;
using Feeds.Repositories;
using Feeds.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Controllers;

public class PostController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _webHostEnvironment = webHostEnvironment;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    [AllowAnonymous]
    [Route("posts/")]
    public IActionResult Index()
    {
        var postList = _unitOfWork.PostRepository.GetAll("ApplicationUser");
        return View(postList.OrderBy(p => p.Title));
    }

    [AllowAnonymous]
    [Route("posts/{year:int}/{month:int}/{day:int}/{slug}")]
    public IActionResult Get(int? year, int? month, int? day, string? slug)
    {
        if (slug == null)
        {
            return NotFound();
        }

        Post post = _unitOfWork.PostRepository.Get(p =>
            p.Slug == slug && p.CreatedOn.Year == year && p.CreatedOn.Month == month && p.CreatedOn.Day == day, "ApplicationUser");
        return View(post);
    }

    [Authorize]
    [HttpGet]
    [Route("posts/create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [Route("posts/create")]
    public IActionResult Create(Post post)
    {
        IFormFile file = Request.Form.Files.FirstOrDefault();
        var targetPath = @"images/posts";
        FileManagementUtility fileManagementUtility = new FileManagementUtility(_webHostEnvironment);
        
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (ModelState.IsValid)
        {
            if (_dbContext.Posts.Any(p =>
                    p.Slug == post.Slug && p.CreatedOn.Year == DateTime.Now.Year &&
                    p.CreatedOn.Month == DateTime.Now.Month && p.CreatedOn.Day == DateTime.Now.Day))
            {
                ModelState.AddModelError("", "Post title can not be duplicate");
                return View();
            }
            post.ApplicationUserId = userId;
            post.Image = fileManagementUtility.UploadFile(file, targetPath);
            _unitOfWork.PostRepository.Add(post);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        return View();
    }

    [Authorize]
    [Route("posts/{year:int}/{month:int}/{day:int}/{slug}/edit")]
    public IActionResult Edit(int? year, int? month, int? day, string? slug)
    {
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (slug == null) return NotFound();
        Post post = _unitOfWork.PostRepository.Get(p =>
            p.Slug == slug && p.CreatedOn.Year == year && p.CreatedOn.Month == month && p.CreatedOn.Day == day);
        if (post.ApplicationUserId == userId)
        {
            return View(post);
        }
        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    [HttpPost]
    [Route("posts/{year:int}/{month:int}/{day:int}/{slug}/edit")]
    public IActionResult Edit(Post post)
    {
        if (ModelState.IsValid)
        {
            FileManagementUtility fileManagementUtility = new FileManagementUtility(_webHostEnvironment);
            IFormFile file = Request.Form.Files.FirstOrDefault();
            if (Request.Form.Files.FirstOrDefault() != null && !string.IsNullOrEmpty(post.Image))
            {
                fileManagementUtility.RemoveFile(post.Image);
                var targetPath = @"images/posts";
                post.Image = fileManagementUtility.UploadFile(file, targetPath);
            }

            post.ApplicationUserId = post.ApplicationUserId;
            post.Image = post.Image;
            _unitOfWork.PostRepository.Update(post);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Get),
                new
                {
                    slug = post.Slug, year = post.CreatedOn.Year, month = post.CreatedOn.Month, day = post.CreatedOn.Day
                });
        }

        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Post post = _unitOfWork.PostRepository.Get(p => p.Id == id);
        var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (post.ApplicationUserId == userId)
        {
            _unitOfWork.PostRepository.Remove(post);
            _unitOfWork.Save();
        }
        return RedirectToAction(nameof(Index));
    }
}