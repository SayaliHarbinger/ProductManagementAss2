using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagementAss2.Data.Repository;
using ProductManagementAss2.Models.Domain;


namespace ProductManagementAss2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductOperations _db;

        public ProductController(IProductOperations db)
        {
            _db = db;
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            return View(await _db.GetProductListAsync());
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                var result = await _db.AddProductAsync(product);

                if (result != null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "Invalid product. Please check your inputs.");
                    return View(product);
                }
            }

            return View(product);
        }

        [Authorize(Roles = "User")]
        public  IActionResult Detail()
        {
            return View();
        }
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var product = await _db.GetProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async  Task<IActionResult> Delete(Guid id)
        {
            var product =await  _db.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var product = await _db.GetProductAsync(id);

            if (product == null)
            {
                return NotFound();
            }

           await _db.DeleteProductAsync(id);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id)
        {
            var existingProduct = await _db.GetProductAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            return View(existingProduct);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Product product)
        {
            if (id != product.ProdId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _db.UpdateProductAsync(id, product);
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Title", "Invalid product. Please check your inputs.");
                    return View(product);
                }
            }

            return View(product);
        }
    }
}
