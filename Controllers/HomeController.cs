using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;

namespace MyMvcAuthProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Listings()
    {
        return View();
    }
       public IActionResult About()
    {
        return View();
    }
       public IActionResult AdminListings()
    {
        return View();
    }
         public IActionResult Contact()
    {
        return View();
    }

         public IActionResult PropertyDetail()
    {
        return View();
    }

    

    public IActionResult Dashboard(){
        return View();
    }
   
       public IActionResult Saved(){
        return View();
    }
       public IActionResult Messages(){
        return View();
    }
    public IActionResult Profile(){
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
