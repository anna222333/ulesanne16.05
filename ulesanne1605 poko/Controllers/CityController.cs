using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ulesanne1605_poko.Models;
using static System.Net.Mime.MediaTypeNames;

namespace ulesanne1605_poko.Controllers
{
    public class CityController : Controller
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<City> Cities;
            Cities = _context.Cities.ToList();
            return View(Cities);
        }

        [HttpGet]
        public IActionResult Create()
        {
            City City = new City();
            ViewBag.Countries = GetCountries();
            return View(City);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Create(City City)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = GetCountries();
                return View(City);
            }


            _context.Add(City);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





        [HttpGet]
        public IActionResult Details(int Id)
        {
            City City = _context.Cities
            .Where(c => c.Id == Id).FirstOrDefault();
            ViewBag.Countries = GetCountries();
            ViewBag.Cities = _context.Cities
    .Where(c => c.CountryId == City.CountryId)  // или все города, если хочешь
    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
    .ToList();
            return View(City);
        }


        [HttpGet]
        public IActionResult Edit(int Id)
        {
            City City = _context.Cities
                .Where(c => c.Id == Id).FirstOrDefault();
            ViewBag.Countries = GetCountries();
            ViewBag.Cities = _context.Cities
    .Where(c => c.CountryId == City.CountryId)  // или все города, если хочешь
    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
    .ToList();
            return View(City);
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(City City)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = GetCountries();
                return View(City);
            }

            _context.Attach(City);
            _context.Entry(City).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Delete(int Id)
        {
            City City = _context.Cities
                .Where(c => c.Id == Id).FirstOrDefault();
            return View(City);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(City City)
        {
            _context.Attach(City);
            _context.Entry(City).State = EntityState.Deleted;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateFromModal([FromBody] City city)
        {
            if (string.IsNullOrWhiteSpace(city.Name) ||
                string.IsNullOrWhiteSpace(city.Code) ||
                city.CountryId == 0)
            {
                Response.StatusCode = 400;
                return Json(new { error = "Name, Code и CountryId обязательны" });
            }

            _context.Cities.Add(city);
            _context.SaveChanges();
            return Json(new { id = city.Id, name = city.Name });
        }



        private List<SelectListItem> GetCountries()
        {
            var lstCountries = new List<SelectListItem>();
            List<Country> Countries = _context.Countries.ToList();

            lstCountries = Countries.Select(ct => new SelectListItem()
            {
                Value = ct.Id.ToString(),
                Text = ct.Name
            }).ToList();

            var defItem = new SelectListItem()
            {
                Value = "",
                Text = "--Select Country--"
            };

            lstCountries.Insert(0, defItem);
            return lstCountries;
        }
    }
}

