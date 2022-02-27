﻿namespace MoneyFox.Converter
{
    using Core._Pending_;
    using System;
    using System.Globalization;
    using Xamarin.Forms;

    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is decimal decimalValue
                ? decimalValue.ToString(CultureHelper.CurrentLocale)
                : value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(decimal.TryParse(value as string, NumberStyles.Currency, CultureHelper.CurrentLocale, out decimal dec))
            {
                return dec;
            }

            return value;
        }
    }
}