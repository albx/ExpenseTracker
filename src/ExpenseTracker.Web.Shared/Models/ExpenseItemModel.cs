using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.Shared.Models;

public class ExpenseItemModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
