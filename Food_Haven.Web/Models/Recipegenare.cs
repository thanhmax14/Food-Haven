    using CsvHelper.Configuration;
    namespace Food_Haven.Web.Models
    {
        public class Recipegenare
        {
            public string Unnamed { get; set; }
            public string title { get; set; }
            public string ingredients { get; set; }
            public string directions { get; set; }
            public string link { get; set; }
            public string source { get; set; }
            public string NER { get; set; }
        }

        public sealed class RecipeMap : ClassMap<Recipegenare>
        {
            public RecipeMap()
            {
                Map(m => m.Unnamed).Index(0); // Cột đầu tiên (không tên)
                Map(m => m.title).Name("title");
                Map(m => m.ingredients).Name("ingredients");
                Map(m => m.directions).Name("directions");
                Map(m => m.link).Name("link");
                Map(m => m.source).Name("source");
                Map(m => m.NER).Name("NER");
            }
        }
    }
