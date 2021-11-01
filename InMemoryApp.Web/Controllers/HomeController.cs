using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InMemoryApp.Web.Controllers
{
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

        public List<Person> GetPersonsFromFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/cities.txt");
            List<Person> persons = new();
            foreach (var person in System.IO.File.ReadLines(path))
            {
                //ID,Job Title,Email Address,FirstName LastName
                var personInfoArray = person.Split(",");

                persons.Add(new Person
                {
                    Id = Convert.ToInt32(personInfoArray[0]),
                    Job = personInfoArray[1],
                    Title = personInfoArray[2],
                    Email = personInfoArray[3],
                    Address = personInfoArray[4],
                    FirstName = personInfoArray[5],
                    LastName = personInfoArray[6],
                });
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