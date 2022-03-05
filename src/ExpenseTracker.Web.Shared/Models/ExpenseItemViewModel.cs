using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.Shared.Models;

public class ExpenseItemViewModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public bool IsAcquired { get; set; }
}
