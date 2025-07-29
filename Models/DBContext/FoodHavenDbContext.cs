using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Models.DBContext
{
    public class FoodHavenDbContext : IdentityDbContext<AppUser>
    {
        public FoodHavenDbContext(DbContextOptions<FoodHavenDbContext> options) : base(options)
        {
        }


        public DbSet<IngredientTag> IngredientTag { get; set; }
        public DbSet<TypeOfDish> TypeOfDish { get; set; }
        public DbSet<BalanceChange> BalanceChanges { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductTypes> ProductTypes { get; set; }
        public DbSet<StoreDetails> StoreDetails { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
        public DbSet<RecipeReview> RecipeReviews { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageImage> MessageImages { get; set; }
        public DbSet<StoreReport> StoreReports { get; set; }
        public DbSet<StoreFollower> StoreFollowers { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            // Bỏ tiền tố AspNet của các bảng: mặc định
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }

            builder.Entity<StoreDetails>()
            .HasOne(h => h.AppUser)
            .WithOne(u => u.StoreDetails)
            .HasForeignKey<StoreDetails>(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

         


            builder.Entity<Product>()
           .HasOne(h => h.StoreDetails)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.StoreID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<BalanceChange>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.BalanceChanges)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Product>()
           .HasOne(h => h.Categories)
           .WithMany(h => h.Products)
           .HasForeignKey(h => h.CategoryID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductTypes>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductTypes)
           .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProductImage>()
           .HasOne(h => h.Product)
           .WithMany(h => h.ProductImages)
           .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Wishlist>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Wishlist>()
            .HasOne(h => h.Product)
            .WithMany(h => h.Wishlists)
            .HasForeignKey(h => h.ProductID).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Cart>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Carts)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.Cascade); ;

            builder.Entity<Cart>()
            .HasOne(h => h.ProductTypes)
            .WithMany(h => h.Carts)
            .HasForeignKey(h => h.ProductTypesID).OnDelete(DeleteBehavior.NoAction); ;


            builder.Entity<Order>()
            .HasOne(h => h.AppUser)
            .WithMany(h => h.Orders)
            .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderDetail>()
            .HasOne(h => h.ProductTypes)
            .WithMany(h => h.OrderDetails)
            .HasForeignKey(h => h.ProductTypesID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<OrderDetail>()
           .HasOne(h => h.Order)
           .WithMany(h => h.OrderDetails)
           .HasForeignKey(h => h.OrderID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Complaint>()
           .HasOne(h => h.OrderDetail)
           .WithMany(h => h.Complaints)
           .HasForeignKey(h => h.OrderDetailID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Recipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.Recipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Recipe>()
           .HasOne(h => h.Categories)
           .WithMany(h => h.Recipes)
           .HasForeignKey(h => h.CateID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.Recipe)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.RecipeID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RecipeReview>()
           .HasOne(h => h.Recipe)
           .WithMany(h => h.RecipeReviews)
           .HasForeignKey(h => h.RecipeID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<FavoriteRecipe>()
           .HasOne(h => h.AppUser)
           .WithMany(h => h.FavoriteRecipes)
           .HasForeignKey(h => h.UserID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Order>()
           .HasOne(h => h.Voucher)
           .WithMany(h => h.Orders)
           .HasForeignKey(h => h.VoucherID).OnDelete(DeleteBehavior.NoAction).IsRequired(false);

            builder.Entity<Recipe>()
         .HasOne(h => h.TypeOfDish)
         .WithMany(h => h.Recipes)
         .HasForeignKey(h => h.TypeOfDishID).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Voucher>()
    .HasOne(v => v.StoreDetails)
    .WithMany(s => s.Vouchers)
    .HasForeignKey(v => v.StoreID)
   .OnDelete(DeleteBehavior.NoAction).IsRequired(false);
            builder.Entity<RecipeIngredientTag>()
    .HasKey(x => new { x.RecipeID, x.IngredientTagID });

            builder.Entity<RecipeIngredientTag>()
                .HasOne(x => x.Recipe)
                .WithMany(r => r.RecipeIngredientTags)
                .HasForeignKey(x => x.RecipeID);

            builder.Entity<RecipeIngredientTag>()
                .HasOne(x => x.IngredientTag)
                .WithMany(t => t.RecipeIngredientTags)
                .HasForeignKey(x => x.IngredientTagID);


            builder.Entity<TypeOfDish>().HasData(
      new TypeOfDish
{
 Name = "Quick and Easy Dinners for One",
 IsActive = true,
 CreatedDate = DateTime.Now
},
new TypeOfDish
{
 Name = "Cooking for Two",
 IsActive = true,
 CreatedDate = DateTime.Now
},
new TypeOfDish
{
 Name = "Main Dishes",
 IsActive = true,
 CreatedDate = DateTime.Now
},
new TypeOfDish
{
 Name = "Vegetarian Main Dishes",
 IsActive = true,
 CreatedDate = DateTime.Now
},
new TypeOfDish
{
 Name = "Side Dishes",
 IsActive = true,
 CreatedDate = DateTime.Now
},
new TypeOfDish
{
 Name = "Healthy Main Dishes",
 IsActive = true,
 CreatedDate = DateTime.Now
}
);

            builder.Entity<IngredientTag>().HasData(
                    new IngredientTag
                    {
                     
                        Name = "Fish",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new IngredientTag
                    {
                     
                        Name = "Chicken",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new IngredientTag
                    {
                   
                        Name = "Beef",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new IngredientTag
                    {
                  
                        Name = "Pork",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new IngredientTag
                    {
                      
                        Name = "Seafood",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    },
                    new IngredientTag
                    {
                   
                        Name = "Vegetable",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    }
                );
            builder.Entity<Message>()
       .HasOne(m => m.FromUser)
       .WithMany(u => u.SentMessages)
       .HasForeignKey(m => m.FromUserId)
       .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                .HasOne(m => m.ToUser)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ToUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                .HasOne(m => m.RepliedTo)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.RepliedToMessageId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RecipeViewHistory>()
       .HasOne(h => h.AppUser)
       .WithMany(u => u.RecipeViewHistories)
       .HasForeignKey(h => h.UserID)
       .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RecipeViewHistory>()
                .HasOne(h => h.ExpertRecipe)
                .WithMany(r => r.ViewHistories)
                .HasForeignKey(h => h.ExpertRecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        }









        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:foodhaven.database.windows.net,1433;Initial Catalog=FoodHaven;Persist Security Info=False;User ID=giahuy;Password=Xinchao123@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");


    }
}
