using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Roleplay.Server.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Models
{
    [Table("BankAccounts")]
    public class BankAccount
    {
        [Key]
        public int Id { get; set; }
        public int AccountNumber { get; set; }
        public double Money { get; set; }
        public int PinCode { get; set; }
        public string HistoryString { get; set; }
        public int OwnerUser { get; set; }
        public int OwnerGroup { get; set; }
        public bool IsPrivate { get; set; }
        [NotMapped]
        public List<BankAccountHistory> History { get; set; }

        public BankAccount()
        {
            History = new List<BankAccountHistory>();
            OwnerUser = 0;
            OwnerGroup = 0;
            IsPrivate = false;
        }

        public void StringToHistory()
        {
            if (HistoryString != "" && HistoryString != null)
            {
                History = JsonConvert.DeserializeObject<List<BankAccountHistory>>(HistoryString);
            }
            else
            {
                History = new List<BankAccountHistory>();
            }
        }

        public void HistoryToString()
        {
            HistoryString = JsonConvert.SerializeObject(History);
        }
    }

    public class BankAccountHistory
    {
        public DateTime Date { get; set; }
        public BankAccountHistoryType Type { get; set; }
        public double Amount { get; set; }
        public int Target { get; set; }
        public int IssuedBy { get; set; }
        public Vector3 Position { get; set; }
        public string Usage { get; set; }

        public BankAccountHistory()
        {
            Usage = "";
        }
    }
}
