﻿namespace MoneyFox.Core.ApplicationCore.Queries.Statistics.GetCategorySummary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using _Pending_.Common.QueryObjects;
using Common.Interfaces;
using Domain.Aggregates.AccountAggregate;
using Domain.Aggregates.CategoryAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;

public class GetCategorySummaryQuery : IRequest<CategorySummaryModel>
{
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}

public class GetCategorySummaryQueryHandler : IRequestHandler<GetCategorySummaryQuery, CategorySummaryModel>
{
    private const int PERCENTAGE_DIVIDER = 100;
    private const int DAY_DIVIDER = 30;
    private const int NUMBERS_OF_MONTHS_TO_LOAD = -12;
    private const decimal DECIMAL_DELTA = 0.1m;
    private const int POSITIONS_TO_ROUND = 2;

    private readonly IAppDbContext appDbContext;
    private List<CategoryOverviewItem> categoryOverviewItems = new();

    private List<Payment> paymentLastTwelveMonths = new();

    public GetCategorySummaryQueryHandler(IAppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<CategorySummaryModel> Handle(GetCategorySummaryQuery request, CancellationToken cancellationToken)
    {
        categoryOverviewItems = new List<CategoryOverviewItem>();
        paymentLastTwelveMonths = await appDbContext.Payments.Include(x => x.Category)
            .Where(x => x.Date.Date >= DateTime.Today.AddMonths(NUMBERS_OF_MONTHS_TO_LOAD))
            .WithoutTransfers()
            .ToListAsync(cancellationToken);

        List<Payment> paymentsInTimeRange = await appDbContext.Payments.Include(x => x.Category)
            .HasDateLargerEqualsThan(request.StartDate.Date)
            .HasDateSmallerEqualsThan(request.EndDate.Date)
            .Where(x => x.Type != PaymentType.Transfer)
            .ToListAsync(cancellationToken);

        foreach (Category? category in paymentsInTimeRange.Where(x => x.Category != null).Select(x => x.Category!).Distinct())
        {
            CreateOverviewItem(payments: paymentsInTimeRange, category: category);
        }

        AddEntryForPaymentsWithoutCategory(paymentsInTimeRange);
        CalculatePercentage(categoryOverviewItems);
        StatisticUtilities.RoundStatisticItems(categoryOverviewItems);

        return new CategorySummaryModel(
            totalEarned: Convert.ToDecimal(paymentsInTimeRange.Where(x => x.Type == PaymentType.Income).Sum(x => x.Amount)),
            totalSpent: Convert.ToDecimal(paymentsInTimeRange.Where(x => x.Type == PaymentType.Expense).Sum(x => x.Amount)),
            categoryOverviewItems: categoryOverviewItems.Where(x => Math.Abs(x.Value) > DECIMAL_DELTA).OrderBy(x => x.Value).ToList());
    }

    private void CreateOverviewItem(IEnumerable<Payment> payments, Category category)
    {
        CategoryOverviewItem categoryOverViewItem = new()
        {
            CategoryId = category.Id,
            Label = category.Name,
            Value = payments.Where(x => x.Category != null)
                .Where(x => x.Category!.Id == category.Id)
                .Where(x => x.Type != PaymentType.Transfer)
                .Sum(x => x.Type == PaymentType.Expense ? -x.Amount : x.Amount),
            Average = CalculateAverageForCategory(category.Id)
        };

        categoryOverviewItems.Add(categoryOverViewItem);
    }

    private void AddEntryForPaymentsWithoutCategory(IEnumerable<Payment> payments)
    {
        categoryOverviewItems.Add(
            new CategoryOverviewItem
            {
                Label = Strings.NoCategoryLabel,
                Value = payments.Where(x => x.Category == null)
                    .Where(x => x.Type != PaymentType.Transfer)
                    .Sum(x => x.Type == PaymentType.Expense ? -x.Amount : x.Amount),
                Average = CalculateAverageForPaymentsWithoutCategory()
            });
    }

    private static void CalculatePercentage(IEnumerable<CategoryOverviewItem> categories)
    {
        decimal sumNegative = categories.Where(x => x.Value < 0).Sum(x => x.Value);
        decimal sumPositive = categories.Where(x => x.Value > 0).Sum(x => x.Value);
        foreach (CategoryOverviewItem? statisticItem in categories.Where(x => x.Value < 0))
        {
            statisticItem.Percentage = statisticItem.Value / sumNegative * PERCENTAGE_DIVIDER;
        }

        foreach (CategoryOverviewItem? statisticItem in categories.Where(x => x.Value > 0))
        {
            statisticItem.Percentage = statisticItem.Value / sumPositive * PERCENTAGE_DIVIDER;
        }
    }

    private decimal CalculateAverageForCategory(int id)
    {
        List<Payment> payments = paymentLastTwelveMonths.Where(x => x.Category != null).Where(x => x.Category!.Id == id).OrderByDescending(x => x.Date).ToList();
        return payments.Count == 0 ? 0 : AverageSumFor(payments);
    }

    private decimal CalculateAverageForPaymentsWithoutCategory()
    {
        List<Payment> payments = paymentLastTwelveMonths.Where(x => x.Category == null).OrderByDescending(x => x.Date).ToList();

        return payments.Count == 0 ? 0 : AverageSumFor(payments);
    }

    private static decimal AverageSumFor(IEnumerable<Payment> payments)
    {
        decimal totalSumForPayments = payments.Sum(x => x.Amount);
        TimeSpan timeDiff = DateTime.Today - DateTime.Today.AddYears(-1);

        return timeDiff.Days < DAY_DIVIDER ? totalSumForPayments : CalculateAverageByMonth(totalSumForPayments: totalSumForPayments, timeDiff: timeDiff);
    }

    private static decimal CalculateAverageByMonth(decimal totalSumForPayments, TimeSpan timeDiff)
    {
        // ReSharper disable once PossibleLossOfFraction
        return Math.Round(d: totalSumForPayments / (timeDiff.Days / DAY_DIVIDER), decimals: POSITIONS_TO_ROUND, mode: MidpointRounding.ToEven);
    }
}

