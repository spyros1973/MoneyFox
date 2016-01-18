﻿using MoneyManager.Core.ViewModels;
using MoneyManager.Foundation.Interfaces;
using MoneyManager.Foundation.Model;
using MoneyManager.Localization;
using MoneyManager.TestFoundation;
using Moq;
using Xunit;

namespace MoneyManager.Core.Tests.ViewModels
{
    public class ModifyAccountViewModelTests
    {
        [Fact]
        public void Title_EditAccount_CorrectTitle()
        {
            var accountname = "Sparkonto";

            var accountRepositorySetup = new Mock<IAccountRepository>();
            accountRepositorySetup.SetupGet(x => x.Selected).Returns(new Account {Id = 2, Name = accountname});

            var viewmodel = new ModifyAccountViewModel(accountRepositorySetup.Object, new BalanceViewModel(
                accountRepositorySetup.Object, new Mock<IPaymentRepository>().Object))
            {IsEdit = true};

            viewmodel.Title.ShouldBe(Strings.EditLabel + " " + accountname);
        }

        [Fact]
        public void Title_AddAccount_CorrectTitle()
        {
            var accountname = "Sparkonto";

            var accountRepositorySetup = new Mock<IAccountRepository>();
            accountRepositorySetup.SetupGet(x => x.Selected).Returns(new Account {Id = 2, Name = accountname});

            var viewmodel = new ModifyAccountViewModel(accountRepositorySetup.Object, new BalanceViewModel(
                accountRepositorySetup.Object, new Mock<IPaymentRepository>().Object))
            {IsEdit = false};

            viewmodel.Title.ShouldBe(Strings.AddAccountTitle);
        }
    }
}