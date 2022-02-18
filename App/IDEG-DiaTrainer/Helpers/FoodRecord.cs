using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /// <summary>
    /// FoodRecord class - stores information about food
    /// </summary>
    public class FoodRecord
    {
        // unique identifier - not displayed in any outputs
        public string Identifier { get; set; }

        // food name - displayed as a title
        public string Name { get; set; }

        // filename of image, may be empty
        public string ImageFilename { get; set; }

        // base amount of this food - the contents (cho, sugar, fat, ...) in parameters below are for exactly this amount of food
        public double? BaseAmount { get; set; }

        // base amount of one portion - this is a default value of inputs, and represents a single portion of this meal
        // e.g.; base amount is 100g, but an average banana weights 118g
        public double? PortionAmount { get; set; }

        // default units for this kind of meal; typically "g" for grams and "ml" for milliliters
        public string Units { get; set; }

        // calories content [kcal]
        public double? Calories { get; set; }

        // carbohydrates content [g]
        public double? Carbohydrates { get; set; }

        // sugar content [g]
        public double? Sugar { get; set; }

        // fat content [g]
        public double? Fat { get; set; }

        // proteins content [g]
        public double? Proteins { get; set; }

        // fibre content [g]
        public double? Fibre { get; set; }
    }
}
