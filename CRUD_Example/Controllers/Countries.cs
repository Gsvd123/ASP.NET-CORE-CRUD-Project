using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using System;

namespace CRUD_Example.Controllers
{
    [Route("[controller]")]
    public class Countries : Controller
    {
        private readonly ICountriesService _countriesService;

        public Countries(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }
        [Route("[action]")]
        public IActionResult UploadFromExcelFile()
        {
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcelFile(IFormFile excelFile)
        {
            if(excelFile == null || excelFile.Length==0)
            {
                ViewBag.ErrorMessage = "Please select a xlsx file";
                return View();
            }

            if (!Path.GetExtension(excelFile.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase)) {

                ViewBag.ErrorMessage = "UnSupported file .'xlsx' file is expected ";
                return View();
            }
          int countriesInserted=await _countriesService.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Success = $"{countriesInserted} countries Uploaded";
            return View();
        }
    }
}
