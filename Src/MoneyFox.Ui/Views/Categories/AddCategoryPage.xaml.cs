﻿namespace MoneyFox.Ui.Views.Categories;

using MoneyFox.Core.Resources;
using ViewModels.Categories;

public partial class AddCategoryPage
{
    public AddCategoryPage()
    {
        InitializeComponent();
        BindingContext = App.GetViewModel<AddCategoryViewModel>();
        var cancelItem = new ToolbarItem
        {
            Command = new Command(async () => await Navigation.PopModalAsync()),
            Text = Strings.CancelLabel,
            Priority = -1,
            Order = ToolbarItemOrder.Primary
        };

        var saveItem = new ToolbarItem
        {
            Command = new Command(() => ViewModel.SaveCommand.Execute(null)),
            Text = Strings.SaveLabel,
            Priority = 1,
            Order = ToolbarItemOrder.Primary
        };

        ToolbarItems.Add(cancelItem);
        ToolbarItems.Add(saveItem);
    }

    private AddCategoryViewModel ViewModel => (AddCategoryViewModel)BindingContext;
}
