using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Storage.ProductTags
{
    /// <summary>
    /// The existant product tags in our system.
    /// </summary>
    public enum ProductTag
    {
        /// <summary>
        /// Luxury<D;S>, the good becomes more efficient at satisfying
        /// desires as its' price rises above the average and falls below it.
        /// D is the Multiplier of the effect. S is the name of the want or desire.
        /// S may be a Want, self, for the specific product, or all for everything the
        /// product gives.
        /// </summary>
        Luxury,
        /// <summary>
        /// Bargain<D;S>, the product becomes more efficient at satisfying
        /// desires as it's price falls below the average and the opposite if it's 
        /// above.
        /// D is the Multiplier of the effect. S is the name of the want or desire.
        /// S may be a Want, Self, for the specific product, or all for everything the
        /// product gives.
        /// </summary>
        Bargain,
        /// <summary>
        /// The Product is considered to be public, and thus is available to anyone to use,
        /// regardless of specific ownership.
        /// </summary>
        Public,
        /// <summary>
        /// Claim<S>.
        /// The product is a claim of ownership on some other product or thing, such as a
        /// stock in a company, or a deed to land. This allows ownership of a <see cref="Fixed"/>
        /// good to leave a territory, without actually moving it.
        /// S is for what it claims, Product or Firm is the only valid options currently.
        /// </summary>
        Claim,
        /// <summary>
        /// The product is exclusively a consumeable, using it always consumes it.
        /// </summary>
        Consumable,
        /// <summary>
        /// The product is a consumer good. It should not be used as an input to a process
        /// and variants may have variant mass.
        /// </summary>
        ConsumerGood,
        /// <summary>
        /// The product is an animal. It's maintenance process is it's consumption and includes
        /// the chance of reproduction. Failure is what is produced when it dies.
        /// </summary>
        Animal,
        /// <summary>
        /// The product is not only an animal, but one that can be domesticated, or the domesticated
        /// form of a wild animal. Follows all the rules of animal, but can be cared for.
        /// </summary>
        Livestock,
        /// <summary>
        /// The product is not only an animal, but a domesticated pet. Rather than being raised
        /// to be killed and consumed or for it's other products, it is raised for companionship
        /// and secondary effects like hunting pests.
        /// </summary>
        Pet,
        /// <summary>
        /// The product is a Military good, not meant to be used as inputs into a process.
        /// Military goods have an expanded information containing information about it's use
        /// and how effective it is.
        /// </summary>
        MilitaryGood,
        /// <summary>
        /// The product is fixed in locations, once created, it is tied to the territory it was made
        /// in and cannot be moved under normal circumstances.
        /// </summary>
        Fixed,
        /// <summary>
        /// The product is meant to be used as a currency primarily. As such it will take
        /// precedence when using barter, and if barter is outlawed, then currency is the only
        /// thing that can be traded.
        /// </summary>
        Currency,
        /// <summary>
        /// The product is not a physical good, but instead a service rendered.
        /// This type of good cannot be maintained, fails into nothing, is always 
        /// consumed, and cannot be transported.
        /// </summary>
        Service,
        /// <summary>
        /// This product cannot have variants.
        /// </summary>
        Invariant,
        /// <summary>
        /// This product's generic cannot be made, instead only variants are allowed.
        /// This is often used for products like claims or the like which are connected
        /// to territories, firms, or the like.
        /// </summary>
        Abstract,
        /// <summary>
        /// This product is a liquid, requiring liquid storage to contain, and will flow into
        /// waterways in the environment.
        /// </summary>
        Liquid,
        /// <summary>
        /// This product is a gas, requiring gas storage to contain and will flow into the 
        /// atmosphere when released into the environment.
        /// </summary>
        Gas,
        /// <summary>
        /// Atomic<P;N>, the product is an elemental product. As such, it can be reduced into
        /// protons and neutrons. Allowing transmutation and the like to exist.
        /// </summary>
        Atomic,
        /// <summary>
        /// Energy<J>, the product is or contains some form of energy. J is the amount of energy
        /// a unit of the product contains. This energy is released as heat when it fails.
        /// </summary>
        Energy
    }
}
