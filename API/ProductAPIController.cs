using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using ProductApp.Models;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;

namespace ProductApp.API
{
    [Route("api/[action]")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly IWebHostEnvironment _HostEnvironment;

        public ProductAPIController(IJwtAuthenticationManager jwtAuthenticationManager, IWebHostEnvironment _HostEnvironment)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this._HostEnvironment = _HostEnvironment;
        }

        #region Actions
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(IFormCollection fc)
        {
            var username = fc["USERNAME"];
            var password = fc["PASSWORD"];
            string[] result = jwtAuthenticationManager.Authenticate(username, password);
            if (result[0] == "fail")
            {
                return Ok(new { status = "Failed" });
            }
            return Ok(new { status = "Success", token = result[1] });
        }

        [HttpPost]

        public IActionResult GetProductsList()
        {
            try
            {
                string FilePath = _HostEnvironment.WebRootPath + "/temp/ProductsInfo.json";
                if (!System.IO.File.Exists(FilePath))
                {
                    //If the ProductList.json file is not created already then it means that there are no products available to be displayed and it needs to be added
                    return Ok(new { status = "No Products found" });
                }
                var content = System.IO.File.ReadAllText(FilePath);
                var productInfo = JsonConvert.DeserializeObject<List<Product>>(content);  //Convert the string content retrieved from the file to a list of products format
                return Ok(new { status = "Success", products = productInfo });
            }
            catch (Exception e)
            {
                return Ok(new { status = "Failed", Message = e.Message });
            }

        }

        [HttpPost]

        public IActionResult GetProductDetails(IFormCollection fc)
        {
            try
            {
                var productID = new Guid(fc["ProductID"]);
                string FilePath = _HostEnvironment.WebRootPath + "/temp/ProductsInfo.json";
                if (System.IO.File.Exists(FilePath))
                {
                    var content = System.IO.File.ReadAllText(FilePath);
                    var products = JsonConvert.DeserializeObject<List<Product>>(content);
                    var result = products.FirstOrDefault(x => x.ID == productID);
                    if (result != null)
                    {
                        return Ok(new { status = "Success", result = result });
                    }
                    else
                    {
                        return Ok(new { status = "Failed", Message = "Product not found" });
                    }
                }
                else
                {
                    return Ok(new { status = "Failed", Message = "No Products found" });
                }

            }
            catch (Exception e)
            {
                return Ok(new { status = "Failed", Message = e.Message });

            }

        }

        [HttpPost]

        public IActionResult AddNewProduct(IFormCollection fc)
        {
            var name = fc["Name"];
            var price = fc["Price"];
            var desc = fc["Description"];
            var photo = fc["Photo"];
            Product p = new Product();
            List<Product> products;
            var ContentPath = _HostEnvironment.WebRootPath + "/temp";

            p.Name = name;
            p.Description = desc;
            p.ID = Guid.NewGuid();
            p.Price = Convert.ToInt32(price);
            p.Path = "https://" + Request.Host + "/temp/" + p.ID + "/" + p.ID + ".jpeg";

            using (var ms = new MemoryStream())
            {
                Request.Form.Files[0].CopyTo(ms);
                byte[] MainFile = ms.ToArray();
                Directory.CreateDirectory(ContentPath + "/" + p.ID);

                AppendAllBytes(ContentPath + "/" + p.ID + "/" + p.ID + ".jpeg", MainFile);
            }
            if (System.IO.File.Exists(_HostEnvironment.WebRootPath + "/temp/ProductsInfo.json"))
            {
                //If the file exists then read the already present products from the file
                products = JsonConvert.DeserializeObject<List<Product>>(System.IO.File.ReadAllText(_HostEnvironment.WebRootPath + "/temp/ProductsInfo.json"));
            }
            else
            {
                //if the file does not exist
                products = new List<Product>();
            }
            products.Add(p);
            System.IO.File.WriteAllText(_HostEnvironment.WebRootPath + "/temp/ProductsInfo.json", JsonConvert.SerializeObject(products));
            return Ok(new { status = "Success", NewProduct = p });
        }

        [HttpPost]
        public IActionResult EditProduct(IFormCollection fc)
        {
            try
            {
                var productID = new Guid(fc["ProductID"]);
                var ContentPath = _HostEnvironment.WebRootPath + "/temp";
                string FilePath = _HostEnvironment.WebRootPath + "/temp/ProductsInfo.json";
                if (System.IO.File.Exists(FilePath))
                {
                    var content = System.IO.File.ReadAllText(FilePath);
                    var products = JsonConvert.DeserializeObject<List<Product>>(content);
                    var result = products.FirstOrDefault(x => x.ID == productID);
                    if (result != null)
                    {
                        result.Description = fc["Description"];
                        result.Name = fc["Name"];
                        result.Price = Convert.ToInt32(fc["Price"]);
                        using (var ms = new MemoryStream())
                        {
                            Request.Form.Files[0].CopyTo(ms);
                            byte[] MainFile = ms.ToArray();
                            AppendAllBytes(ContentPath + "/" + result.ID + "/" + result.ID + ".jpeg", MainFile);
                        }
                        var index = products.IndexOf(result);
                        products.RemoveAt(index);
                        products.Insert(index, result);
                        return Ok(new { status = "Success" });
                    }
                    else
                    {
                        return Ok(new { status = "Failed", Message = "Product not found" });
                    }
                }
                else
                {
                    return Ok(new { status = "Failed", Message = "No Products found" });
                }

            }
            catch (Exception e)
            {
                return Ok(new { status = "Failed", Message = e.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteProduct(IFormCollection fc)
        {
            try
            {
                var productID = new Guid(fc["ProductID"]);
                var ContentPath = _HostEnvironment.WebRootPath + "/temp";
                string FilePath = _HostEnvironment.WebRootPath + "/temp/ProductsInfo.json";
                if (System.IO.File.Exists(FilePath))
                {
                    var content = System.IO.File.ReadAllText(FilePath);
                    var products = JsonConvert.DeserializeObject<List<Product>>(content);
                    var result = products.FirstOrDefault(x => x.ID == productID);
                    if (result != null)
                    {
                        products.Remove(result);
                        System.IO.File.WriteAllText(FilePath, JsonConvert.SerializeObject(products));
                        Directory.Delete(ContentPath + "/" + result.ID, true);
                        return Ok(new { status = "Success" });
                    }
                    else
                    {
                        return Ok(new { status = "Failed", Message = "Product not found" });
                    }
                }
                else
                {
                    return Ok(new { status = "Failed", Message = "File not found" });

                }
            }
            catch (Exception e)
            {
                return Ok(new { status = "Failed", Message = e.Message });

            }
        }

        #endregion

        #region helperMethods

        public static void AppendAllBytes(string path, byte[] bytes)
        {

            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        #endregion

    }
}
