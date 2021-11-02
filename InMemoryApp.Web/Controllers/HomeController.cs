using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IMemoryCache _memoryCache;

        public HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            
            return View(GetPersonList());
        }

        [HttpPost]
        private List<Person> GetPersonList()
        {
            var watch = Stopwatch.StartNew();

            
            
            if (_memoryCache.Get<List<Person>>("PersonList") == null)
            {
                var personList = GetPersonsFromFile();
                MemoryCacheEntryOptions options = new()
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(1), SlidingExpiration = TimeSpan.FromSeconds(30), Priority = CacheItemPriority.High
                };
                _memoryCache.Set<List<Person>>("PersonList", personList);
                watch.Stop();
                TempData["TimeInfo"] = new TimeInfo {Source = "File", ExecutionTime = $"{watch.ElapsedMilliseconds.ToString()} ms"};
                return personList;
            }
            watch.Stop();
            TempData["TimeInfo"] = new TimeInfo {Source = "Cache", ExecutionTime = $"{watch.ElapsedMilliseconds.ToString()} ms"};
            return _memoryCache.Get<List<Person>>("PersonList");
           
        }
        public JsonResult CleanCache()
        {
            try
            {
                _memoryCache.Remove("PersonList");
                return Json(new {status=true,message="Clean cache successfully"});
            }
            catch (Exception)
            {
                return Json(new {status=false,message="Something went wrong"});
            }
        }
        private List<Person> GetPersonsFromFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cities.txt");
            List<Person> persons = new();
            try
            {
                foreach (var person in System.IO.File.ReadLines(path))
                {
                    var personInfoArray = person.Split(",");

                    persons.Add(new Person
                    {
                        Id = Convert.ToInt32(personInfoArray[0]),
                        Job = personInfoArray[1],
                        Email = personInfoArray[2],
                        FullName = personInfoArray[3],
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            return persons;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}