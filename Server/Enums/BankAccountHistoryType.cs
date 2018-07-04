using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roleplay.Server.Enums
{
    public enum BankAccountHistoryType
    {
        Unknown = 0,
        Withdraw = 1, // Abheben
        Deposit = 2, // Einzahlen
        Transfer = 3, // Überweisen
        Taxes = 4, // Steuern
        UnemploymentBenefits = 5, // Arbeitslosengeld
        Salary = 6, // Gehalt
        CardPayment = 7, // Kartenzahlung
        Debit = 8 // Lastschrift
    }
}
