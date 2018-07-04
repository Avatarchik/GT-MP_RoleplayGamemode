using GrandTheftMultiplayer.Server.Elements;
using Newtonsoft.Json;
using Roleplay.Base;
using Roleplay.Server.Base;
using Roleplay.Server.Enums;
using Roleplay.Server.Extensions;
using Roleplay.Server.Managers;
using Roleplay.Server.Models;
using Roleplay.Server.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Controller
{
    public class BankController : RoleplayScript
    {
        public List<int> AtmModels = new List<int>
        {
            -870868698, // Small Atm
            -1126237515, // Blue Atm
            -1364697528, // Red Atm
            506770882, // Green Atm
        };
        public BankController()
        {
            InteractionController.InteractionObjects.AddRange(AtmModels);
            InteractionController.OnPlayerInteractWithObject += InteractionController_OnPlayerInteractWithObject;
            ClientEventManager.RegisterClientEvent("CloseAtmMenu", CloseAtmMenu);
            ClientEventManager.RegisterClientEvent("WITHDRAW", WithdrawAtm);
            ClientEventManager.RegisterClientEvent("DEPOSIT", DepositAtm);
        }

        private void DepositAtm(Client sender, string eventName, object[] arguments)
        {
            Deposit(sender, Convert.ToDouble(arguments[0]));
        }

        private void WithdrawAtm(Client sender, string eventName, object[] arguments)
        {
            Withdraw(sender, Convert.ToDouble(arguments[0]));
        }

        private void CloseAtmMenu(Client sender, string eventName, object[] arguments)
        {
            CloseAtmMenu(sender);
        }

        private void InteractionController_OnPlayerInteractWithObject(int model, InteractData data)
        {
            if (!AtmModels.Contains(model))
                return;
            API.sendNotificationToPlayer(data.Client, "Mit Bankautomat Interagiert");

            int accountNumber = GetCharacterBankAccount(data.Client);

            OpenAtmMenuForPlayer(data.Client, accountNumber);
        }

        public static void Withdraw(Client client, double amount, bool allowNegativeBankValue = false)
        {
            int accountnumber = client.GetData("OPEN_ATM_ACCOUNT_NUMBER", 0);
            if (accountnumber == 0)
                return;
            BankAccount account = null;
            using (var db = new Database())
            {
                account = db.BankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountnumber);
                if (account == null)
                    return;
                if(account.Money < amount && !allowNegativeBankValue)
                {
                    GameMode.sharedAPI.sendColoredNotificationToPlayer(client, "Es befindet sich nicht genug Geld auf dem Konto!", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
                    return;
                }

                client.Account().CurrentCharacter.Cash += amount;
                CharacterController.SaveCharacter(client, client.Account().CurrentCharacter);

                account.Money -= amount;
                account.StringToHistory();
                account.History.Add(new BankAccountHistory
                {
                    Amount = (amount - (2 * amount)),
                    Date = DateTime.Now,
                    IssuedBy = client.Account().CurrentCharacter.Id,
                    Target = account.AccountNumber,
                    Type = BankAccountHistoryType.Withdraw,
                    Position = client.position
                });
                account.HistoryToString();
                db.SaveChanges();
                UpdateAtmDisplay(client);

                GameMode.sharedAPI.sendColoredNotificationToPlayer(client, $"Du hast erfolgreich {amount} $ von {account.AccountNumber} abgehoben.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
            }
        }

        public static void Deposit(Client client, double amount)
        {
            int accountnumber = client.GetData("OPEN_ATM_ACCOUNT_NUMBER", 0);
            if (accountnumber == 0)
                return;
            BankAccount account = null;
            using (var db = new Database())
            {
                account = db.BankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountnumber);
                if (account == null)
                    return;
                if (client.Account().CurrentCharacter.Cash < amount)
                {
                    GameMode.sharedAPI.sendColoredNotificationToPlayer(client, "Du hast nicht so viel Geld dabei!", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_RED);
                    return;
                }

                client.Account().CurrentCharacter.Cash -= amount;
                CharacterController.SaveCharacter(client, client.Account().CurrentCharacter);

                account.Money += amount;
                account.StringToHistory();
                account.History.Add(new BankAccountHistory
                {
                    Amount = amount,
                    Date = DateTime.Now,
                    IssuedBy = client.Account().CurrentCharacter.Id,
                    Target = client.Account().CurrentCharacter.Id,
                    Type = BankAccountHistoryType.Deposit,
                    Position = client.position
                });
                account.HistoryToString();
                db.SaveChanges();
                UpdateAtmDisplay(client);

                GameMode.sharedAPI.sendColoredNotificationToPlayer(client, $"Du hast erfolgreich {amount} $ auf {account.AccountNumber} eingezahlt.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_GREEN);
            }
        }

        public static void GiveUnemploymentBenefits(Client client)
        {
            int accountnumber = GetCharacterBankAccount(client);
            if (accountnumber == 0)
                return;
            using (var db = new Database())
            {
                BankAccount bankAccount = db.BankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountnumber);
                if (bankAccount == null)
                    return;
                bankAccount.Money += Constants.UnemploymentBenefits;
                bankAccount.StringToHistory();
                bankAccount.History.Add(new BankAccountHistory
                {
                    Amount = Constants.UnemploymentBenefits,
                    Date = DateTime.Now,
                    IssuedBy = 0,
                    Target = client.Account().CurrentCharacter.Id,
                    Type = BankAccountHistoryType.UnemploymentBenefits,
                    Position = client.position
                });
                bankAccount.HistoryToString();
                db.SaveChangesAsync();
            }
            client.sendColoredNotification($"Du hast {Constants.UnemploymentBenefits} $ Arbeitslosengeld bekommen.", (int)HudColor.HUD_COLOUR_PURE_WHITE, (int)HudColor.HUD_COLOUR_BLUE);
        }

        public void OpenAtmMenuForPlayer(Client client, int bankAccountNumber)
        {
            BankAccount account = null;
            using (var db = new Database())
            {
                account = db.BankAccounts.FirstOrDefault(acc => acc.AccountNumber == bankAccountNumber);
            }
            if (account == null)
                return;
            client.setData("OPEN_ATM_ACCOUNT_NUMBER", account.AccountNumber);
            client.triggerEvent("showBrowser", Constants.BankAtmUrl, "FillDisplay", JsonConvert.SerializeObject(PrepareAtmMenu(account)));
        }

        public static void CloseAtmMenu(Client client)
        {
            client.resetData("OPEN_ATM_ACCOUNT_NUMBER");
            client.triggerEvent("hideAccountModal");
        }

        public static void UpdateAtmDisplay(Client client)
        {
            int accountnumber = client.GetData("OPEN_ATM_ACCOUNT_NUMBER", 0);
            if (accountnumber == 0)
                return;
            BankAccount account = null;
            using (var db = new Database())
            {
                account = db.BankAccounts.FirstOrDefault(acc => acc.AccountNumber == accountnumber);
            }
            if (account == null)
                return;
            client.triggerEvent("updateBrowser", "FillDisplay", JsonConvert.SerializeObject(PrepareAtmMenu(account)));
        }

        public static AtmOverviewMenu PrepareAtmMenu(BankAccount bankAccount)
        {
            bankAccount.StringToHistory();
            var data = new AtmOverviewMenu
            {
                AccountNumber = bankAccount.AccountNumber.ToString(),
                AccountValue = $"{bankAccount.Money} $"
            };
            foreach (BankAccountHistory history in bankAccount.History.OrderByDescending(d => d.Date))
            {
                var hist = new AtmOverviewMenuHistory();
                hist.Date = history.Date.ToString("dd.MM.yyyy - HH:mm:ss");
                if (history.Amount >= 0)
                {
                    hist.Amount = $"<b style='color: green;'>+{history.Amount}</b>";
                }
                else
                {
                    hist.Amount = $"<b style='color: red;'>{history.Amount}</b>";
                }
                switch (history.Type)
                {
                    case BankAccountHistoryType.Deposit:
                        hist.Type = "Einzahlung";
                        break;
                    case BankAccountHistoryType.Withdraw:
                        hist.Type = "Auszahlung";
                        break;
                    case BankAccountHistoryType.Transfer:
                        hist.Type = "&Uuml;berweisung";
                        break;
                    case BankAccountHistoryType.Taxes:
                        hist.Type = "Steuern";
                        break;
                    case BankAccountHistoryType.UnemploymentBenefits:
                        hist.Type = "Arbeitslosengeld";
                        break;
                    case BankAccountHistoryType.Salary:
                        hist.Type = "Gehalt";
                        break;
                    case BankAccountHistoryType.CardPayment:
                        hist.Type = "Kartenzahlung";
                        break;
                    case BankAccountHistoryType.Debit:
                        hist.Type = "Lastschrift";
                        break;
                    default:
                        hist.Type = "Unbekannt";
                        break;
                }
                data.History.Add(hist);
            }
            return data;
        }

        public static int CreateBankAccount(double startMoney, int ownerUser, bool isPrivate = false,int ownerGroup = 0)
        {
            int accnumber = Convert.ToInt32("4" + ownerGroup.ToString() + GetTenDigitRandomNumbers() + ownerUser.ToString());

            using (var db = new Database())
            {
                BankAccount account = new BankAccount
                {
                    AccountNumber = accnumber,
                    History = new List<BankAccountHistory>(),
                    HistoryString = "",
                    Money = startMoney,
                    OwnerUser = ownerUser,
                    OwnerGroup = ownerGroup,
                    PinCode = 0,
                    IsPrivate = isPrivate
                };
                db.BankAccounts.Add(account);
                db.SaveChanges();
                return account.AccountNumber;
            }
        }

        public static int GetCharacterBankAccount(Client client)
        {
            int charid = client.Account().CurrentCharacter.Id;
            using (var db = new Database())
            {
                if(db.BankAccounts.Count() == 0)
                {
                    return 0;
                }
                var account = db.BankAccounts.FirstOrDefault(x => x.OwnerUser == charid && x.IsPrivate);
                if(account == null)
                {
                    return CreateBankAccount(Constants.BankAccountStartMoney, charid, true);
                }
                return account.AccountNumber;
            }
        }

        public static string GetTenDigitRandomNumbers()
        {
            Random generator = new Random();
            return generator.Next(999, 10000).ToString();
        }
    }
}
