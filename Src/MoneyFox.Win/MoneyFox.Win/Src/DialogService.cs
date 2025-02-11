namespace MoneyFox.Win;

using System;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Core.Resources;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Pages.Dialogs;

public class DialogService : IDialogService
{
    private LoadingDialog? loadingDialog;

    /// <summary>
    ///     Show a dialog with two buttons with customizable Texts. Returns the answer.
    /// </summary>
    /// <param name="title">Title for the dialog.</param>
    /// <param name="message">Text for the dialog.</param>
    /// <param name="positiveButtonText">Text for the yes button.</param>
    /// <param name="negativeButtonText">Text for the no button.</param>
    public async Task<bool> ShowConfirmMessageAsync(string title, string message, string? positiveButtonText = null, string? negativeButtonText = null)
    {
        await HideLoadingDialogAsync();
        var dialog = new ContentDialog
        {
            XamlRoot = MainWindow.RootFrame.XamlRoot,
            Title = title,
            Content = message,
            PrimaryButtonText = positiveButtonText ?? Strings.YesLabel,
            SecondaryButtonText = negativeButtonText ?? Strings.NoLabel
        };

        var result = await dialog.ShowAsync();

        return result == ContentDialogResult.Primary;
    }

    /// <summary>
    ///     Shows a dialog with title and message. Contains only an OK button.
    /// </summary>
    /// <param name="title">Title to display.</param>
    /// <param name="message">Text to display.</param>
    public async Task ShowMessageAsync(string title, string message)
    {
        await HideLoadingDialogAsync();
        var dialog = new ContentDialog
        {
            XamlRoot = MainWindow.RootFrame.XamlRoot,
            Title = title,
            Content = message,
            PrimaryButtonText = Strings.OkLabel
        };
        await dialog.ShowAsync();
    }

    /// <summary>
    ///     Shows a loading Dialog.
    /// </summary>
    public async Task ShowLoadingDialogAsync(string? message = null)
    {
        await HideLoadingDialogAsync();
        loadingDialog = new() { Text = message ?? Strings.LoadingLabel };
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        dispatcherQueue.TryEnqueue(async () => { await loadingDialog.ShowAsync(); });
    }

    /// <summary>
    ///     Hides the previously opened Loading Dialog.
    /// </summary>
    public Task HideLoadingDialogAsync()
    {
        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        dispatcherQueue.TryEnqueue(
            () =>
            {
                loadingDialog?.Hide();
            });

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Returns the currently open ContentDialog object if one exists, null otherwise.
    /// </summary>
    public static ContentDialog? GetOpenContentDialog()
    {
        foreach (var popup in VisualTreeHelper.GetOpenPopupsForXamlRoot(MainWindow.RootFrame.XamlRoot))
        {
            if (popup.Child is ContentDialog child)
            {
                return child;
            }
        }

        return null;
    }

    /// <summary>
    ///     Hides the open ContentDialog object, if one exists, so another dialog can be displayed.
    /// </summary>
    public static void HideContentDialog(ContentDialog? contentDialog)
    {
        if (contentDialog != null)
        {
            contentDialog.Hide();
        }
    }

    /// <summary>
    ///     Restores visibilty of the ContentDialog object that was previously hidden
    /// </summary>
    public static async Task ShowContentDialog(ContentDialog? contentDialog)
    {
        if (contentDialog != null)
        {
            await contentDialog.ShowAsync();
        }
    }
}
