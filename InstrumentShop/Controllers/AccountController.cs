using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstrumentShop.Models;

namespace InstrumentShop.Controllers
{
    public class AccountController : Controller
    {

        public List<Models.Instrument> productListAll = Models.Instrument.GetData(); // All products
        public Models.UserData userdata; // User data


        // ACTION: INDEX

        // Logs user in


        public ActionResult Index()
        {

            // CATCH LOGIN FORM

            if (HttpContext.Request.RequestType == "POST")
            {
                var Email = Request.Form["email"];
                var Password = Request.Form["password"];

                var CurrentUser = UserData.GetUserData(Email);
                if (CurrentUser != null && CurrentUser.Password == Password)
                {
                    Session["UserID"] = CurrentUser.ID;
                    Session["UserFirstName"] = CurrentUser.FirstName;

                    // Items in cart

                    var total = 0;
                    foreach(var cartitem in CurrentUser.ShoppingCartList)
                    {
                        total += cartitem.Quantity;
                    }
                    Session["CartCount"] = total; // SchoppingCart-icon number used in _Header

                    // Redirect login to product page

                    if (Session["ProductInfo"] is int)
                    {
                        Instrument productdata = productListAll.Where(x => x.ID == (int)Session["ProductInfo"]).FirstOrDefault();
                        Session.Remove("ProductInfo");

                        return RedirectToAction("Product", "Home", productdata);
                    }

                    return RedirectToAction("Index", "Home");
                }
            }

            return RedirectToAction("Index", "Home");
        }


        // ACTION: LOG OUT


        public ActionResult LogOut()
        {

            // Clear all account related session variables

            Session.Remove("UserID");
            Session.Remove("CartCount");
            Session.Remove("UserFirstName");

            ViewModel VM = ViewModel.View(productListAll, "all");

            return RedirectToAction("Index", "Home", VM);
        }


        // ACTION: REGISTER

        // Create account-page


        public ActionResult Register()
        {

            Session.Remove("ProductInfo");

            // CATCH FORM

            if (HttpContext.Request.RequestType == "POST")
            {
                List<UserData> userlist = UserData.GetUsers();
                int userID = userlist.Count;

                var email = Request.Form["Email"];
                var password = Request.Form["Password"];
                var confirmPassword = Request.Form["ConfirmPassword"];

                var firstName = Request.Form["FirstName"];
                var lastName = Request.Form["LastName"];
                var address = Request.Form["Address"];
                var postalCode = Request.Form["PostalCode"];
                var postalAddress = Request.Form["PostalAddress"];
                var country = Request.Form["Country"];

                // Create user

                UserData userdata = new UserData()
                {
                    ID = userID,
                    Email = email,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Address = address,
                    PostalCode = postalCode,
                    PostalAddress = postalAddress,
                    Country = country
                };

                // Save user

                UserData.SaveUserData(userdata);

                // Get and save user info for display in _Header

                var CurrentUser = UserData.GetUserData(userID);
                Session["UserID"] = CurrentUser.ID;
                Session["UserFirstName"] = CurrentUser.FirstName;
                Session["CartCount"] = 0;

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}