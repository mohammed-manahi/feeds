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

    public IActionResult Index()
    {
        var postList = _unitOfWork.PostRepository.GetAll();
        return View(postList.OrderBy(p => p.Title));
    }

    public IActionResult Get(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Post post = _unitOfWork.PostRepository.Get(p => p.Id == id);
        return View(post);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Post post)
    {
        IFormFile file = Request.Form.Files.FirstOrDefault();
        var targetPath = @"images/posts";
        FileManagementUtility fileManagementUtility = new FileManagementUtility(_webHostEnvironment);
        if (ModelState.IsValid)
        {
            post.Image = fileManagementUtility.UploadFile(file, targetPath);
            _unitOfWork.PostRepository.Add(post);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [HttpGet]
    public IActionResult Edit(int? id)
    {
        if (id == null) return NotFound();
        Post post = _unitOfWork.PostRepository.Get(p => p.Id == id);
        return View(post);
    }

    [HttpPost]
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
            return RedirectToAction(nameof(Get), new { id = post.Id });
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