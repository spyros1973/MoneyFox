﻿namespace MoneyFox.Tests.Presentation.ViewModels.Budget
{

    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using MediatR;
    using MoneyFox.Core.ApplicationCore.Queries.BudgetLoading;
    using MoneyFox.Core.ApplicationCore.UseCases.BudgetUpdate;
    using MoneyFox.Core.Common.Extensions;
    using MoneyFox.Core.Common.Messages;
    using MoneyFox.Core.Interfaces;
    using MoneyFox.ViewModels.Budget;
    using NSubstitute;
    using TestFramework;
    using Views.Categories;
    using Xunit;

    public class EditBudgetViewModelShould
    {
        private const int CATEGORY_ID = 10;
        private readonly INavigationService navigationService;
        private readonly ISender sender;

        private readonly EditBudgetViewModel viewModel;

        protected EditBudgetViewModelShould()
        {
            sender = Substitute.For<ISender>();
            navigationService = Substitute.For<INavigationService>();
            viewModel = new EditBudgetViewModel(sender: sender, navigationService: navigationService);
        }

        public class OnReceiveMessage : EditBudgetViewModelShould
        {
            [Fact]
            public void AddsSelectedCategoryToList()
            {
                // Act
                var categorySelectedMessage = new CategorySelectedMessage(new CategorySelectedDataSet(categoryId: CATEGORY_ID, name: "Beer"));
                viewModel.Receive(categorySelectedMessage);

                // Assert
                viewModel.SelectedCategories.Should().HaveCount(1);
                viewModel.SelectedCategories.Should().Contain(c => c.CategoryId == CATEGORY_ID);
            }

            [Fact]
            public void IgnoresSelectedCategory_WhenEntryWithSameIdAlreadyInList()
            {
                // Act
                var categorySelectedMessage = new CategorySelectedMessage(new CategorySelectedDataSet(categoryId: CATEGORY_ID, name: "Beer"));
                viewModel.Receive(categorySelectedMessage);
                viewModel.Receive(categorySelectedMessage);

                // Assert
                viewModel.SelectedCategories.Should().HaveCount(1);
                viewModel.SelectedCategories.Should().Contain(c => c.CategoryId == CATEGORY_ID);
            }
        }

        public class OnRemoveCategory : EditBudgetViewModelShould
        {
            [Fact]
            public void Removes_SelectedCategory_OnCommand()
            {
                // Arrange
                var budgetCategoryViewModel = new BudgetCategoryViewModel(categoryId: 1, name: "test");
                viewModel.SelectedCategories.Add(budgetCategoryViewModel);

                // Act
                viewModel.RemoveCategoryCommand.Execute(budgetCategoryViewModel);

                // Assert
                viewModel.SelectedCategories.Should().BeEmpty();
            }
        }

        public class OnOpenCategorySelection : EditBudgetViewModelShould
        {
            [Fact]
            public async Task CallNavigationToCategorySelection()
            {
                // Act
                await viewModel.OpenCategorySelectionCommand.ExecuteAsync(null);

                // Assert
                await navigationService.Received(1).OpenModal<SelectCategoryPage>();
            }
        }

        public class OnSaveBudget : EditBudgetViewModelShould
        {
            [Fact]
            public async Task SendsCorrectSaveCommand()
            {
                // Capture
                UpdateBudget.Command? capturedCommand = null;
                await sender.Send(Arg.Do<UpdateBudget.Command>(q => capturedCommand = q));

                // Arrange
                var testBudget = new TestData.DefaultBudget();
                viewModel.SelectedBudget.Name = testBudget.Name;
                viewModel.SelectedBudget.SpendingLimit = testBudget.SpendingLimit;

                // Act
                viewModel.SelectedCategories.AddRange(testBudget.Categories.Select(c => new BudgetCategoryViewModel(categoryId: c, name: "Category")));
                await viewModel.SaveBudgetCommand.ExecuteAsync(null);

                // Assert
                capturedCommand.Should().NotBeNull();
                capturedCommand!.Name.Should().Be(testBudget.Name);
                capturedCommand.SpendingLimit.Should().Be(testBudget.SpendingLimit);
                capturedCommand.Categories.Should().BeEquivalentTo(testBudget.Categories);
                await navigationService.Received(1).GoBackFromModal();
            }
        }

        public class OnInitialize : EditBudgetViewModelShould
        {
            [Fact]
            public async Task SendCorrectLoadingCommand()
            {
                // Capture
                var testBudget = new TestData.DefaultBudget();
                var categories = testBudget.Categories.Select(c => new BudgetData.BudgetCategory(id: c, name: "category")).ToImmutableList();
                LoadBudget.Query? capturedQuery = null;
                sender.Send(Arg.Do<LoadBudget.Query>(q => capturedQuery = q))
                      .Returns(new BudgetData(id: testBudget.Id, name: testBudget.Name, spendingLimit: testBudget.SpendingLimit, categories: categories));

                // Arrange

                // Act
                await viewModel.InitializeCommand.ExecuteAsync(testBudget.Id);

                // Assert
                capturedQuery.Should().NotBeNull();
                capturedQuery!.BudgetId.Should().Be(testBudget.Id);
                viewModel.SelectedBudget.Id.Should().Be(testBudget.Id);
                viewModel.SelectedBudget.Name.Should().Be(testBudget.Name);
                viewModel.SelectedBudget.SpendingLimit.Should().Be(testBudget.SpendingLimit);

                viewModel.SelectedCategories[0].CategoryId.Should().Be(categories[0].Id);
                viewModel.SelectedCategories[0].Name.Should().Be(categories[0].Name);
            }
        }

        public class OnDelete : EditBudgetViewModelShould
        {
            // [Fact]
            // public async Task SendsCorrectDeleteCommand()
            // {
            //     // Capture
            //     DeleteBudget.Command? capturedCommand = null;
            //     await sender.Send(Arg.Do<DeleteBudget.Command>(q => capturedCommand = q));
            //
            //     // Arrange
            //     //viewModel.SelectedBudget = new BudgetViewModel { Id = 10 };
            //
            //     // Act
            //     await viewModel.DeleteBudgetCommand.ExecuteAsync(null);
            //
            //     // Assert
            //     capturedCommand.Should().NotBeNull();
            //     capturedCommand!.BudgetId.Should().Be(budgetViewModel.Id);
            // }
        }
    }

}
