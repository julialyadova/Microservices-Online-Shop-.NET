using System.Linq;
using System.Text.RegularExpressions;

namespace Api.Data
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public double Price { get; set; }
        public string ImageURL { get; set; }
        public bool IsAvailable { get; set; }
        public int Discount { get; set; }

        public bool Validate()
        {
            return ValidateName() && ValidatePrice() && ValidateImageURL() && ValidateDiscount();
        }

        public bool ValidateFK(ApplicationDbContext db)
        {
            return Category != null && db.Categories.Any(c => c.Id == Category.Id);
        }

        public bool ValidateName()
        {
            var rgx = new Regex("[a-zA-Z0-9а-яА-Я ,.]");
            return !string.IsNullOrWhiteSpace(Name) && rgx.Match(Name).Success;
        }

        public bool ValidatePrice()
        {
            return Price >= 0;
        }

        public bool ValidateImageURL()
        {
            return true; /*ImageURL == null ||
                ((ImageURL.EndsWith(".png") || ImageURL.EndsWith(".jpg"))
                && !ImageURL.Contains('?') && !ImageURL.Contains('#'));*/
        }

        public bool ValidateDiscount()
        {
            return Discount >= 0 && Discount <= 100;
        }
    }
}
