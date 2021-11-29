﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FoxyBank
{
    public class Bank
    {
        public List<Person> Persons { get; set; }
        public Dictionary<int, int> Accounts { get; set; }

        public Bank()
        {
            this.Persons = new List<Person>();
            this.Accounts = new Dictionary<int, int>();
        }
        public void StartApplication()
        {
            byte Tries = 3;
            Console.WriteLine("Hej välkommen till Foxy Bank.");

            do
            {

                Person loggedInPerson = Login();
                if (loggedInPerson == null)
                {
                    Console.WriteLine("Failed.");
                    Tries--;
                }
                else
                {
                    Console.WriteLine("Your now logged in!");
                    char firstDigit = loggedInPerson.UserId.ToString()[0];

                    if (firstDigit == '1')
                    {
                        RunAdminMenu(loggedInPerson);
                    }
                    else
                    {
                        RunUserMenu(loggedInPerson);
                    }
                    break;
                }
            } while (Tries > 0);

        }

        public Person Login()
        {
            Console.WriteLine("Write user id.");
            int AnId = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter password.");
            string AnPassword = Console.ReadLine();

            foreach (Person A1 in Persons)
            {
                if (A1.Authentication(AnPassword, AnId))
                {
                    return A1;
                }
            }
            return null;
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
        public void RunAdminMenu(Person loggedInPerson)
        {
            bool isRunning = true;

            do
            {
                Console.WriteLine($"Hej {loggedInPerson.FirstName} {loggedInPerson.LastName}. Vad vill du göra?");

                Console.WriteLine("Användarmeny för administrator:" +
                            "\n1. Skapa ny bankkund" +
                            "\n2. Ändra valutakurs" +
                            "\n3. Ändra sparränta" +
                            "\n4 Logga ut");

                string menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        RegisterNewUser();
                        break;
                    case "2":
                        //ExchangeRate();
                        break;

                    case "3":
                        //InterestRate();
                        break;

                    case "4":
                        //LogOut();
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }
            while (isRunning != false);
        }

        public void RunUserMenu(Person loggedInPerson)
        {
            bool isRunning = true;

            do
            {
                Console.WriteLine($"Hej {loggedInPerson.FirstName} {loggedInPerson.LastName}. Vad vill du göra:");
                Console.WriteLine("1. Se dina konton och saldo" +
                        "\n2. Överföring mellan egna konton" +
                        "\n3. Överföring till andra användares konton" +
                        "\n4. Skapa nytt bankkonto" +
                        "\n5. Logga ut");

                string menuChoice = Console.ReadLine();

                switch (menuChoice)
                {
                    case "1":
                        //DisplayAllAccounts();
                        break;

                    case "2":
                        //InternalTransfer();
                        break;

                    case "3":
                        //ExternalTransfer();
                        break;
                    case "4":
                        CreateAccount((User)loggedInPerson);
                        break;

                    case "5":
                        //LogOut();
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }
            while (isRunning != false);
        }

        public void RegisterNewUser()
        {


            Console.WriteLine("Var god skriv in användarens förnamn");
            string firstNameInput = Console.ReadLine();
            Console.WriteLine("Var god skriv in användarens efternamn");
            string lastNameInput = Console.ReadLine();
            Console.WriteLine("Var god skriv in användarens lösenord");
            string passWordInput = Console.ReadLine();
            User newBankUser = new User(firstNameInput, lastNameInput, passWordInput, GenerateUserID());


            this.Persons.Add(newBankUser);
            Console.WriteLine("Ny användare tillagd.");
            Console.WriteLine("Användarinfo");
            Console.WriteLine("Namn : {0} {1}", newBankUser.FirstName, newBankUser.LastName);
            Console.WriteLine("Lösenord : {0}",newBankUser.PassWord);
            Console.WriteLine("ID : {0}", newBankUser.UserId);
            Console.ReadKey();



        }

        public void CreateAccount(User user)
        {
            int accountNr = 0;

            Random rand = new Random();
            int randomizedAccNr = rand.Next(10000, 11000);

            if (!Accounts.ContainsKey(randomizedAccNr)) { accountNr = randomizedAccNr; }
            else
            {
                bool foundId = false;
                while (!foundId)
                {
                    randomizedAccNr = rand.Next(10000, 11000);
                    if (!Accounts.ContainsKey(randomizedAccNr))
                    {
                        accountNr = randomizedAccNr;
                        foundId = true;
                    }
                }
            }

            Account createdAccount = null;

            Console.WriteLine("\nVad vill du öppna för konto?");
            do
            {
                Console.WriteLine("\n1.Sparkonto");
                Console.WriteLine("2.Personkonto\n");

                string answer = Console.ReadLine();


                if (answer == "1")
                {
                    createdAccount = new SavingAccount(accountNr);
                    user.Accounts.Add(createdAccount);
                    this.Accounts.Add(createdAccount.AccountNr, user.UserId);

                }

                else if (answer == "2")
                {
                    createdAccount = new PersonalAccount(accountNr);
                    user.Accounts.Add(createdAccount);
                    this.Accounts.Add(createdAccount.AccountNr, user.UserId);

                }
                else
                {
                    Console.WriteLine("Vänligen välj vilket typ av konto du vill öppna. Svara ett nummer från menyn.");
                }

            } while (createdAccount == null);

            Console.WriteLine($"\nGrattis! Du skapade ett {((createdAccount is PersonalAccount) ? "Personkonto" : "Sparkonto")} med kontonumret " + createdAccount.AccountNr);
        }
    }
}


