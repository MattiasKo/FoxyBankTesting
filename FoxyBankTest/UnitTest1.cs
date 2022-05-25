using FoxyBank;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoxyBankTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestLogin()
        {
            //arrange
            User LoginUser = new User("Isak", "Jensen", "Hemlis123", 2001);
            //act
            var actual = LoginUser.Authentication(LoginUser.PassWord, LoginUser.UserId);
            var expected = true;
            //assert
            Assert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void Bank_User_And_Bank_Matching()
        {
            //arrange
            Bank bank = new Bank();
            User user = new User("Isak", "Jensen", "Hemlis123", 2001);
            user.BankAccounts.Add(new PersonalAccount(10000));
            bank.BankAccounts.Add(10000, 2001);
            //act
            var expected = 2001;
            var actual = user.UserId;
            var expected2 = 10000;
            var actual2 = user.BankAccounts[0].AccountNr;
            //assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(expected2, actual2);

        }
        [TestMethod]
        public void Bank_User_Transfer_Between_Accounts()
        {
            //arrange
            
            Bank bank = new Bank();
            User user = new User("Isak", "Jensen", "Hemlis123", 2001);
            user.BankAccounts.Add(new PersonalAccount(10000));
            bank.BankAccounts.Add(10000, 2001);
            user.BankAccounts[0].AccountName = "Personkonto";
            user.BankAccounts[0].CurrencySign = "kr";
            user.BankAccounts[0].AddBalance(10000);
            bank.CurrencyExRate.Add("USD", 9.11m);

            user.BankAccounts.Add(new PersonalAccount(10001));
            bank.BankAccounts.Add(10001, 2001);
            user.BankAccounts[1].AddBalance(10000);
            user.BankAccounts[1].CurrencySign = "kr";
            user.BankAccounts[1].AccountName = "Personkonto";
            bank.Persons.Add(user);

            ForeignAccount f1 = new ForeignAccount(10003);
            f1.CurrencySign = "$";
            user.BankAccounts.Add(f1);
            bank.BankAccounts.Add(10003, 2001);
            user.BankAccounts[2].AccountName = "Konto i Amerikanska dollar";
            user.BankAccounts[2].AddBalance(10000);
            //act 1
            var actual1 = user.BankAccounts[0].AccountName;
            var expected1 = "Personkonto";

            user.BankAccounts[1].AddBalance(10000);
            var actual2 = user.BankAccounts[1].GetBalance();
            var expected2 = 20000;


            user.BankAccounts[0].SubstractBalance(10000);
            var actual3 = user.BankAccounts[0].GetBalance();
            var expected3 = 0;

            user.BankAccounts[2].AddBalance((10000 / bank.CurrencyExRate["USD"]));
            string actual4 = string.Format("{0:0.00}", user.BankAccounts[2].GetBalance());
            string expected4 = "11097,69";
            //assert
            Assert.AreEqual(actual1, expected1);
            Assert.AreEqual(actual2, expected2);
            Assert.AreEqual(actual3, expected3);
            Assert.AreEqual(actual4, expected4);

        }
       
    }
}
