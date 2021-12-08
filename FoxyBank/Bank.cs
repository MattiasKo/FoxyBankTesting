﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FoxyBank
{
    public class Bank
    {
        public List<Person> Persons { get; set; }
        public Dictionary<int, int> BankAccounts { get; set; }
        public Dictionary<string, decimal> CurrencyExRate { get; private set; }

        public Bank()
        {
            this.Persons = new List<Person>();
            this.BankAccounts = new Dictionary<int, int>();
            this.CurrencyExRate = new Dictionary<string, decimal>() { { "USD", 9.11m }, { "EUR", 10.25m } };
        }

        public void StartApplication()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();
            Console.WriteLine("Hej välkommen till Foxy Bank.");
            Person loggedInPerson = Login();

        }
        public Person Login()
        {
            byte Tries = 3;
            bool Answer = false;
            int AnId;
            do
            {
                do
                {
                    Console.WriteLine("\nSkriv användarID");
                    Answer = int.TryParse(Console.ReadLine(), out AnId);
                    if (Answer == false && Tries != 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Ogiltigt användarID, försök igen.");
                        Tries--;
                    }
                    if (Tries == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Misslyckad inloggning.");


                        return null;
                    }

                } while (Answer == false && Tries != 0);

                Console.WriteLine("Skriv in lösenord");
                string AnPassword = HidePassWord();

                foreach (Person A1 in Persons)
                {
                    if (A1.Authentication(AnPassword, AnId))
                    {
                        Console.WriteLine("\nDu är inloggad.");
                        A1.UpdateLog("Loggat in.");
                        char firstDigit = A1.UserId.ToString()[0];
                        if (firstDigit == '1')              //All users with Admin function has an ID which starts with nr 1
                        {
                            RunAdminMenu((Admin)A1);
                            return null;
                        }
                        else
                        {
                            RunUserMenu((User)A1);
                            return null;
                        }
                    }
                }
                Tries--;
                Console.WriteLine("\nMisslyckad inloggning.");
            } while (Tries != 0);
            return null;
        }
        public string HidePassWord()
        {
            string password = string.Empty;
            ConsoleKey key;
            do
            {
                var keyInfo = Console.ReadKey(intercept: true);
                key = keyInfo.Key;
                if (key == ConsoleKey.Backspace && password.Length > 0)
                {
                    Console.Write("\b \b");
                    password = password[0..^1];
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    Console.Write("*");
                    password += keyInfo.KeyChar;
                }

            } while (key != ConsoleKey.Enter);

            return password;

        }
        public int GenerateUserID()
        {
            bool IDCheck = true;
            Random random = new Random();
            int randomizeID = 0;
            do
            {
                IDCheck = true;
                randomizeID = random.Next(2000, 3000);
                foreach (var item in Persons)
                {
                    if (randomizeID == item.UserId)
                    {

                        IDCheck = false;
                    }

                }
            }
            while (IDCheck == false);

            return randomizeID;

        }
        public void RunAdminMenu(Admin loggedInPerson)
        {
            bool isRunning = true;

            do
            {
                Console.WriteLine($"\nHej {loggedInPerson.FirstName} {loggedInPerson.LastName}. Vad vill du göra?");

                Console.WriteLine("\nAnvändarmeny för administrator:" +
                            "\n1. Skapa ny bankkund" +
                            "\n2. Ändra valutakurs" +
                            "\n3. Ändra sparränta" +
                            "\n4. Visa log" +
                            "\n5. Logga ut" +
                            "\n6. Avsluta programmet");
                string menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        RegisterNewUser(loggedInPerson);
                        break;
                    case "2":
                        CurrencyUpdate(CurrencyExRate["USD"]);
                        Console.WriteLine();
                        break;

                    case "3":
                        //InterestRate();
                        break;

                    case "4":
                        loggedInPerson.DisplayLog();
                        break;

                    case "5":
                        isRunning = false;
                        StartApplication();

                        break;

                    case "6":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }
            while (isRunning != false);
        }
        public void RunUserMenu(User loggedInPerson)

        {
            bool isRunning = true;

            do
            {
                Console.WriteLine($"\nHej {loggedInPerson.FirstName} {loggedInPerson.LastName}. Vad vill du göra:");
                Console.WriteLine("\n1. Se dina konton och saldo" +
                        "\n2. Överför pengar" +
                        "\n3. Skapa nytt bankkonto" +
                        "\n4. Visa log" +
                        "\n5. Sätta in pengar" +
                        "\n6. Logga ut" +
                        "\n7. Avsluta programmet");

                string menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        loggedInPerson.UpdateLog("Visat alla konton.");
                        loggedInPerson.DisplayAllAccounts();
                        break;

                    case "2":
                        TransferMoney(loggedInPerson);
                        break;

                    case "3":
                        CreateAccount(loggedInPerson);
                        break;

                    case "4":
                        loggedInPerson.DisplayLog();
                        break;

                    case "5":
                        DepositMoney(loggedInPerson);
                        break;

                    case "6":
                        loggedInPerson.UpdateLog("Loggat ut.");
                        isRunning = false;
                        StartApplication();
                        break;

                    case "7":
                        loggedInPerson.UpdateLog("Stängt ner programmet.");
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nOgiltigt val.");
                        break;
                }
            }
            while (isRunning != false);
        }
        public void RegisterNewUser(Admin loggedInPerson)
        {


            Console.WriteLine("Var god skriv in användarens förnamn");
            string firstNameInput = Console.ReadLine();
            Console.WriteLine("Var god skriv in användarens efternamn");
            string lastNameInput = Console.ReadLine();
            string passWordInput;
            bool PassHasDigit;
            string passWordCheck;
            do
            {
                do
                {
                    Console.WriteLine("\nVar god skriv in användarens lösenord, Lösenordet måste minst ha 8 bokstäver och ett nummer.");

                    passWordInput = HidePassWord();
                    PassHasDigit = passWordInput.Any(char.IsDigit);
                    if (PassHasDigit == false)
                    {
                        Console.WriteLine("\nLösenordet behöver minst ett nummer.");
                    }
                    if (passWordInput.Length < 8)
                    {
                        Console.WriteLine("\nLösenordet är för kort.");
                    }

                } while (passWordInput.Length < 8 || PassHasDigit == false);
                passWordCheck = passWordInput;


                Console.WriteLine("\nSkriv lösenordet igen.");
                passWordCheck = HidePassWord();
                if (passWordCheck != passWordInput)
                {
                    Console.WriteLine("\nLösenordet är inte samma.");

                }
            } while (passWordCheck != passWordInput);


            User newBankUser = new User(firstNameInput, lastNameInput, passWordInput, GenerateUserID());
            this.Persons.Add(newBankUser);
            newBankUser.UpdateLog("Ditt användar konto har skapats.");
            loggedInPerson.UpdateLog($"Skapat en ny användare. ID : {newBankUser.UserId}");
            Console.Clear();
            Console.WriteLine("Ny användare tillagd.");
            Console.WriteLine("Användarinfo");
            Console.WriteLine("Namn : {0} {1}", newBankUser.FirstName, newBankUser.LastName);
            Console.WriteLine("Lösenord : {0}", newBankUser.PassWord);
            Console.WriteLine("ID : {0}", newBankUser.UserId);
            Console.ReadKey();
        }
        public int GenerateAccountNr()
        {
            int accountNr = 0;

            Random rand = new Random();
            int randomizedAccNr = rand.Next(10000, 11000);

            if (!BankAccounts.ContainsKey(randomizedAccNr)) { accountNr = randomizedAccNr; }
            else
            {
                bool foundId = false;
                while (!foundId)
                {
                    randomizedAccNr = rand.Next(10000, 11000);
                    if (!BankAccounts.ContainsKey(randomizedAccNr))
                    {
                        accountNr = randomizedAccNr;
                        foundId = true;
                    }
                }
            }
            return accountNr;
        }

        public void CreateAccount(User user)
        {
            BankAccount createdAccount = null;
            Console.Clear();

            Console.WriteLine("\nVad vill du öppna för konto?");
            do
            {
                Console.WriteLine("\n1. Sparkonto");
                Console.WriteLine("\n2. Personkonto");
                Console.WriteLine("\n3. Lånekonto");
                Console.WriteLine("\n4. Konto i Amerikanska dollar\n");

                string answer = Console.ReadLine();


                if (answer == "1")
                {
                    createdAccount = new SavingAccount(GenerateAccountNr());

                    user.BankAccounts.Add(createdAccount);
                    this.BankAccounts.Add(createdAccount.AccountNr, user.UserId);
                    createdAccount.AccountName = "Sparkonto";
                    user.UpdateLog("Skapat nytt Sparkonto. Konto Nummer: " + createdAccount.AccountNr);
                    createdAccount.CurrencySign = " kr";

                }

                else if (answer == "2")
                {
                    createdAccount = new PersonalAccount(GenerateAccountNr());
                    user.BankAccounts.Add(createdAccount);
                    this.BankAccounts.Add(createdAccount.AccountNr, user.UserId);
                    createdAccount.AccountName = "Personkonto";
                    user.UpdateLog("Skapat nytt Personkonto. Konto Nummer: " + createdAccount.AccountNr);
                    createdAccount.CurrencySign = " kr";
                }

                else if (answer == "3")
                {
                    createdAccount = new LoanAccount(GenerateAccountNr());
                    user.BankAccounts.Add(createdAccount);
                    this.BankAccounts.Add(createdAccount.AccountNr, user.UserId);
                    createdAccount.AccountName = "Lånekonto";
                    createdAccount.CurrencySign = " kr";
                }

                else if (answer == "4")
                {
                    createdAccount = new ForeignAccount(GenerateAccountNr());
                    user.BankAccounts.Add(createdAccount);
                    this.BankAccounts.Add(createdAccount.AccountNr, user.UserId);
                    createdAccount.AccountName = "Konto i Amerikanska dollar";
                    createdAccount.CurrencySign = "$";
                }

                else
                {
                    Console.WriteLine("Vänligen välj vilket typ av konto du vill öppna. Svara ett nummer från menyn.");
                }

            } while (createdAccount == null);
            if (createdAccount is LoanAccount)
            {
                Console.WriteLine($"\nGrattis! Du har skapat ett " + createdAccount.AccountName + " med kontonummer : " + createdAccount.AccountNr);
            }
            else if (createdAccount is PersonalAccount)
            {
                Console.WriteLine($"\nGrattis! Du har skapat ett " + createdAccount.AccountName + " med kontonummer : " + createdAccount.AccountNr);
            }
            else if (createdAccount is SavingAccount)
            {
                Console.WriteLine($"\nGrattis! Du har skapat ett " + createdAccount.AccountName + " med kontonummer : " + createdAccount.AccountNr);
                SavingAccount S = (SavingAccount)createdAccount;
                Console.Write("Räntan är " + S.GetInterest() + "%");
            }
            else if (createdAccount is ForeignAccount)
            {
                Console.WriteLine($"\nGrattis! Du har skapat ett " + createdAccount.AccountName + " med kontonummer : " + createdAccount.AccountNr);
            }

            Console.WriteLine("\nKlicka enter för att komma vidare.");
            Console.ReadKey();
            Console.Clear();
        }
        public void TransferMoney(User user)
        {
            int transferFromAcc = 0;
            BankAccount fromAcc = null;
            BankAccount toAcc = null;
            Console.Clear();

            if (user.BankAccounts.Count != 0)
            {
                user.DisplayAllAccounts();

                Console.WriteLine("\nVilket konto vill du överföra pengar ifrån? Skriv kontonumret.");
                do
                {
                    int inputAcc = 0;

                    if (int.TryParse(Console.ReadLine(), out inputAcc))
                    {
                        fromAcc = user.BankAccounts.Find(x => x.AccountNr == inputAcc);
                        if (fromAcc != null)
                        {
                            if (fromAcc.GetBalance() > 0)
                            {
                                transferFromAcc = fromAcc.AccountNr;

                            }
                            else
                            {
                                Console.WriteLine("Konto du valde har inte tillräckligt högt saldo. Vänligen välj ett annat konto.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inget konton med det kontonumret du matade in hittades. Vänligen testa att skriva kontonumret igen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vänligen mata in ett korrekt kontonummer. Vänligen testa att skriva kontonumret igen.");
                    }
                } while (transferFromAcc == 0);


                int transferToAcc = 0;
                User transferToUser = null;
                Console.WriteLine("\nVilket konto vill du överföra pengar till? Skriv kontonumret.");

                do
                {
                    int inputAcc = 0;
                    if (int.TryParse(Console.ReadLine(), out inputAcc))
                    {
                        toAcc = user.BankAccounts.Find(x => x.AccountNr == inputAcc);
                        if (transferFromAcc == inputAcc)
                        {
                            Console.WriteLine("Vänligen välj ett annat konto än det du ska överför pengar ifrån.");
                        }
                        else if (this.BankAccounts.ContainsKey(inputAcc))
                        {

                            if (this.BankAccounts[inputAcc] != user.UserId)
                            {
                                transferToUser = (User)this.Persons.Find(x => x.UserId == this.BankAccounts[inputAcc]);

                                Console.WriteLine($"\nKontot du valde med kontonummer {inputAcc} tillhör {transferToUser.FirstName} {transferToUser.LastName}." +
                                    $"\nÄr du säker att du vill överföra pengar till detta konto? Svara \"Ja\" isåfall. Klicka enter för att ändra kontonumret.");

                                if (Console.ReadLine().ToUpper() == "JA")
                                {
                                    transferToAcc = inputAcc;
                                }
                                else
                                {
                                    Console.WriteLine("Vänligen skriv kontonumret på kontot du vill överföra pengar till.");
                                }
                            }
                            else
                            {
                                transferToAcc = inputAcc;
                                transferToUser = user;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inget konton med det kontonumret du matade in hittades. Vänligen testa att skriva kontonumret igen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vänligen mata in ett korrekt kontonummer. Vänligen testa att skriva kontonumret igen.");
                    }
                } while (transferToAcc == 0);


                decimal amountOfMoneyToTransfer = 0;

                int indexOfTransferFromAcc = user.BankAccounts.IndexOf(user.BankAccounts.Find(x => x.AccountNr == transferFromAcc));

                if (fromAcc is ForeignAccount)
                {
                    Console.WriteLine($"\nHur mycket pengar vill du överföra från {transferFromAcc} till {transferToAcc}? " +
                        $"Du har $ {user.BankAccounts[indexOfTransferFromAcc].GetBalance()} tillgängligt.");
                }
                else
                {
                    Console.WriteLine($"\nHur mycket pengar vill du överföra från {transferFromAcc} till {transferToAcc}? " +
                        $"Du har {user.BankAccounts[indexOfTransferFromAcc].GetBalance()} kr tillgängligt.");
                }
                do
                {
                    decimal inputAmount = 0;
                    if (decimal.TryParse(Console.ReadLine(), out inputAmount))
                    {
                        if (inputAmount > 0 & user.BankAccounts[indexOfTransferFromAcc].GetBalance() >= inputAmount)
                        {
                            amountOfMoneyToTransfer = inputAmount;

                        }
                        else
                        {
                            Console.WriteLine("Vänligen mata in en summa som är giltig att överföra. Skriv summan som ska överföras igen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vänligen mata in en summa som är giltig att överföra. Skriv summan som ska överföras igen.");
                    }

                } while (amountOfMoneyToTransfer == 0);

                int triesLeft = 3;
                bool succesfulTransaction = false;

                if (toAcc is ForeignAccount)
                {
                    Console.WriteLine($"\nDu vill överföra {amountOfMoneyToTransfer} kr från kontot med kontonummer {transferFromAcc} till kontonummer {transferToAcc}." +
                        $"\nVänligen mata in ditt lösenord för att genomföra transaktionen.");

                }
                else if (fromAcc is ForeignAccount)
                {
                    Console.WriteLine($"\nDu vill överföra $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc} till kontonummer {transferToAcc}." +
                      $"\nVänligen mata in ditt lösenord för att genomföra transaktionen.");
                }
                else
                {
                    Console.WriteLine($"\nDu vill överföra {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc} till kontonummer {transferToAcc}." +
                    $"\nVänligen mata in ditt lösenord för att genomföra transaktionen.");
                }

                do
                {
                    string input = HidePassWord(); ;

                    if (user.Authentication(input, user.UserId))
                    {
                        user.BankAccounts[indexOfTransferFromAcc].SubstractBalance(amountOfMoneyToTransfer);

                        if (user.UserId == transferToUser.UserId)
                        {
                            int indexOfTransferToAcc = user.BankAccounts.IndexOf(user.BankAccounts.Find(x => x.AccountNr == transferToAcc));

                            if (fromAcc is ForeignAccount && !(toAcc is ForeignAccount))
                            {
                                user.BankAccounts[indexOfTransferToAcc].AddBalance((CurrencyExRate["USD"] * amountOfMoneyToTransfer));
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                user.UpdateLog($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc}" +
                                    $" till {transferToAcc}.");
                                Console.WriteLine($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc}" +
                                    $" till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är " +
                                    $"$ {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2} " +
                                    $"\noch ditt nya saldo på kontot med kontonummer " +
                                    $"{transferToAcc} är {user.BankAccounts[indexOfTransferToAcc].GetBalance():f2} kr.");
                                succesfulTransaction = true;
                            }
                            else if (toAcc is ForeignAccount && !(fromAcc is ForeignAccount))
                            {
                                user.BankAccounts[indexOfTransferToAcc].AddBalance((amountOfMoneyToTransfer / CurrencyExRate["USD"]));
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;

                                Console.WriteLine($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc}" +
                                    $" till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är " +
                                    $"$ {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2} " +
                                    $"\noch ditt nya saldo på kontot med kontonummer " +
                                    $"{transferToAcc} är {user.BankAccounts[indexOfTransferToAcc].GetBalance():f2} kr.");

                                succesfulTransaction = true;
                            }

                            else if (toAcc is ForeignAccount && fromAcc is ForeignAccount)
                            {
                                user.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToTransfer);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;

                                Console.WriteLine($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc}" +
                                    $" till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är " +
                                    $"$ {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2} " +
                                    $"\noch ditt nya saldo på kontot med kontonummer " +
                                    $"{transferToAcc} är $ {user.BankAccounts[indexOfTransferToAcc].GetBalance():f2}.");

                                succesfulTransaction = true;
                            }
                            else
                            {
                                user.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToTransfer);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;

                                Console.WriteLine($"\n\nDu överförde {amountOfMoneyToTransfer} kr från kontot med kontonummer {transferFromAcc}" +
                                    $" till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är " +
                                    $"{user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2} kr " +
                                    $"\noch ditt nya saldo på kontot med kontonummer " +
                                    $"{transferToAcc} är {user.BankAccounts[indexOfTransferToAcc].GetBalance():f2} kr.");

                                succesfulTransaction = true;
                            }
                        }
                        else
                        {
                            int indexOfTransferToAcc = transferToUser.BankAccounts.IndexOf(transferToUser.BankAccounts.Find(x => x.AccountNr == transferToAcc));

                            if (fromAcc is ForeignAccount && !(toAcc is ForeignAccount))
                            {
                                transferToUser.BankAccounts[indexOfTransferToAcc].AddBalance((CurrencyExRate["USD"] * amountOfMoneyToTransfer));
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == transferToUser.UserId))] = transferToUser;

                                Console.WriteLine($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc} till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är $ {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2}");

                                succesfulTransaction = true;
                            }

                            else if (toAcc is ForeignAccount && !(fromAcc is ForeignAccount))
                            {
                                transferToUser.BankAccounts[indexOfTransferToAcc].AddBalance((amountOfMoneyToTransfer / CurrencyExRate["USD"]));
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == transferToUser.UserId))] = transferToUser;

                                Console.WriteLine($"\n\nDu överförde {amountOfMoneyToTransfer} kr från kontot med kontonummer {transferFromAcc} till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2} kr.");

                                succesfulTransaction = true;
                            }

                            else if (toAcc is ForeignAccount && fromAcc is ForeignAccount)
                            {
                                transferToUser.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToTransfer);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == transferToUser.UserId))] = transferToUser;

                                Console.WriteLine($"\n\nDu överförde $ {amountOfMoneyToTransfer} från kontot med kontonummer {transferFromAcc} till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är $ {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2}.");

                                succesfulTransaction = true;
                            }

                            else
                            {
                                transferToUser.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToTransfer);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == transferToUser.UserId))] = transferToUser;

                                Console.WriteLine($"\n\nDu överförde {amountOfMoneyToTransfer}kr från kontot med kontonummer {transferFromAcc} till {transferToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot med kontonummer {transferFromAcc} är {user.BankAccounts[indexOfTransferFromAcc].GetBalance():f2}");

                                succesfulTransaction = true;
                            }
                        }
                    }
                    else
                    {
                        triesLeft--;
                        Console.WriteLine(triesLeft > 0 ? "\nFel lösenord. Försök igen." : "\nTransaktion misslyckades. Du skrev fel lösenord för många gånger.");
                    }

                } while (triesLeft > 0 & !succesfulTransaction);
            }
            else
            {
                user.DisplayAllAccounts();
            }
            Console.WriteLine("\nKlicka enter för att komma vidare.");
            Console.ReadKey();
            Console.Clear();
        }
        public decimal CurrencyUpdate(decimal UpDatedCurr)
        {
            Console.WriteLine($"Aktuell kurs för USD: {CurrencyExRate["USD"]}");

            Console.WriteLine($"Vill du ändra kursen? \n1. Ja \n2. Nej");

            string input = Console.ReadLine().ToUpper();
            if (input == "1" || input == "JA")
            {
                Console.Clear();
                Console.WriteLine("Ange aktuell kurs för 1 USD till SEK: ");
                decimal upDatedCurr = Convert.ToDecimal(Console.ReadLine());

                CurrencyExRate["USD"] = upDatedCurr;
                Console.WriteLine(CurrencyExRate["USD"]);

                Console.Clear();

                Console.WriteLine($"Aktuell kurs för USD: {CurrencyExRate["USD"]}");
                return upDatedCurr;
            }
            else
            {
                return CurrencyExRate["USD"];
            }
        }
        public void DepositMoney(User user)
        {
            BankAccount depositAcc = null;
            Console.Clear();

            if (user.BankAccounts.Count != 0)
            {
                user.DisplayAllAccounts();

                int depositToAcc = 0;
                User depositToUser = null;
                Console.WriteLine("\nVilket konto vill du sätta in pengar på? Skriv kontonumret.");

                do
                {
                    int inputAcc = 0;
                    if (int.TryParse(Console.ReadLine(), out inputAcc))
                    {
                        depositAcc = user.BankAccounts.Find(x => x.AccountNr == inputAcc);

                        if (this.BankAccounts.ContainsKey(inputAcc))
                        {

                            if (this.BankAccounts[inputAcc] != user.UserId)
                            {
                                depositToUser = (User)this.Persons.Find(x => x.UserId == this.BankAccounts[inputAcc]);

                                Console.WriteLine($"\nKontot du valde med kontonummer {inputAcc} tillhör {depositToUser.FirstName} {depositToUser.LastName}." +
                                    $"\nÄr du säker att du vill sätta in pengar på detta konto? Svara \"Ja\" isåfall. Klicka enter för att ändra kontonumret.");

                                if (Console.ReadLine().ToUpper() == "JA")
                                {
                                    depositToAcc = inputAcc;
                                }
                                else
                                {
                                    Console.WriteLine("Vänligen skriv kontonumret på kontot du vill överföra pengar till.");
                                }
                            }
                            else
                            {
                                depositToAcc = inputAcc;
                                depositToUser = user;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Inget konton med det kontonumret du matade in hittades. Vänligen testa att skriva kontonumret igen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vänligen mata in ett korrekt kontonummer. Vänligen testa att skriva kontonumret igen.");
                    }
                } while (depositToAcc == 0);


                decimal amountOfMoneyToDeposit = 0;

                if (depositAcc is ForeignAccount)
                {
                    Console.WriteLine($"\nHur mycket pengar vill du sätta in på kontonummer {depositToAcc}? Ange summan i dollar");
                }
                else
                {
                    Console.WriteLine($"\nHur mycket pengar vill du sätta in på kontonummer {depositToAcc}?");
                }
                do
                {
                    decimal inputAmount = 0;
                    if (decimal.TryParse(Console.ReadLine(), out inputAmount))
                    {
                        if (inputAmount > 0)
                        {
                            amountOfMoneyToDeposit = inputAmount;

                        }
                        else
                        {
                            Console.WriteLine("Vänligen mata in en summa som är giltig att överföra. Skriv summan som ska överföras igen.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Vänligen mata in en summa som är giltig att överföra. Skriv summan som ska överföras igen.");
                    }

                } while (amountOfMoneyToDeposit == 0);

                int triesLeft = 3;
                bool succesfulTransaction = false;

                if (depositAcc is ForeignAccount)
                {
                    Console.WriteLine($"\nDu vill sätta in $ {amountOfMoneyToDeposit} på kontot med kontonummer {depositToAcc}." +
                        $"\nVänligen mata in ditt lösenord för att genomföra transaktionen.");
                }

                else
                {
                    Console.WriteLine($"\nDu vill sätta in {amountOfMoneyToDeposit} kr på kontot med kontonummer {depositToAcc}." +
                         $"\nVänligen mata in ditt lösenord för att genomföra transaktionen.");
                }

                do
                {
                    string input = HidePassWord(); ;

                    if (user.Authentication(input, user.UserId))
                    {
                        if (user.UserId == depositToUser.UserId)
                        {
                            int indexOfDepositToAcc = user.BankAccounts.IndexOf(user.BankAccounts.Find(x => x.AccountNr == depositToAcc));

                            if (depositAcc is ForeignAccount)
                            {
                                user.BankAccounts[indexOfDepositToAcc].AddBalance(amountOfMoneyToDeposit);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                user.UpdateLog($"\n\nDu satte in $ {amountOfMoneyToDeposit} på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"\n\nDu satte in $ {amountOfMoneyToDeposit} på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot är $ {user.BankAccounts[indexOfDepositToAcc].GetBalance():f2}.");

                                succesfulTransaction = true;
                            }
                            else
                            {
                                user.BankAccounts[indexOfDepositToAcc].AddBalance(amountOfMoneyToDeposit);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                user.UpdateLog($"\n\nDu satte in {amountOfMoneyToDeposit} kr på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"\n\nDu satte in {amountOfMoneyToDeposit} kr på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"Ditt nya saldo på kontot är {user.BankAccounts[indexOfDepositToAcc].GetBalance():f2} kr.");

                                succesfulTransaction = true;
                            }
                        }
                        else
                        {
                            int indexOfTransferToAcc = depositToUser.BankAccounts.IndexOf(depositToUser.BankAccounts.Find(x => x.AccountNr == depositToAcc));

                            if (depositAcc is ForeignAccount)
                            {
                                depositToUser.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToDeposit);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == depositToUser.UserId))] = depositToUser;
                                user.UpdateLog($"\n\nDu satte in $ {amountOfMoneyToDeposit} på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"\n\nDu satte in $ {amountOfMoneyToDeposit} på kontot med kontonummer {depositToAcc}.");

                                succesfulTransaction = true;
                            }

                            else
                            {
                                depositToUser.BankAccounts[indexOfTransferToAcc].AddBalance(amountOfMoneyToDeposit);
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == user.UserId))] = user;
                                Persons[Persons.IndexOf(Persons.Find(x => x.UserId == depositToUser.UserId))] = depositToUser;
                                user.UpdateLog($"\n\nDu satte in {amountOfMoneyToDeposit} kr på kontot med kontonummer {depositToAcc}.");
                                Console.WriteLine($"\n\nDu satte in {amountOfMoneyToDeposit} kr på kontot med kontonummer {depositToAcc}.");

                                succesfulTransaction = true;
                            }
                        }
                    }
                    else
                    {
                        triesLeft--;
                        Console.WriteLine(triesLeft > 0 ? "\nFel lösenord. Försök igen." : "\nTransaktion misslyckades. Du skrev fel lösenord för många gånger.");
                    }

                } while (triesLeft > 0 & !succesfulTransaction);
            }
            Console.WriteLine("\nKlicka enter för att komma vidare.");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
   


