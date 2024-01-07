using Feeds.Data;
using Feeds.Models;
using Feeds.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Feeds.Controllers;

public class PostController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public PostController(ApplicationDbContext dbContext, IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
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
        if (ModelState.IsValid)
        {
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
            _unitOfWork.PostRepository.Update(post);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Get), new { id = post.Id });
        }

        return View();
    }

    // [HttpGet]
    // public IActionResult Delete(int? id)
    // {
    //     if (id == null) return NotFound();
    //     Post post = _unitOfWork.PostRepository.Get(p => p.Id == id);
    //     return View(post);
    // }

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