using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Grubhub.Models
{
    public class CustomerOrder
    { 
            [Key]
            public int commentId { get; set; }

            [Required]
            public int CustomerId { get; set; }
            [Required]
            public int PostId { get; set; } // ID of the post that the customer is ordering from

            [Required]
            public string FoodName { get; set; }

            [Required]
            public int NumBoxes { get; set; }

            [Required]
            public decimal EstimatedTotalPrice { get; set; }

            [Required]
            public string PickupPlace { get; set; }
            [AllowNull]
            public string? Notes { get; set; }

            [Required]
            public string CustomerName { get; set; }



    }
}
