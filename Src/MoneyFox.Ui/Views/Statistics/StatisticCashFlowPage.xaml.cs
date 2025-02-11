﻿namespace MoneyFox.Ui.Views.Statistics;

using CommunityToolkit.Maui.Views;
using Popups;
using ViewModels.Statistics;

public partial class StatisticCashFlowPage
{
    public StatisticCashFlowPage()
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<StatisticCashFlowViewModel>();
        ViewModel.LoadedCommand.Execute(null);
    }

    private StatisticCashFlowViewModel ViewModel => (StatisticCashFlowViewModel)BindingContext;

    private void OpenFilterDialog(object sender, EventArgs e)
    {
        var popup = new DateSelectionPopup(dateFrom: ViewModel.StartDate, dateTo: ViewModel.EndDate);
        Shell.Current.ShowPopup(popup);
    }
}
