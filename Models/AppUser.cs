using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppUser:IdentityUser
    {
        public DateTime? JoinedDate { get; set; } = DateTime.Now;
        public DateTime LastAccess { get; set; } = DateTime.Now;
        [StringLength(20)]
        public string? FirstName { get; set; } = default;
        [StringLength(20)]
        public string? LastName  { get; set; } = default;
        public DateTime? Birthday { get; set; }
        [StringLength(200)]
        public string? Address { get; set; } = default;
        [StringLength(10)]
        public string? RequestSeller { get; set; } = "0";
        public string? ImageUrl { get; set; } = "~/assets/imgs/theme/icons/icon-user.svg";
        public bool IsProfileUpdated { get; set; } = false;
        public bool IsBannedByAdmin { get; set; } = false;
        public DateTime? ModifyUpdate { get; set; } = DateTime.Now;
        [StringLength(500)]
        public string? RejectNote { get; set; }
        public virtual StoreDetails StoreDetails { get; set; }
        public ICollection<BalanceChange> BalanceChanges { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
        public ICollection<Cart> Carts { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
        public ICollection<RecipeViewHistory> RecipeViewHistories { get; set; }

    }
}
