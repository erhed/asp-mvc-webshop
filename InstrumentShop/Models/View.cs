using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InstrumentShop.Models
{
    public class ViewModel
    {
        public List<Instrument> ProductList { get; set; }
        public string Category { get; set; }
        public UserData UserData { get; set; }

        // ViewModels

        public static ViewModel View(List<Instrument> productlist, string category)
        {
            ViewModel VM = new ViewModel
            {
                ProductList = productlist,
                Category = category
            };

            return VM;
        }

        public static ViewModel View(List<Instrument> productlist, UserData userdata)
        {
            ViewModel VM = new ViewModel
            {
                ProductList = productlist,
                UserData = userdata
            };

            return VM;
        }

        public static ViewModel View(List<Instrument> productlist, UserData userdata, string category)
        {
            ViewModel VM = new ViewModel
            {
                ProductList = productlist,
                UserData = userdata,
                Category = category
            };

            return VM;
        }
    }
}