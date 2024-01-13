using Feeds.Data;
using Feeds.Models;
using Feeds.Repositories;
using Feeds.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Controllers;

public class PostController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IUnitOfWork _unitOfWork;

    public PostController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork,
        IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _webHostEnvironment = webHostEnvironment;
        _unitOfWork = unitOfWork;
    }

    [Route("posts/")]
    public IActionResult Index()
    {
        var postList = _unitOfWork.PostRepository.GetAll();
        return View(postList.OrderBy(p => p.Title));
    }

    [Route("posts/{year:int}/{month:int}/{day:int}/{slug}")]
    public IActionResult Get(int? year, int? month, int? day, string? slug)
    {
        if (slug == null)
        {
            return NotFound();
        }

        Post post = _unitOfWork.PostRepository.Get(p =>
            p.Slug == slug && p.CreatedOn.Year == year && p.CreatedOn.Month == month && p.CreatedOn.Day == day);
        return View(post);
    }

    [HttpGet]
    [Route("posts/create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Route("posts/create")]
    public IActionResult Create(Post post)
    {
        IFormFile file = Request.Form.Files.FirstOrDefault();
        var targetPath = @"images/posts";
        FileManagementUtility fileManagementUtility = new FileManagementUtility(_webHostEnvironment);
       
        if (ModelState.IsValid)
        {
            if (_dbContext.Posts.Any(p =>
                    p.Slug == post.Slug))
            {
                ModelState.AddModelError("", "Post title can not be duplicate");
                return View();
            }
            post.Image = fileManagementUtility.UploadFile(file, targetPath);
            _unitOfWork.PostRepository.Add(post);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [Route("posts/{year:int}/{month:int}/{day:int}/{slug}/edit")]
    public IActionResult Edit(int? year, int? month, int? day, string? slug)
    {
        if (slug == null) return NotFound();
        Post post = _unitOfWork.PostRepository.Get(p =>
            p.Slug == slug && p.CreatedOn.Year == year && p.CreatedOn.Month == month && p.CreatedOn.Day == day);
        return View(post);
    }

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

    [HttpPost]
    public IActionResult Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Post post = _unitOfWork.PostRepository.Get(p => p.Id == id);
        _unitOfWork.PostRepository.Remove(post);
        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }
}