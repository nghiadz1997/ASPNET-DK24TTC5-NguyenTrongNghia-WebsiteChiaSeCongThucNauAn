using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeWebsite.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public string Role { get; set; } = "Member"; // Phân quyền: Admin, Member

        // Mối quan hệ 1-N: 1 User có nhiều Recipe và Comment
        public virtual ICollection<Recipe> Recipes { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(100)]
        public string CategoryName { get; set; }

        // Mối quan hệ 1-N: 1 Danh mục có nhiều Công thức
        public virtual ICollection<Recipe> Recipes { get; set; }
    }

    public class Recipe
    {
        [Key]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Tên món ăn không được để trống")]
        [StringLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int PrepTime { get; set; } // Thời gian chuẩn bị (Phút)
        public int CookTime { get; set; } // Thời gian nấu (Phút)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Khóa ngoại liên kết với bảng User
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        // Khóa ngoại liên kết với bảng Category
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        // Mối quan hệ 1-N: 1 Công thức có nhiều Nguyên liệu, Bước làm, Bình luận
        public virtual ICollection<Ingredient> Ingredients { get; set; }
        public virtual ICollection<Step> Steps { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }

    public class Ingredient
    {
        [Key]
        public int IngredientId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Quantity { get; set; } // Ví dụ: 200g, 1 muỗng cafe...

        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }

    public class Step
    {
        [Key]
        public int StepId { get; set; }

        public int StepNumber { get; set; } // Thứ tự bước (1, 2, 3...)

        [Required]
        public string Instruction { get; set; } // Nội dung hướng dẫn

        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }

    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; } // Đánh giá từ 1 đến 5 sao

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int RecipeId { get; set; }
        [ForeignKey("RecipeId")]
        public virtual Recipe Recipe { get; set; }
    }
}