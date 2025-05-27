using Microsoft.AspNetCore.Mvc;


namespace ulesanne1605_poko.Controllers
{
    public class CountryController : Controller
    {
        private readonly AppDbContext _context;

        public CountryController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Country> countries;
            countries =_context.Countries.ToList();
            return View(countries);
        }

        [HttpGet]


        public IActionResult Create()
        {
            Country country = new Country();
            if (country == null)
                return NotFound();
            return View(country);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]

        public IActionResult Create(Country country)
        {
            if (!ModelState.IsValid)
            {
                return View(country);
            }

            _context.Add(country);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]

        public IActionResult Details(int Id)
        {
            Country country = GetCountry(Id);
            return View(country);
        }
        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Country country = GetCountry(Id);
            if (country == null)
                return NotFound();
            return View(country);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Edit(Country country)
        {
            if (!ModelState.IsValid)
            {
                return View(country);
            }
            _context.Attach(country);
            _context.Entry(country).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        private Country GetCountry(int Id)
        {
            Country country;
            country = _context.Countries
                .Where(c => c.Id == Id).FirstOrDefault();
            return country;
        }


        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Country country = GetCountry(Id);
            return View(country);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateFromModal([FromBody] Country country)
        {
            if (string.IsNullOrWhiteSpace(country.Name) ||
      string.IsNullOrWhiteSpace(country.Code) ||
      string.IsNullOrWhiteSpace(country.CurrencyName))
            {
                Response.StatusCode = 400;
                return Json(new { error = "Name, Code и CurrencyName обязательны" });
            }

            _context.Countries.Add(country);
            _context.SaveChanges();
            return Json(new { id = country.Id, name = country.Name });
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(Country country)
        {
        
            var hasCities = _context.Cities.Any(c => c.CountryId == country.Id);
            if (hasCities)
            {
                ModelState.AddModelError("", "You can not delete the country with cities");
                country = GetCountry(country.Id);
                return View(country);
            }

            try
            {
                _context.Attach(country);
                _context.Entry(country).State = EntityState.Deleted;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _context.Entry(country).Reload();
                ModelState.AddModelError("", ex.InnerException?.Message ?? ex.Message);
                country = GetCountry(country.Id);
                return View(country);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}