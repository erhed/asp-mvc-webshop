using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace InstrumentShop.Models
{
    public class Instrument
    {
        // PROPERTIES

        public int ID { get; set; }
        public string ProductName { get; set; }
        public string ManufacturerWebsite { get; set; }
        public int ReleaseYear { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Sold { get; set; }
        public int Inventory { get; set; }
        public int Clicks { get; set; }
        public int Points { get; set; }

        // PRODUCT DATA FILEPATH

        public static string filepath = HttpContext.Current.Server.MapPath("~/App_Data/Storage/products.json");

        // SAVE DATA TO FILE

        public static void SaveData(List<Instrument> instruments)
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                Formatting = Formatting.Indented
            };

            string json = JsonConvert.SerializeObject(instruments.ToArray(), settings);
            System.IO.File.WriteAllText(filepath, json);
        }

        // GET DATA FROM FILE

        public static List<Instrument> GetData()
        {

            List<Instrument> data;

            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var json = System.IO.File.ReadAllText(filepath);
            data = JsonConvert.DeserializeObject<List<Instrument>>(json, settings);

            // Sorting algorithm

            // In stock - least relevant
            data = data.OrderBy(x => x.Inventory).ToList();
            int points = 0;
            foreach (var d1 in data)
            {
                points = points + 2;
                d1.Points = points;
            }

            // Clicks - medium relevance
            data = data.OrderBy(x => x.Clicks).ToList();
            points = 0;
            foreach (var d2 in data)
            {
                points = points + 5;
                d2.Points += points;
            }

            // Sold - most relevant
            data = data.OrderBy(x => x.Sold).ToList();
            points = 0;
            foreach (var d3 in data)
            {
                points = points + 8;
                d3.Points += points;
            }

            // Order by points
            data = data.OrderByDescending(x => x.Points).ToList();
            SaveData(data);

            return data;
        }
    }

    // SUBCLASSES

    public class Synthesizer : Instrument
    {
        public int NumberOfKeys { get; set; }
        public bool MIDI { get; set; }
    }

    public class Bass : Instrument
    {
        public int NumberOfStrings { get; set; }
        public bool Electric { get; set; }
    }

    public class Guitar : Instrument
    {
        public string StringType { get; set; }
        public bool Cutaway { get; set; }
    }
}