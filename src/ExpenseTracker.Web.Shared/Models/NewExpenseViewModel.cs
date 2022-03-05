﻿using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Web.Shared.Models;

public class NewExpenseViewModel
{
    [Required]
    public DateTime ExpenseDate { get; set; } = DateTime.Today;

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public decimal TotalAmount { get; set; } = 0;

    public List<ExpenseItemViewModel> Items { get; set; } = new();
}
