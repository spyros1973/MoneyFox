﻿namespace MoneyFox.Win.ViewModels;

using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Core.Common.Extensions;
using Core.Common.Messages;

internal sealed class SelectDateRangeDialogViewModel : BaseViewModel
{
    private DateTime endDate;
    private DateTime startDate;

    public SelectDateRangeDialogViewModel()
    {
        StartDate = new(year: DateTime.Today.Year, month: DateTime.Today.Month, day: 1);
        EndDate = DateTime.Today.GetLastDayOfMonth();
    }

    /// <summary>
    ///     Start Date for the custom date range
    /// </summary>
    public DateTime StartDate
    {
        get => startDate;

        set
        {
            startDate = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     End Date for the custom date range
    /// </summary>
    public DateTime EndDate
    {
        get => endDate;

        set
        {
            endDate = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     Selects the dates and notifies observer via the MessageHub
    /// </summary>
    public RelayCommand DoneCommand => new(Done);

    private void Done()
    {
        Messenger.Send(new DateSelectedMessage(startDate: StartDate, endDate: EndDate));
    }
}
