using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace InstrumentShop.Models
{
    public class UserData
    {

        // USER PROPERTIES

        public int ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PostalAddress { get; set; }
        public string Country { get; set; }

        public List<CartItem> ShoppingCartList { get; set; }

        public static List<UserData> UserList = GetUsers();

        // GET USER DATA BY EMAIL

        public static UserData GetUserData(string email)
        {
            var selected = UserList.Where(x => x.Email == email).FirstOrDefault();
            return selected;
        }

        // GET USER DATA BY ID

        public static UserData GetUserData(int id)
        {
            UserData userdata;

            string filepath = HttpContext.Current.Server.MapPath("~/App_Data/Storage/user" + id + ".json");

            if (System.IO.File.Exists(filepath))
            {
                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects
                };

                var json = System.IO.File.ReadAllText(filepath);
                userdata = JsonConvert.DeserializeObject<UserData>(json, settings);
            }
            else
            {
                userdata = UserList.Where(x => x.ID == id).FirstOrDefault();
            }

            return userdata;
        }

        // SAVE USER DATA

        public static void SaveUserData(UserData user)
        {
            string filepath = HttpContext.Current.Server.MapPath("~/App_Data/Storage/user" + user.ID + ".json");

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(user, settings);
            System.IO.File.WriteAllText(filepath, json);
        }

        // GET ALL USERS

        public static List<UserData> GetUsers()
        {
            List<UserData> UserList = new List<UserData>();
            UserData userdata;
            int userID = 0;
            bool fileExists = true;

            while (fileExists)
            {
                string filepath = HttpContext.Current.Server.MapPath("~/App_Data/Storage/user" + userID + ".json");
                if (System.IO.File.Exists(filepath))
                {
                    var settings = new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    };
                    var json = System.IO.File.ReadAllText(filepath);
                    userdata = JsonConvert.DeserializeObject<UserData>(json, settings);
                    UserList.Add(userdata);
                    userID++;
                }
                else
                {
                    fileExists = false;
                }
                
            }
            return UserList;
        }

        // CARTITEM FOR ShoppingCartList

        public class CartItem
        {
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }
        }
    }
}