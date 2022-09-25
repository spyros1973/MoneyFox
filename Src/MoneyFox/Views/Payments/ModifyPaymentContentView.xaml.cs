﻿namespace MoneyFox.Views.Payments;

public partial class ModifyPaymentContentView
{
    public ModifyPaymentContentView()
    {
        InitializeComponent();
    }

    private void AmountFieldGotFocus(object sender, FocusEventArgs e)
    {
        AmountEntry.CursorPosition = 0;
        AmountEntry.SelectionLength = AmountEntry.Text != null ? AmountEntry.Text.Length : 0;
    }
}
