﻿namespace MoneyFox.Ui.Converter;

using System.Globalization;
using MoneyFox.Core.ApplicationCore.Domain.Aggregates.AccountAggregate;
using MoneyFox.Core.Resources;

public class PaymentTypeStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var paymentType = (PaymentType)Enum.ToObject(enumType: typeof(PaymentType), value: value);

        return paymentType switch
        {
            PaymentType.Expense => Strings.ExpenseLabel,
            PaymentType.Income => Strings.IncomeLabel,
            PaymentType.Transfer => Strings.TransferLabel,
            _ => string.Empty
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
