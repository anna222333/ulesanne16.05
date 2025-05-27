using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
//using static System.Net.Mime.MediaTypeNames;

namespace ulesanne1605_poko.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        private readonly IWebHostEnvironment _webHost;

        public CustomerController(AppDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public IActionResult Index()
        {
            List<Customer> customers = _context.Customers
                 .Include(c => c.City)
                .ThenInclude(city => city.Country)
                .ToList();

            return View(customers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            Customer Customer = new Customer();
            ViewBag.Countries = GetCountries();

            ViewBag.Cities = new List<SelectListItem>();

            return View(Customer);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Customer customer, IFormFile ProfilePhoto)
        {
            if (!ModelState.IsValid)
            {
                // если валидация не прошла — возвращаем список стран и городов (по выбранной стране)
                ViewBag.Countries = GetCountries();
                ViewBag.Cities = customer.CountryId > 0
                    ? GetCities(customer.CountryId)
                    : new List<SelectListItem>();
                return View(customer);
            }

            // Обработка загруженного фото
            if (ProfilePhoto != null && ProfilePhoto.Length > 0)
            {
                // Генерация уникального имени файла
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfilePhoto.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Сохраняем файл
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ProfilePhoto.CopyTo(fileStream);
                }

                // Устанавливаем путь к изображению в модель
                customer.PhotoUrl = "/images/" + uniqueFileName;
            }
            else
            {
                // Если фото нет — задаём значение по умолчанию
                customer.PhotoUrl = "/images/noimage.png";
            }

            // Сохраняем клиента
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Details(int Id)
        {

            Customer customer = _context.Customers
.Include(cty => cty.City)
.Include(cou => cou.City.Country)
.Where(c => c.Id == Id).FirstOrDefault();

            return View(customer);
        }

        [HttpGet]
        public IActionResult Edit(int Id)
        {


            Customer customer = _context.Customers
            .Include(co => co.City)
                .Where(c => c.Id == Id).FirstOrDefault();

            Console.WriteLine("PhotoUrl: " + customer.PhotoUrl);

            if (customer.City != null)
            {
                customer.CountryId = customer.City.CountryId;
            }


            ViewBag.Countries = GetCountries();
            ViewBag.Cities = GetCities(customer.CountryId);



            return View(customer);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Customer customer, bool removePhoto = false)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Countries = GetCountries();
                ViewBag.Cities = customer.CountryId > 0 ? GetCities(customer.CountryId) : new List<SelectListItem>();
                return View(customer);
            }
            if (removePhoto)
            {
                customer.PhotoUrl = "noimage.png";
            }
            else if (customer.ProfilePhoto != null)
            {
                string uniqueFileName = GetProfilePhotoFileName(customer);
                customer.PhotoUrl = uniqueFileName;
            }
            else
            {
                var existingCustomer = _context.Customers.AsNoTracking().FirstOrDefault(c => c.Id == customer.Id);
                customer.PhotoUrl = existingCustomer?.PhotoUrl ?? "noimage.png";
            }

            _context.Attach(customer);
            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Where(c => c.Id == id).FirstOrDefault();
            return View(customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete(Customer customer)
        {
            _context.Attach(customer);
            _context.Entry(customer).State = EntityState.Deleted;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public JsonResult GetCitiesByCountry(int countryId)
        {
            List<SelectListItem> cities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.Name)
                .Select(n =>
                new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name
                }).ToList();
            return Json(cities);
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

            var defItem = new SelectListItem
            {
                Value = "",
                Text = "--- Select Country ---"
            };

            lstCountries.Insert(0, defItem);

            return lstCountries;
        }


        private string GetProfilePhotoFileName(Customer customer)
        {
            string uniqueFileName = null;
            if (customer.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + customer.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    customer.ProfilePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }


        private List<SelectListItem> GetCities(int countryId)
        {
            List<SelectListItem> cities = _context.Cities
                .Where(c => c.CountryId == countryId)
                .OrderBy(n => n.Name)
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name
                })
                .ToList();

            return cities;
        }

        

    }
}