using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstrumentShop.Models;

namespace InstrumentShop.Controllers
{
    public class HomeController : Controller
    {

        public List<Models.Instrument> productListAll = Models.Instrument.GetData(); // All products
        public List<Models.Instrument> productListShow = new List<Models.Instrument>(); // Products to be sent to View
        public Models.UserData userdata; // User data


        // ACTION: INDEX

        // Show all products or products in category (sorted by "popularity")

        public ActionResult Index(string category = "all")
        {

            TempData.Remove("ProductInfo"); // Used for routing back to product page when logging in from a product page

            Instrument.SaveData(productListAll);

            // CATEGORY - Only send products in category to view

            if (category == "all")
            {
                productListShow = productListAll;
                TempData["Category"] = "all"; // Used for routing
            }
            else
            {
                foreach (var product in productListAll)
                {
                    if (category == "synthesizers" && product is InstrumentShop.Models.Synthesizer)
                    {
                        productListShow.Add(product);
                        TempData["Category"] = "synthesizers";
                    }
                    if (category == "bass" && product is InstrumentShop.Models.Bass)
                    {
                        productListShow.Add(product);
                        TempData["Category"] = "bass";
                    }
                    if (category == "guitars" && product is InstrumentShop.Models.Guitar)
                    {
                        productListShow.Add(product);
                        TempData["Category"] = "guitars";
                    }
                }
            }

            ViewModel VM = ViewModel.View(productListShow, category);

            return View(VM);
        }


        // ACTION: PRODUCT

        // Display product info page


        public ActionResult Product(int id)
        {
            Instrument productdata = productListAll.Where(x => x.ID == id).FirstOrDefault(); // Get product data by ID

            TempData["ProductInfo"] = id; // Used for routing back to product page after "AddToCart"
            Session["ProductInfo"] = id; // Used for routing back to product page after "Login"

            // Add click to "Instrument.Click"

            List<Models.Instrument> productListAllCopy = Models.Instrument.GetData();
            foreach (var product in productListAllCopy)
            {
                if (product.ID == id)
                {
                    product.Clicks++;
                }
            }

            Instrument.SaveData(productListAllCopy); // Save clicks to file

            return View(productdata);
        }


        // ACTION: BUY

        // Reset cart counter and send bought products to Reciept page


        public ActionResult Buy()
        {
            userdata = Models.UserData.GetUserData((int)Session["UserID"]);

            Session["CartCount"] = 0; // Reset CartItem-counter on Shopping Cart-icon

            TempData["UserData"] = userdata; // Copy Shopping List for Reciept

            ViewModel VM = ViewModel.View(productListAll, userdata);

            return RedirectToAction("Reciept", VM);
        }


        // ACTION: RECIEPT

        // Reciept, empties Shopping Cart & increases "Instrument.Sold"


        public ActionResult Reciept(ViewModel VMget)
        {

            // Instrument.Sold increase

            List<Models.Instrument> productListAllCopy2 = Models.Instrument.GetData();

            UserData userdata2 = Models.UserData.GetUserData((int)Session["UserID"]);

            foreach (var cartitem in userdata2.ShoppingCartList)
            {
                foreach (var product in productListAllCopy2)
                {
                    if (product.ID == cartitem.ProductID)
                    {
                        product.Sold += 1 * cartitem.Quantity;
                    }
                }
            }

            Instrument.SaveData(productListAllCopy2);

            UserData fromCart = (UserData)TempData["UserData"]; // UserData with copy of Shopping Cart-list

            ViewModel VM = ViewModel.View(productListAll, fromCart);

            // Clear shopping cart and save empty cart to user data file

            userdata2.ShoppingCartList.Clear();
            UserData.SaveUserData(userdata2);

            return View(VM);
        }
    }
}