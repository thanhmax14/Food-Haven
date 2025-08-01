using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ViewModels
{
    public class ExpertRecipeViewModel
    {
        public string title { get; set; }

        public List<string> ingredients { get; set; }

        public List<string> directions { get; set; }

        public List<string> ner { get; set; }

        public string link { get; set; }

        public string source { get; set; }
    }
    public class ExpertRecipeEditViewModel
    {
        public Guid Id { get; set; }
        public string title { get; set; }
        public List<string> ingredients { get; set; }
        public List<string> directions { get; set; }
        public List<string> ner { get; set; }
        public string link { get; set; }
        public string source { get; set; }
    }
}
