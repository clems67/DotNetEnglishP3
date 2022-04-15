using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        [RegularExpression(@"^([1-9][0-9]*)$", ErrorMessage = "stocksth")]
        public string Stock { get; set; }
        [Required(ErrorMessage = "MissingPrice")]
        [RegularExpression(@"^([0-9]+[.|,]?[0-9]*)$", ErrorMessage = "PriceNotANumber")]
        public string Price { get; set; }
    }
}
