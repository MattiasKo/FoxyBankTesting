﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace FoxyBank
{

    public class User : Person
    {

        public List<BankAccount> BankAccounts { get; set; }

        public User(string firstName, string lastName, string passWord, int userId)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.PassWord = passWord;
            this.UserId = userId;
            this.BankAccounts = new List<BankAccount>();
        }

        public void DisplayAllAccounts()
        {
            if (BankAccounts.Count == 0)
            {
                Console.WriteLine("Inga tillgängliga konton.");
            }
            else
            {
                foreach (BankAccount created in BankAccounts)
                {
                    if (created is SavingAccount)
                    {
                        SavingAccount S = (SavingAccount)created;
                        Console.WriteLine($"Kontonamn: {S.AccountName} " +
                                   $"\nKontonummer: {S.AccountNr} " +
                                   $"\nTillgängligt belopp: {S.GetBalance()}" +
                                    $" Ränta = { string.Format("{0:0.00}", S.GetInterest() * S.GetBalance()):f2}" + ". Räntan ligger på " + S.GetInterest() + "%." +
                                    $"\n");
                    }
                    else
                    {
                        Console.WriteLine($"Kontonamn: {created.AccountName} " +
                                        $"\nKontonummer: {created.AccountNr} " +
                                        $"\nTillgängligt belopp: {created.GetBalance()} {created.CurrencySign}" +
                                        $"\n");
                    }
                }
            }
        }
    }
}

