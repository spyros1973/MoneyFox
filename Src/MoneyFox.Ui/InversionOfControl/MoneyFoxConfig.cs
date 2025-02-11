namespace MoneyFox.Ui.InversionOfControl;

using Common.Services;
using Infrastructure.Adapters;
using Mapping;
using MoneyFox.Core.Common.Interfaces;
using MoneyFox.Core.Interfaces;
using MoneyFox.Core.InversionOfControl;
using MoneyFox.Infrastructure.InversionOfControl;
using MoneyFox.Ui.Views.About;
using MoneyFox.Ui.Views.Accounts;
using MoneyFox.Ui.Views.Backup;
using MoneyFox.Ui.Views.Dashboard;
using MoneyFox.Ui.Views.OverflowMenu;
using ViewModels.Accounts;
using ViewModels.Budget;
using ViewModels.Categories;
using ViewModels.Dialogs;
using ViewModels.Payments;
using ViewModels.Settings;
using ViewModels.SetupAssistant;
using ViewModels.Statistics;

public sealed class MoneyFoxConfig
{
    public void Register(IServiceCollection serviceCollection)
    {
        RegisterServices(serviceCollection);
        RegisterViewModels(serviceCollection);
        RegisterAdapters(serviceCollection);

        _ = serviceCollection.AddSingleton(_ => AutoMapperFactory.Create());
        new CoreConfig().Register(serviceCollection);
        InfrastructureConfig.Register(serviceCollection);
    }

    private static void RegisterServices(IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddTransient<IDialogService, DialogService>()
            .AddTransient<INavigationService, NavigationService>()
            .AddTransient<IToastService, ToastService>();
    }

    private static void RegisterViewModels(IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddTransient<AboutViewModel>()
            .AddTransient<AccountListViewModel>()
            .AddTransient<AddAccountViewModel>()
            .AddTransient<EditAccountViewModel>()
            .AddTransient<AddCategoryViewModel>()
            .AddTransient<CategoryListViewModel>()
            .AddTransient<EditCategoryViewModel>()
            .AddTransient<SelectCategoryViewModel>()
            .AddTransient<DashboardViewModel>()
            .AddTransient<BackupViewModel>()
            .AddTransient<OverflowMenuViewModel>()
            .AddTransient<AddPaymentViewModel>()
            .AddTransient<EditPaymentViewModel>()
            .AddTransient<PaymentListViewModel>()
            .AddTransient<SettingsViewModel>()
            .AddTransient<CategoryIntroductionViewModel>()
            .AddTransient<SetupCompletionViewModel>()
            .AddTransient<WelcomeViewModel>()
            .AddTransient<PaymentForCategoryListViewModel>()
            .AddTransient<StatisticAccountMonthlyCashFlowViewModel>()
            .AddTransient<StatisticCashFlowViewModel>()
            .AddTransient<StatisticCategoryProgressionViewModel>()
            .AddTransient<StatisticCategorySpreadingViewModel>()
            .AddTransient<StatisticCategorySummaryViewModel>()
            .AddTransient<StatisticSelectorViewModel>()
            .AddTransient<SelectDateRangeDialogViewModel>()
            .AddTransient<SelectFilterDialogViewModel>()
            .AddTransient<AddBudgetViewModel>()
            .AddTransient<EditBudgetViewModel>()
            .AddTransient<BudgetListViewModel>();
    }

    private static void RegisterAdapters(IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddTransient<IBrowserAdapter, BrowserAdapter>()
            .AddTransient<IConnectivityAdapter, ConnectivityAdapter>()
            .AddTransient<IEmailAdapter, EmailAdapter>()
            .AddTransient<ISettingsAdapter, SettingsAdapter>();
    }
}
