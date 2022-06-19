﻿namespace MoneyFox.ViewModels.Budget
{

    using System.Linq;
    using System.Threading.Tasks;
    using CommunityToolkit.Mvvm.Input;
    using Core.ApplicationCore.Queries.BudgetLoading;
    using Core.ApplicationCore.UseCases.BudgetDeletion;
    using Core.ApplicationCore.UseCases.BudgetUpdate;
    using Core.Common.Extensions;
    using Core.Interfaces;
    using MediatR;

    internal sealed class EditBudgetViewModel : ModifyBudgetViewModel
    {
        private readonly ISender sender;
        private readonly INavigationService navigationService;

        public EditBudgetViewModel(ISender sender, INavigationService navigationService) : base(navigationService)
        {
            this.sender = sender;
            this.navigationService = navigationService;
        }

        public AsyncRelayCommand<int> InitializeCommand => new AsyncRelayCommand<int>(InitializeAsync);

        public AsyncRelayCommand DeleteBudgetCommand => new AsyncRelayCommand(DeleteBudgetAsync);

        private async Task InitializeAsync(int budgetId)
        {
            var query = new LoadBudget.Query(budgetId: budgetId);
            var budgetData = await sender.Send(query);
            SelectedBudget.Id = budgetData.Id;
            SelectedBudget.Name = budgetData.Name;
            SelectedBudget.SpendingLimit = budgetData.SpendingLimit;

            SelectedCategories.AddRange(budgetData.Categories.Select(bc => new BudgetCategoryViewModel(bc.Id, bc.Name)));
        }

        private async Task DeleteBudgetAsync()
        {
            var command = new DeleteBudget.Command(budgetId: SelectedBudget.Id);
            await sender.Send(command);
        }

        protected override async Task SaveBudgetAsync()
        {
            var command = new UpdateBudget.Command(
                budgetId: SelectedBudget.Id,
                name: SelectedBudget.Name,
                spendingLimit: SelectedBudget.SpendingLimit,
                categories: SelectedCategories.Select(sc => sc.CategoryId).ToList());

            await sender.Send(command);
            await navigationService.GoBackFromModal();
        }
    }

}
