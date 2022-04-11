using System.Linq;
using System.Text.RegularExpressions;

namespace Api.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Category Parent { get; set; }

        public bool Validate()
        {
            var rgx = new Regex("[a-zA-Z0-9а-яА-Я ,.]");
            return !string.IsNullOrWhiteSpace(Name) && rgx.Match(Name).Success;
        }

        public bool ValidateFK(ApplicationDbContext db)
        {
            if (Parent == null || Parent.Id == 0) 
                return true;

            var parentExists = db.Categories.Any(c => c.Id == Parent.Id);
            return parentExists && !IsCycledWith(Parent.Id, db);
        }

        private bool IsCycledWith(int parentId, ApplicationDbContext db)
        {
            while (parentId != 0)
            {
                if (parentId == Id)
                    return true;
                parentId = db.Categories.Where(c => c.Id == parentId).Select(c => c.Parent == null ? 0 : c.Parent.Id).FirstOrDefault();
            }
            return false;
        }
    }
}
