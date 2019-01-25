using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InstrumentShop.Models;

namespace InstrumentShop.Controllers
{
    public class CartController : Controller
    {

        public List<Models.Instrument> productListAll = Models.Instrument.GetData(); // All products
        public Models.UserData userdata; // User data


        // ACTION: SHOPPING CART


        public ActionResult ShoppingCart()
        {
            // Get user data/shopping list

            var UserID = (int)Session["UserID"];
            var CurrentUser = UserData.GetUserData(UserID);

            ViewModel VM = ViewModel.View(productListAll, CurrentUser);

            return View(VM);
        }


        // ACTION: EDIT QUANTITY

        // Handles plus, minus & remove-buttons in shopping cart
        // Session["CartCount"] manages ShoppingCart-icon number


        public ActionResult EditQuantity(string actionType, int cartID)
        {
            var UserID = (int)Session["UserID"];
            var CurrentUser = UserData.GetUserData(UserID);

            if (actionType == "increase")
            {
                CurrentUser.ShoppingCartList.ElementAt(cartID).Quantity++;
                Session["CartCount"] = (int)Session["CartCount"] + 1;
            }
            if (actionType == "decrease")
            {
                if (CurrentUser.ShoppingCartList.ElementAt(cartID).Quantity == 1)
                {
                    // Remove if decreased to 0
                    Session["CartCount"] = (int)Session["CartCount"] - CurrentUser.ShoppingCartList.ElementAt(cartID).Quantity;
                    CurrentUser.ShoppingCartList.RemoveAt(cartID);
                }
                else
                {
                    CurrentUser.ShoppingCartList.ElementAt(cartID).Quantity--;
                    Session["CartCount"] = (int)Session["CartCount"] - 1;
                }
            }
            if (actionType == "remove")
            {
                Session["CartCount"] = (int)Session["CartCount"] - CurrentUser.ShoppingCartList.ElementAt(cartID).Quantity;
                CurrentUser.ShoppingCartList.RemoveAt(cartID);
            }

            // Save data

            UserData.SaveUserData(CurrentUser);

            return RedirectToAction("ShoppingCart", "Cart");
        }


        // ACTION: ADD TO CART 

        // Adds item to cart, if more than one of the same product is added the CartItem 'Quantity' parameter is increased


        public ActionResult AddToCart(int id)
        {
            Session["CartCount"] = (int)Session["CartCount"] + 1;

            foreach (Models.Instrument product in productListAll)
            {
                if (product.ID == id)
                {
                    userdata = Models.UserData.GetUserData((int)Session["UserID"]);

                    if (userdata.ShoppingCartList == null)
                    {
                        userdata.ShoppingCartList = new List<UserData.CartItem>();
                    }

                    // Increase "Quantity" if product already exists in cart

                    var exists = false;
                    foreach (var cartitem in userdata.ShoppingCartList)
                    {
                        if (cartitem.ProductID == id)
                        {
                            exists = true;
                            cartitem.Quantity++;
                        }
                    }

                    // Add item to cart

                    if (exists == false)
                    {
                        userdata.ShoppingCartList.Add(new UserData.CartItem
                        {
                            ProductID = product.ID,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            Quantity = 1
                        });
                    }

                    UserData.SaveUserData(userdata);
                }
            }

            string category = (string)TempData["Category"];  // Stay in category, used by HOME/INDEX

            ViewModel VM = ViewModel.View(productListAll, userdata, category);

            // Redirect to product info page if AddtoCart button was pressed there..

            if (TempData["ProductInfo"] is int)
            {
                Instrument productdata = productListAll.Where(x => x.ID == id).FirstOrDefault();

                return RedirectToAction("Product", "Home", productdata);
            }

            return RedirectToAction("Index", "Home", VM);
        }
    }
}