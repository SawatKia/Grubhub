using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Grubhub.Models
{
    public class GrabberPost
    {
        [Key]
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string GrabberName { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime Date_Created { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string CanteenName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int MaxQuantity { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public double MaxTotalPrice { get; set; }

        [Required]
        [AllowNull]
        public DateTime? CloseTime { get; set; }

    }
}
