using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ulesanne1605_poko.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Customer> Cities;
            Cities = _context.Customers.ToList();
            return View(Cities);
        }

        [HttpGet]

        public IActionResult Create()
        {
            Customer Customer = new Customer();
            ViewBag.Countries =GetCountries();
            return View(Customer);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
    }
}
