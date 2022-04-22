﻿namespace MoneyFox.Core.Tests._Pending_.QueryObjects
{

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ApplicationCore.Domain.Aggregates.AccountAggregate;
    using Core._Pending_.Common.QueryObjects;
    using Xunit;

    [ExcludeFromCodeCoverage]
    public class PaymentQueryExtensionTests
    {
        [Fact]
        public void HasDateLargerEqualsThan()
        {
            // Arrange
            var paymentListQuery = new List<Payment>
            {
                new Payment(date: DateTime.Now.AddDays(1), amount: 12, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now, amount: 13, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now.AddDays(-1), amount: 14, type: PaymentType.Expense, chargedAccount: new Account("d"))
            }.AsQueryable();

            // Act
            var resultList = paymentListQuery.HasDateLargerEqualsThan(DateTime.Now).ToList();

            // Assert
            Assert.Equal(expected: 2, actual: resultList.Count);
            Assert.Equal(expected: 12, actual: resultList[0].Amount);
            Assert.Equal(expected: 13, actual: resultList[1].Amount);
        }

        [Fact]
        public void IsCleared()
        {
            // Arrange
            var paymentListQuery = new List<Payment>
            {
                new Payment(date: DateTime.Now, amount: 12, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now.AddDays(1), amount: 13, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now.AddDays(1), amount: 14, type: PaymentType.Expense, chargedAccount: new Account("d"))
            }.AsQueryable();

            // Act
            var resultList = paymentListQuery.AreCleared().ToList();

            // Assert
            Assert.Single(resultList);
            Assert.Equal(expected: 12, actual: resultList[0].Amount);
        }

        [Fact]
        public void IsNotCleared()
        {
            // Arrange
            var paymentListQuery = new List<Payment>
            {
                new Payment(date: DateTime.Now, amount: 12, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now.AddDays(1), amount: 13, type: PaymentType.Expense, chargedAccount: new Account("d")),
                new Payment(date: DateTime.Now.AddDays(1), amount: 14, type: PaymentType.Expense, chargedAccount: new Account("d"))
            }.AsQueryable();

            // Act
            var resultList = paymentListQuery.AreNotCleared().ToList();

            // Assert
            Assert.Equal(expected: 2, actual: resultList.Count);
            Assert.Equal(expected: 13, actual: resultList[0].Amount);
            Assert.Equal(expected: 14, actual: resultList[1].Amount);
        }

        [Fact]
        public void WithoutTransfers()
        {
            // Arrange
            var paymentListQuery = new List<Payment>
            {
                new Payment(
                    date: DateTime.Now,
                    amount: 12,
                    type: PaymentType.Expense,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 13,
                    type: PaymentType.Income,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 14,
                    type: PaymentType.Transfer,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 15,
                    type: PaymentType.Income,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 16,
                    type: PaymentType.Transfer,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t"))
            }.AsQueryable();

            // Act
            var resultList = paymentListQuery.WithoutTransfers().ToList();

            // Assert
            Assert.Equal(expected: 3, actual: resultList.Count);
            Assert.Equal(expected: 12, actual: resultList[0].Amount);
            Assert.Equal(expected: 13, actual: resultList[1].Amount);
            Assert.Equal(expected: 15, actual: resultList[2].Amount);
        }

        [Theory]
        [InlineData(PaymentType.Expense, 22)]
        [InlineData(PaymentType.Income, 23)]
        [InlineData(PaymentType.Transfer, 24)]
        public void IsPaymentType(PaymentType paymentType, decimal amount)
        {
            // Arrange
            var paymentListQuery = new List<Payment>
            {
                new Payment(
                    date: DateTime.Now,
                    amount: 12,
                    type: PaymentType.Expense,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 13,
                    type: PaymentType.Income,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: 14,
                    type: PaymentType.Transfer,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t")),
                new Payment(
                    date: DateTime.Now,
                    amount: amount,
                    type: paymentType,
                    chargedAccount: new Account("d"),
                    targetAccount: new Account("t"))
            }.AsQueryable();

            // Act
            var resultList = paymentListQuery.IsPaymentType(paymentType).ToList();
            var seedval = paymentType switch
            {
                PaymentType.Expense => 12,
                PaymentType.Income => 13,
                PaymentType.Transfer => 14,
                _ => 0
            };

            // Assert
            Assert.Equal(expected: 2, actual: resultList.Count);
            Assert.Equal(expected: seedval, actual: resultList[0].Amount);
            Assert.Equal(expected: amount, actual: resultList[1].Amount);
        }
    }

}
