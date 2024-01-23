﻿using Expense_Tracker.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Expense_Tracker.Models;
using System.Dynamic;

namespace Expense_Tracker.Controllers.Dashboard
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public async Task<IActionResult> Index()
        {
            //Last 7 Days
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            //List<Transaction> SelectedTransactions =await _context.Transactions.
            //    Include(x => x.Category).
            //    Where(y => y.Date>=StartDate &&  y.Date <= EndDate).
            //    ToListAsync();

            var TransactionList = await _context.Transactions.
                Include(x => x.Category).
                ToListAsync();

            //Total Income
            int TotalIncome = TransactionList.
                Where(i => i.Category.Type == "Income").
                Sum(j => j.Amount);
            ViewBag.TotalIncome = TotalIncome; 


            //Total Expense
            int TotalExpense = TransactionList.
                Where(i => i.Category.Type == "Expense").
                Sum(j => j.Amount);
            ViewBag.TotalExpense = TotalExpense.ToString("C0");


            //Total Balance
            int Balance = TotalIncome-TotalExpense;
            ViewBag.Balance = Balance.ToString("C0");


			ViewBag.DoughnutChartData = TransactionList
			   .Where(i => i.Category.Type == "Expense")
			   .GroupBy(j => j.Category.CategoryId)
			   .Select(k => new
			   {
				   categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
				   amount = k.Sum(j => j.Amount),
				   formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
			   })
			   .OrderByDescending(l => l.amount)
			.ToList();



			//Spline Chart - Income vs Expense

			//Income
			List<SplineChartData> IncomeSummary = TransactionList
				.Where(i => i.Category.Type == "Income")
				.GroupBy(j => j.Date)
				.Select(k => new SplineChartData()
				{
					day = k.First().Date.ToString("dd-MMM"),
					income = k.Sum(l => l.Amount)
				})
				.ToList();

			//Expense
			List<SplineChartData> ExpenseSummary = TransactionList
				.Where(i => i.Category.Type == "Expense")
				.GroupBy(j => j.Date)
				.Select(k => new SplineChartData()
				{
					day = k.First().Date.ToString("dd-MMM"),
					expense = k.Sum(l => l.Amount)
				})
				.ToList();

			//Combine Income & Expense
			string[] Last7Days = Enumerable.Range(0, 7)
				.Select(i => StartDate.AddDays(i).ToString("dd-MMM"))
				.ToArray();

			ViewBag.SplineChartData = from day in Last7Days
									  join income in IncomeSummary on day equals income.day into dayIncomeJoined
									  from income in dayIncomeJoined.DefaultIfEmpty()
									  join expense in ExpenseSummary on day equals expense.day into expenseJoined
									  from expense in expenseJoined.DefaultIfEmpty()
									  select new
									  {
										  day = day,
										  income = income == null ? 0 : income.income,
										  expense = expense == null ? 0 : expense.expense,
									  };
			//Recent Transactions
			ViewBag.RecentTransactions = await _context.Transactions
	.Include(t => t.Category)
	.OrderByDescending(t => t.Date)
	.Take(5)
	.ToListAsync();



			return View();









		}
		
	}

	public class SplineChartData
	{
		public string day;
		public int income;
		public int expense;

	}
}
