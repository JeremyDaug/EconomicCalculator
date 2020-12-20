using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.Enums
{
    public enum CashType
    {
        Commodity, // A common item that is traded as currency.
        Minted, // A currency which requires specifically creating to use. Can Suffer Debasement.
        Token, // A token or bank note which is cheap but can be redeemed for a more valuable good. Can suffer over issuance.
        Fiat // Unbacked currency worth whatever the market says it is and nothing else. Can suffer Hyperinflation.
    }
}
