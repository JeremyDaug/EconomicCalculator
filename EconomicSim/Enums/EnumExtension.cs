using EconomicSim.Objects.Firms;

namespace EconomicSim.Enums;

public class EnumExtension
{
    public static string ToName(FirmRank firmRank)
    {
        switch (firmRank)
        {
            case FirmRank.Firm:
                return "Firm";
            case FirmRank.Company:
                return "Company";
            case FirmRank.Corporation:
                return "Corporation";
            case FirmRank.Megacorp:
                return "Mega-corporation";
            default:
                throw new ArgumentException("Invalid Firm Rank");
        }
    }

    public static string ToName(OwnershipStructure structure)
    {
        switch (structure)
        {
            case OwnershipStructure.SelfEmployed:
                return "Self-Employed";
            case OwnershipStructure.Association:
                return "Association";
            case OwnershipStructure.Guild:
                return "Guild";
            case OwnershipStructure.Private:
                return "Private";
            case OwnershipStructure.Cooperative:
                return "Cooperative";
            case OwnershipStructure.PubliclyTraded:
                return "Publicly Traded";
            case OwnershipStructure.StateOwned:
                return "State Owned";
            default:
                throw new ArgumentException("Invalid Ownership Structure");
        }
    }

    public static string ToName(ProfitStructure profitStructure)
    {
        switch (profitStructure)
        {
            case ProfitStructure.Distributed:
                return "Distributed";
            case ProfitStructure.Shares:
                return "Shares";
            case ProfitStructure.PrivatelyOwned:
                return "Privately Owned";
            case ProfitStructure.ProfitSharing:
                return "Profit Sharing";
            case ProfitStructure.NonProfit:
                return "Non-profit";
            default:
                throw new ArgumentException("Invalid Profit Structure");
        }
    }

    public static string ToName(WageType jobWageType)
    {
        switch (jobWageType)
        {
            case WageType.Slave:
                return "Slave";
            case WageType.LossSharing:
                return "Loss Sharing";
            case WageType.Productivity:
                return "Per Unit";
            case WageType.Daily:
                return "Daily";
            case WageType.Salary:
                return "Salaried";
            case WageType.Contractor:
                return "Wage Contractor";
            case WageType.ProfitSharing:
                return "Profit Sharing";
            default:
                throw new ArgumentException("Invalid Wage Type");
        }
    }
}