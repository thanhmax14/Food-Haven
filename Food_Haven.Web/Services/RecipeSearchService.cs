using CsvHelper;
using Food_Haven.Web.Models;
using Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Food_Haven.Web.Services
{
    public class RecipeSearchService
    {
        private static List<Recipegenare> _allRecipes;
        private static readonly object _lock = new object();
        private readonly string _csvPath;
        private static int _recipeCount = -1; // To store recipe count in cache

        public RecipeSearchService(string path)
        {
            _csvPath = path;
        }

        // Kiểm tra và nạp dữ liệu chỉ khi cần
        private void EnsureDataLoaded()
        {
            if (_allRecipes == null)
            {
                lock (_lock)
                {
                    if (_allRecipes == null) // double-check locking
                    {
                        _allRecipes = LoadRecipesFromCsv(_csvPath);
                    }
                }
            }
        }

        // 1. Đếm tất cả công thức (dòng, trừ dòng header) với cache để tiết kiệm tài nguyên
        public int GetRecipeCount()
        {
            if (_recipeCount == -1)
            {
                int count = 0;
                using var reader = new StreamReader(_csvPath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    count++;
                }
                _recipeCount = count; // Lưu lại kết quả đếm để tái sử dụng
            }
            return _recipeCount;
        }

        // 2. Lấy tất cả nguyên liệu duy nhất từ cột NER
        public List<string> GetUniqueIngredients(int maxRecipe = 100000)
        {
            var ingredientsSet = new HashSet<string>();
            using var reader = new StreamReader(_csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            int count = 0;
            while (csv.Read())
            {
                var nerRaw = csv.GetField("NER");
                if (!string.IsNullOrWhiteSpace(nerRaw))
                {
                    try
                    {
                        var nerList = JsonSerializer.Deserialize<List<string>>(nerRaw);
                        foreach (var ing in nerList)
                            ingredientsSet.Add(ing.Trim().ToLower());
                    }
                    catch { }
                }
                count++;
                if (count >= maxRecipe) break; // Đọc tối đa số lượng công thức cần thiết
            }
            return ingredientsSet.OrderBy(x => x).ToList();
        }

        // 3. Tìm công thức dựa trên nguyên liệu đã chọn
        public List<Recipegenare> FindRecipesByIngredients(List<string> selectedIngredients, int limit = 5, int maxRecipe = 100000)
        {
            var results = new List<Recipegenare>();
            using var reader = new StreamReader(_csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            int count = 0;
            while (csv.Read())
            {
                var nerRaw = csv.GetField("NER");
                if (!string.IsNullOrWhiteSpace(nerRaw))
                {
                    try
                    {
                        var nerList = JsonSerializer.Deserialize<List<string>>(nerRaw)
                            .Select(x => x.Trim().ToLower()).ToList();

                        // Kiểm tra TẤT CẢ nguyên liệu đã chọn đều nằm trong NER
                        if (selectedIngredients.All(sel => nerList.Contains(sel.Trim().ToLower())))
                        {
                            var recipe = new Recipegenare
                            {
                                title = csv.GetField("title"),
                                ingredients = csv.GetField("ingredients"),
                                directions = csv.GetField("directions"),
                                link = csv.GetField("link"),
                                source = csv.GetField("source"),
                                NER = nerRaw
                            };
                            results.Add(recipe);

                            if (results.Count >= limit)
                                break;
                        }
                    }
                    catch { }
                }
                count++;
                if (count >= maxRecipe) break; // Để tránh quét file quá lớn nếu cần
            }
            return results;
        }

        // Đọc file CSV và chuyển thành danh sách công thức
        private List<Recipegenare> LoadRecipesFromCsv(string path)
        {
            var list = new List<Recipegenare>();
            if (!File.Exists(path)) return list;
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var recipe = new Recipegenare
                    {
                        title = csv.GetField("title"),
                        ingredients = csv.GetField("ingredients"),
                        directions = csv.GetField("directions"),
                        link = csv.GetField("link"),
                        source = csv.GetField("source"),
                        NER = csv.GetField("NER")
                    };
                    list.Add(recipe);
                }
            }
            return list;
        }
        public List<Recipegenare> LoadRecipesFromCsv(int skip, int limit)
        {
            var list = new List<Recipegenare>();
            if (!File.Exists(_csvPath)) return list;

            using var reader = new StreamReader(_csvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Read();
            csv.ReadHeader();

            int index = 0;
            while (csv.Read())
            {
                if (index++ < skip) continue; // tăng index ở đây luôn, gọn gàng

                if (list.Count >= limit) break;

                try
                {
                    var recipe = new Recipegenare
                    {
                        title = csv.GetField("title"),
                        ingredients = csv.GetField("ingredients"),
                        directions = csv.GetField("directions"),
                        link = csv.GetField("link"),
                        source = csv.GetField("source"),
                        NER = csv.GetField("NER")
                    };
                    list.Add(recipe);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error at line {index + 1}: {ex.Message}");
                }
            }

            return list;
        }



        public ExpertRecipe MapToExpertRecipe(Recipegenare r)
        {
            return new ExpertRecipe
            {
                ID = Guid.NewGuid(),
                Title = r.title,
                Ingredients = r.ingredients,
                Directions = r.directions,
                Link = r.link,
                Source = r.source,
                NER = r.NER
            };
        }

    }
}
