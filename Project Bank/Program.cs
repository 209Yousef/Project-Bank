using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace Project_Bank
{
    internal class Program
    {
        const string clientsFileName = "Clients.txt";

        const string usersFileName = "Users.txt";

        enum enWhatDoYoWantMainMenueScreen 
        { eShowClientList = 1, eAddClient = 2, eDeleteClient = 3,
            eUpdateClient = 4, eFindClient = 5, eTransactions = 6, eExist = 7
        };

        enum enWhatDoYoWantTransactionMenueScreen
        {
            eDeposit = 1, eWithdraw = 2, 
            eTotalBalance = 3, eMainMenue = 4
        };

        struct stDataClient
        {
            public string accountNumber;
            public string pinCode;
            public string name;
            public string phone;
            public double accountBalance;

            public bool markClientDelete;
        }

        struct stUser
        {
            public string userName;
            public string password;
            public int permission;
        }

        static List<string> SplitString(string s, string delim)
        {
            List<string> lWords = new List<string>();

            string sWord;
            int pos;

            while((pos = s.IndexOf(delim)) != -1)
            {
                sWord = s.Substring(0, pos);

                if(sWord != "")
                {
                    lWords.Add(sWord);
                }

                s = s.Remove(0, pos + delim.Length);
            }

            if(s != "")
            {
                lWords.Add(s);  
            }

            return lWords;
        }

        static stDataClient ConvertDataLineToRecord(string lineRecord, string delim)
        {
            List<string> lWords = SplitString(lineRecord, delim);
            stDataClient dataClient = new stDataClient();

            dataClient.accountNumber = lWords[0];
            dataClient.pinCode = lWords[1];
            dataClient.name = lWords[2];
            dataClient.phone = lWords[3];
            dataClient.accountBalance = double.Parse(lWords[4]);

            return dataClient;
        }

        static List<stDataClient> LoadDataRecordFromFile(string fileName)
        {
            List<stDataClient> lClients = new List<stDataClient>();
            stDataClient dataClient = new stDataClient();

            StreamReader myFile = new StreamReader(fileName);
            
            while(!myFile.EndOfStream)
            {
                string lineRecord = myFile.ReadLine();
                dataClient = ConvertDataLineToRecord(lineRecord, "#//#");
                lClients.Add(dataClient);
            }

            myFile.Close();

            return lClients;
        }

        static void PrintClients(List<stDataClient> lClients)
        {
            for(int i = 0; i < lClients.Count; i++)
            {
                Console.Write($"| {lClients[i].accountNumber, -20}");
                Console.Write($"| {lClients[i].pinCode, -15}");
                Console.Write($"| {lClients[i].name, -40}");
                Console.Write($"| {lClients[i].phone, -20}");
                Console.WriteLine($"| {lClients[i].accountBalance, -15}");
            }
        }

        static void ShowDataClientsScreen()
        {
            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);

            Console.WriteLine($"\t\t\t\t\t\tClient List ({lClients.Count}) Client(s).\n");

            Console.Write("----------------------------------------------------------------------------------");
            Console.WriteLine("--------------------------------------\n");
            Console.Write($"| {"Account Number",-20}");
            Console.Write($"| {"Pin Code",-15}");
            Console.Write($"| {"Client Name",-40}");
            Console.Write($"| {"Phone",-20}");
            Console.Write($"| {"Balance",-15}\n");
            Console.Write("\n----------------------------------------------------------------------------------");
            Console.WriteLine("--------------------------------------\n");

            PrintClients(lClients);

            Console.Write("\n\n----------------------------------------------------------------------------------");
            Console.WriteLine("--------------------------------------\n\n");

        }

        static string ReadAccountNumber()
        {
            string accountNumber;

            Console.Write("Enter account number? ");
            accountNumber = Console.ReadLine();

            return accountNumber;
        }

        static string ConvertRecordToLine(stDataClient dataClient, string delim = "#//#")
        {
            return dataClient.accountNumber + delim + dataClient.pinCode + delim +
                   dataClient.name + delim + dataClient.phone + delim + dataClient.accountBalance; 
        }

        static bool IsExistsAccountNumberInFile(string accountNumber, string fileName)
        {
            StreamReader myFile = new StreamReader(fileName);

            stDataClient dataClient = new stDataClient();

            while(!myFile.EndOfStream)
            {
                string lineRecord = myFile.ReadLine();

                dataClient = ConvertDataLineToRecord(lineRecord, "#//#");

                if(dataClient.accountNumber == accountNumber)
                {
                    myFile.Close();
                    return true;
                }
            }

            myFile.Close();
            return false;
            
        }

        static stDataClient ReadNewClient()
        {
            stDataClient dataClient = new stDataClient();

            Console.Write("Enter account number? ");
            dataClient.accountNumber = Console.ReadLine();
            
            while(IsExistsAccountNumberInFile(dataClient.accountNumber, clientsFileName))
            {
                Console.Write($"Client with acount number ({dataClient.accountNumber}) already exits, enter another accout number? ");
                dataClient.accountNumber = Console.ReadLine();
            }

            Console.Write("Enter PinCode? ");
            dataClient.pinCode = Console.ReadLine();

            Console.Write("Enter Client Name? ");
            dataClient.name = Console.ReadLine();

            Console.Write("Enter phone? ");
            dataClient.phone = Console.ReadLine();

            Console.Write("Enter account balance? ");
            dataClient.accountBalance = double.Parse(Console.ReadLine());

            return dataClient;
        }

        static void AddDataClientToFile(stDataClient dataClient, string fileName)
        {
            StreamWriter myFile = new StreamWriter(fileName, true);

            string lineRecord = ConvertRecordToLine(dataClient);

            myFile.WriteLine(lineRecord);

            myFile.Close(); 
        }

        static void AddNewClient()
        {
            stDataClient dataClient = new stDataClient();

            dataClient = ReadNewClient();

            AddDataClientToFile(dataClient, clientsFileName);
        }

        static void AddClients()
        {
            char isAddMore;

            do
            {
                Console.WriteLine("\nAdding new client\n");

                AddNewClient();

                Console.Write("\nClient Added Succussufully, Do you want add more client? y/n ? ");
                isAddMore = char.Parse(Console.ReadLine());
            }
            while (isAddMore == 'y' || isAddMore == 'Y');
        }

        static void ShowAddClientsScreen()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("\t\tAdd New Client Screen                     ");
            Console.WriteLine("==============================================");

            AddClients();
        }

        static bool FindClientByAccountNumber(string accountNumber, List<stDataClient> lClients, ref stDataClient dataClient)
        {
            for(int i = 0; i < lClients.Count; i++)
            {
                if (lClients[i].accountNumber == accountNumber)
                {
                    dataClient = lClients[i];
                    return true;
                }
            }

            return false;
        }

        static void PrintClientCard(stDataClient dataClient)
        {
            Console.WriteLine("\nThe Folowing are the client details: ");

            Console.WriteLine("---------------------------------------");

            Console.WriteLine($"Account Number: {dataClient.accountNumber}");
            Console.WriteLine($"PinCode       : {dataClient.pinCode}");
            Console.WriteLine($"Client Name   : {dataClient.name}");
            Console.WriteLine($"Phone         : {dataClient.phone}");
            Console.WriteLine($"Account Balance: {dataClient.accountBalance}");

            Console.WriteLine("---------------------------------------");
            
        }

        static bool MarkClientForDeleteClientByAccountNumber(List<stDataClient> lClients, string accountNumber)
        {
            stDataClient dataClient = new stDataClient();

            for(int i = 0; i < lClients.Count; i++)
            {
                if (lClients[i].accountNumber == accountNumber)
                {
                    dataClient = lClients[i];
                    dataClient.markClientDelete = true;
                    lClients[i] = dataClient;
                    return true;
                }
            }
            return false;
        }

        static void SaveDataRecordToFile(string fileName, List<stDataClient> lClients)
        {
            StreamWriter myFile = new StreamWriter(fileName, false);

            for(int i = 0; i < lClients.Count; i++)
            {
                if (lClients[i].markClientDelete == false)
                {
                    string lineRecord = ConvertRecordToLine(lClients[i]);

                    myFile.WriteLine(lineRecord);
                }
            }

            myFile.Close();
        }

        static void DeleteClient()
        {
            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);

            string accountNumber = ReadAccountNumber();

            stDataClient dataClient = new stDataClient();

            if(FindClientByAccountNumber(accountNumber, lClients, ref dataClient))
            {
                PrintClientCard(dataClient);

                Console.Write("\nAre you sure do you want delete this client? y/n ? ");
                char answer = char.Parse(Console.ReadLine());

                if (char.ToLower(answer) == 'y')
                {
                    MarkClientForDeleteClientByAccountNumber(lClients, accountNumber);
                    SaveDataRecordToFile(clientsFileName, lClients);

                    lClients = LoadDataRecordFromFile(clientsFileName);

                    Console.WriteLine("\nClient Deleted Succussufully.");
                }
            }
            else
            {
                Console.WriteLine($"\nClient with account number ({accountNumber}), not found!");
            }
        }

        static void ShowDeleteClientScreen()
        {
            Console.WriteLine("\n===================================");
            Console.WriteLine("\tDelete Client Screen               ");
            Console.WriteLine("===================================\n");

            DeleteClient();
        }

        static stDataClient ChangeDataClient(string accountNumber)
        {
            stDataClient dataClient = new stDataClient();

            dataClient.accountNumber = accountNumber;

            Console.Write("\nEnter PinCode: ");
            dataClient.pinCode = Console.ReadLine();

            Console.Write("Enter name: ");
            dataClient.name = Console.ReadLine();

            Console.Write("Enter phone: ");
            dataClient.phone = Console.ReadLine();

            Console.Write("Enter account balance: ");
            dataClient.accountBalance = double.Parse(Console.ReadLine());

            return dataClient;
        }

        static void UpdateClientData(List<stDataClient> lClients, string accountNumber)
        {
            stDataClient dataClient = new stDataClient();

            if(FindClientByAccountNumber(accountNumber, lClients, ref dataClient))
            {
                PrintClientCard(dataClient);

                Console.Write("\nAre you sure do you want update this client? y/n ? ");
                char isUpdate = char.Parse(Console.ReadLine());

                if(char.ToUpper(isUpdate) == 'Y')
                {
                    for (int i = 0; i < lClients.Count; i++)
                    {
                        if (lClients[i].accountNumber == accountNumber)
                        {
                            lClients[i] = ChangeDataClient(accountNumber);
                            break;
                        }
                    }

                    SaveDataRecordToFile(clientsFileName, lClients);

                    Console.WriteLine("\nClient updated succussufully.");

                }
            }

            else
            {
                Console.WriteLine($"\nClient with account number ({accountNumber}), not found!");
            }
        }

        static void ShowUpdateClientScreen()
        {
            Console.WriteLine("\n===================================");
            Console.WriteLine("\tDelete Client Screen               ");
            Console.WriteLine("===================================\n");

            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);
            string accountNumber = ReadAccountNumber();

            UpdateClientData(lClients, accountNumber);
        }

        static void FindClient(string accountNumber, List<stDataClient> lClients)
        {
            stDataClient dataClient = new stDataClient();

            if(FindClientByAccountNumber(accountNumber, lClients, ref dataClient))
            {
                PrintClientCard(dataClient);
            }

            else
            {
                Console.WriteLine($"\nClient with account number ({accountNumber}), not found!");
            }
        }

        static void ShowFindClientScreen()
        {
            Console.WriteLine("\n=========================================");
            Console.WriteLine("\tFind Client Screen                       ");
            Console.WriteLine("=========================================\n");

            string accountNumber = ReadAccountNumber();
            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);

            FindClient(accountNumber, lClients);
        }

        static void ShowExistScreen()
        {
            Console.WriteLine("\n-----------------------------------");
            Console.WriteLine("\tPrograms Ends :-)                  ");
            Console.WriteLine("-----------------------------------  ");
        }

        static void GoBackToMainMenue()
        {
            Console.WriteLine("\n\nPress any key to go back main menue...");

            Console.ReadKey();

            ShowMainMenueScreen();
        }

        static short ReadMainMenueOption()
        {
            short number;

            do
            {
                Console.Write("Choose what do you want to do? [1 to 7]? ");
                number = short.Parse(Console.ReadLine());
            }
            while (number < 1 || number > 7);

            return number;
        }

        static void ShowMainMenueScreen()
        {
            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("\t\tMain Menue Screen                         ");
            Console.WriteLine("==============================================");

            Console.WriteLine("\t[1] Show Client List.");
            Console.WriteLine("\t[2] Add New Client.");
            Console.WriteLine("\t[3] Delete Client.");
            Console.WriteLine("\t[4] Update Client Info.");
            Console.WriteLine("\t[5] Find Client.");
            Console.WriteLine("\t[6] Transactions.");
            Console.WriteLine("\t[7] Exit.");
            Console.WriteLine("==============================================");

            PerformMainMenuOption((enWhatDoYoWantMainMenueScreen)ReadMainMenueOption());

        }

        static short ReadTransactionMenueScreen()
        {
            short number;

            do
            {
                Console.Write("Choose what do you want? [1 to 4] ? ");
                number = short.Parse(Console.ReadLine());
            }
            while (number < 1 || number > 4);

            return number;
        }

        static double ReadDepositAmount()
        {
            double amount;

            Console.Write("\nPlease enter deposit amount? ");
            amount = double.Parse(Console.ReadLine());

            return amount;
        }
            
        static bool DepositBalanceToClientWithAccountNumber(List<stDataClient> lClients, double amount, string accountNumber)
        {
            Console.Write("\nAre you sure do you want perfom this transaction? [y/n] ? ");
            char answer = char.Parse(Console.ReadLine());

            stDataClient dataClient = new stDataClient();

            if(answer == 'y' || answer == 'Y')
            {
                for(int i = 0; i < lClients.Count; i++)
                {
                    if (lClients[i].accountNumber == accountNumber)
                    {
                        dataClient = lClients[i];
                        dataClient.accountBalance += amount;
                        lClients[i] = dataClient;

                        Console.WriteLine($"\nDone succussufully, your new account balance is: {lClients[i].accountBalance}");

                        SaveDataRecordToFile(clientsFileName, lClients);

                        return true;
                    }
                }
            }

            return false;
        }

        static void ShowDepositScreen()
        {
            Console.WriteLine("\n=================================");
            Console.WriteLine("\tDeposit Screen                   ");
            Console.WriteLine("=================================");

            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);
            string accountNumber = ReadAccountNumber();
            
            stDataClient dataClient = new stDataClient();   

            while(!FindClientByAccountNumber(accountNumber, lClients, ref dataClient))
            {
                Console.WriteLine($"\nClient with account number ({accountNumber}), does not exist.");

                Console.Write("Please enter another account number? ");
                accountNumber = Console.ReadLine();
            }

            PrintClientCard(dataClient);

            double depositAmount = ReadDepositAmount();

            DepositBalanceToClientWithAccountNumber(lClients, depositAmount,accountNumber);
        }

        static void GoBackToTransactionMenueScreen()
        {
            Console.WriteLine("\n\nPress any key to go back Transaction Menue Screen...");

            Console.ReadKey();

            ShowTransactionMenueScreen();
        }

        static void PrintClientSmallCard(List<stDataClient> lClients)
        {
            for (int i = 0; i < lClients.Count; i++)
            {
                Console.Write($"| {lClients[i].accountNumber, -18}");
                Console.Write($"| {lClients[i].name, -48}");
                Console.WriteLine($"| {lClients[i].accountBalance, -40}");
            }
        }

        static double TotalBalances(List<stDataClient> lClients)
        {
            double sum = 0;

            for(int i = 0; i < lClients.Count; i++)
            {
                sum += lClients[i].accountBalance;
            }

            return sum;
        }

        static void ShowTotalBalancesScreen()
        {
            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);

            Console.WriteLine($"\t\t\t\t\tBalances List ({lClients.Count}) Client(s).\n");

            Console.Write("--------------------------------------------------------------------------------------");
            Console.WriteLine("----------------------------------\n");

            Console.Write($"| {"Account Number", -18}");
            Console.Write($"| {"Client Name", -48}");
            Console.WriteLine($"| {"Balance", -40}");

            Console.Write("\n--------------------------------------------------------------------------------------");
            Console.WriteLine("----------------------------------\n");

            PrintClientSmallCard(lClients);

            Console.Write("\n\n--------------------------------------------------------------------------------------");
            Console.WriteLine("----------------------------------\n");

            Console.WriteLine($"\t\t\t\t\t Total Balances = {TotalBalances(lClients)}");
        }

        static double ReadWithdrawAmount()
        {
            double amount;

            Console.Write("Please enter withdraw amount? ");
            amount = double.Parse(Console.ReadLine());

            return amount;
        }

        static void ShowWithdrawScreen()
        {
            Console.WriteLine("\n=================================");
            Console.WriteLine("\tWithdraw Screen                   ");
            Console.WriteLine("=================================");

            List<stDataClient> lClients = LoadDataRecordFromFile(clientsFileName);
            string accountNumber = ReadAccountNumber();

            stDataClient dataClient = new stDataClient();
            
            while(!FindClientByAccountNumber(accountNumber, lClients, ref dataClient))
            {
                Console.WriteLine($"\nClient with account number ({accountNumber}), does not exist");

                Console.Write("Please enter another account number? ");
                accountNumber = Console.ReadLine();
            }

            PrintClientCard(dataClient);

            double withdrawAmount = ReadWithdrawAmount();

            while (withdrawAmount > dataClient.accountBalance)
            {
                Console.WriteLine($"\nAmount exceeds the balance, can you wightdraw up to = {dataClient.accountBalance}");

                Console.Write("Please enter another amount? ");
                withdrawAmount = double.Parse(Console.ReadLine());
            }

            DepositBalanceToClientWithAccountNumber(lClients, withdrawAmount * -1, accountNumber);
        }

        static void PerfomTransactionMenueScreen(enWhatDoYoWantTransactionMenueScreen whatDoYoWant)
        {
            switch(whatDoYoWant)
            {
                case enWhatDoYoWantTransactionMenueScreen.eDeposit:
                    Console.Clear();
                    ShowDepositScreen();
                    GoBackToTransactionMenueScreen();
                    break;

                case enWhatDoYoWantTransactionMenueScreen.eWithdraw:
                    Console.Clear();
                    ShowWithdrawScreen();
                    GoBackToTransactionMenueScreen();
                    break;

                case enWhatDoYoWantTransactionMenueScreen.eTotalBalance:
                    Console.Clear();
                    ShowTotalBalancesScreen();
                    GoBackToTransactionMenueScreen();
                    break;

                case enWhatDoYoWantTransactionMenueScreen.eMainMenue:
                    ShowMainMenueScreen();
                    break;
            }
        }

        static void ShowTransactionMenueScreen()
        {
            Console.Clear();

            Console.WriteLine("==============================================");
            Console.WriteLine("\t\tTransaction Menue Screen                  ");
            Console.WriteLine("==============================================");
            Console.WriteLine("\t[1]Deposit.");
            Console.WriteLine("\t[2]Withdraw.");
            Console.WriteLine("\t[3]Total Balance.");
            Console.WriteLine("\t[4]Main Menue.");
            Console.WriteLine("==============================================");

            PerfomTransactionMenueScreen((enWhatDoYoWantTransactionMenueScreen)ReadTransactionMenueScreen());
        }

        static void PerformMainMenuOption(enWhatDoYoWantMainMenueScreen whatDoYoWant)
        {
            switch(whatDoYoWant)
            {
                case enWhatDoYoWantMainMenueScreen.eShowClientList:
                    Console.Clear();
                    ShowDataClientsScreen();
                    GoBackToMainMenue();
                    break;  

                case enWhatDoYoWantMainMenueScreen.eAddClient:
                    Console.Clear();
                    ShowAddClientsScreen();
                    GoBackToMainMenue();
                    break;

                case enWhatDoYoWantMainMenueScreen.eDeleteClient:
                     Console.Clear();
                     ShowDeleteClientScreen();
                     GoBackToMainMenue();
                     break;

                case enWhatDoYoWantMainMenueScreen.eUpdateClient:
                    Console.Clear();
                    ShowUpdateClientScreen();
                    GoBackToMainMenue();
                    break;

                case enWhatDoYoWantMainMenueScreen.eFindClient:
                    Console.Clear();
                    ShowFindClientScreen();
                    GoBackToMainMenue();
                    break;

                case enWhatDoYoWantMainMenueScreen.eTransactions:
                    Console.Clear();
                    ShowTransactionMenueScreen();

                    break;

                case enWhatDoYoWantMainMenueScreen.eExist:
                    Console.Clear();
                    ShowExistScreen();
                    break;

            }
        }

        static void Main(string[] args)
        {
            ShowMainMenueScreen();

            Console.ReadKey();
        }
    }
}
