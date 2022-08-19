using System.Text.Json.Serialization;
using EconomicSim.Helpers;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Market;
using EconomicSim.Objects.Pops.Culture;
using EconomicSim.Objects.Pops.Species;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;

namespace EconomicSim.Objects.Pops
{
    /// <summary>
    /// Pop group data class.
    /// </summary>
    [JsonConverter(typeof(PopJsonConverter))]
    public class PopGroup : IPopGroup
    {
        public PopGroup()
        {
            Property = new Dictionary<IProduct, decimal>();
            Species = new List<SpeciesCount>();
            Cultures = new List<CultureCount>();
            ForSale = new Dictionary<IProduct, decimal>();
        }

        /// <summary>
        /// Pop Group Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A Generated name for a Pop Group, should be synthesized from
        /// Market, Firm, and Job.
        /// </summary>
        public string Name => $"{Job.Name} of {Firm.Name} in {Market.Name}";

        /// <summary>
        /// The total number of people in the pop group.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The job the pop group is attached to.
        /// </summary>
        public Job Job { get; set; }
        IJob IPopGroup.Job => Job;

        /// <summary>
        /// The firm the pop group is attached to.
        /// </summary>
        public Firm Firm { get; set; }
        IFirm IPopGroup.Firm => Firm;

        /// <summary>
        /// The market the pop resides in.
        /// </summary>
        public IMarket Market { get; set; }

        /// <summary>
        /// The skill the pop has.
        /// </summary>
        public Skill Skill { get; set; }
        ISkill IPopGroup.Skill => Skill;

        /// <summary>
        /// The lower Skill Level of the Population Group.
        /// </summary>
        public decimal LowerSkillLevel { get; set; }
        
        /// <summary>
        /// The Highest Skill Level of the Population Group.
        /// </summary>
        public decimal HigherSkillLevel { get; set; }

        /// <summary>
        /// The property the population Owns.
        /// </summary>
        public Dictionary<IProduct, decimal> Property { get; set; }

        IReadOnlyDictionary<IProduct, decimal> IPopGroup.Property
            => Property.ToDictionary(x => (IProduct)x.Key,
                x => x.Value);

        /// <summary>
        /// The Species that makes up the pop.
        /// </summary>
        public List<SpeciesCount> Species { get; set; }

        IReadOnlyList<SpeciesCount> IPopGroup.Species
            => Species;

        /// <summary>
        /// The cultures which make up in the Pop.
        /// </summary>
        public List<CultureCount> Cultures { get; set; }

        IReadOnlyList<CultureCount> IPopGroup.Cultures
            => Cultures;
        
        /// <summary>
        /// The products desired by the pop every day.
        /// </summary>
        public IReadOnlyList<INeedDesire> Needs
        {
            get
            { // TODO make this more flexible, less sucky, and not rebuild it every time it's called.
                var result = new List<INeedDesire>();

                foreach (var species in Species)
                {
                    foreach (var desire in species.Species.Needs)
                    {
                        // check if the desire has already been added.
                        var extant = result.SingleOrDefault(x => x.Product == desire.Product && 
                                                                 x.Tier == desire.Tier);
                        if (extant == null)
                        { // if is hasn't, add it and set extant.
                            extant = new NeedDesire
                            {
                                Product = desire.Product,
                                Amount = 0,
                                Tier = desire.Tier
                            };
                            result.Add(extant);
                        }
                        // then update the amount
                        extant.Amount += desire.Amount;
                    }
                }
                // do the same with Culture desires
                foreach (var culture in Cultures)
                {
                    foreach (var desire in culture.Culture.Needs)
                    {
                        // check if the desire has already been added.
                        var extant = result.SingleOrDefault(x => x.Product == desire.Product && 
                                                                 x.Tier == desire.Tier);
                        if (extant == null)
                        { // if is hasn't, add it and set extant.
                            extant = new NeedDesire
                            {
                                Product = desire.Product,
                                Amount = 0,
                                Tier = desire.Tier
                            };
                            result.Add(extant);
                        }
                        // then update the amount
                        extant.Amount += desire.Amount;
                    }
                }
                
                return result;
            }
        }

        /// <summary>
        /// The Wants desired by the pop every day.
        /// </summary>
        public IReadOnlyList<IWantDesire> Wants
        {
            get
            {
                // TODO make this more flexible, less sucky, and not rebuild it every time it's called.
                var result = new List<IWantDesire>();

                foreach (var species in Species)
                {
                    foreach (var desire in species.Species.Wants)
                    {
                        // check if the desire has already been added.
                        var extant = result.SingleOrDefault(x => x.Want == desire.Want && 
                                                                 x.Tier == desire.Tier);
                        if (extant == null)
                        { // if is hasn't, add it and set extant.
                            extant = new WantDesire
                            {
                                Want = desire.Want,
                                Amount = 0,
                                Tier = desire.Tier
                            };
                            result.Add(extant);
                        }
                        // then update the amount
                        extant.Amount += desire.Amount;
                    }
                }
                // do the same with Culture desires
                foreach (var culture in Cultures)
                {
                    foreach (var desire in culture.Culture.Wants)
                    {
                        // check if the desire has already been added.
                        var extant = result.SingleOrDefault(x => x.Want == desire.Want && 
                                                                 x.Tier == desire.Tier);
                        if (extant == null)
                        { // if is hasn't, add it and set extant.
                            extant = new WantDesire
                            {
                                Want = desire.Want,
                                Amount = 0,
                                Tier = desire.Tier
                            };
                            result.Add(extant);
                        }
                        // then update the amount
                        extant.Amount += desire.Amount;
                    }
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets the hours of the population group
        /// Currently this is just 16 hours per day, but should be updated to change the
        /// rate based on species/culture/etc
        /// </summary>
        public decimal GetTotalHours()
        {
            // TODO make this more flexible and use available productivity based on the population's details (species, culture, etc).
            return Count * 16;
        }

        #region ICanSellRegion

        public Dictionary<IProduct, decimal> SellWeight { get; set; }
        public bool IsSelling { get; set; }

        private Dictionary<IProduct, decimal> ForSale { get; set; }
        public decimal SalePrice(IProduct product)
        { // not needed until Sell Phase is completed.
            throw new NotImplementedException();
        }

        public async Task<ICanSell> SellPhase()
        {
            // check we need to sell at all
            if (Firm.OwnershipStructure == OwnershipStructure.SelfEmployed)
            { // if firm is self-employed, the firm sells for them.
                // This may need to be altered/reversed to function properly
                // Pops need to buy their needs
                // firms need to buy their inputs
                // but self-employed pops and firms are the same, until they aren't.
                // Self-employed pops and similar firms are going to need to include their
                // workers desires to function properly. 
                // TODO make Self-Employed Workers be included under firms.
                IsSelling = false;
                return this; // set selling to false and return.
            }
            // TODO check the pop's storage to ensure they won't overflow.
            // TODO create this logic later.
            // For now, we assume that a pop cannot sell their products.
            // Forced barter 'should' limit how much they can accumulate.
            
            IsSelling = false;
            return this;
        }

        IReadOnlyDictionary<IProduct, decimal> IPopGroup.ForSale => ForSale;

        #endregion

        #region SafeProductExchange

        /// <summary>
        /// Gives all of an available product that a pop is willing to give.
        /// </summary>
        /// <param name="product">The product to give</param>
        /// <returns>the product and how much to give.</returns>
        public Dictionary<IProduct, decimal> GetAvailableProduct(Product product)
        {
            var result = new Dictionary<IProduct, decimal>();
            
            if (Property.ContainsKey(product) && ForSale.ContainsKey(product))
            { // if the pop owns the product requested and it's for sale
                // get whatever is for sale
                result.Add(product, ForSale[product]);
                // and remove what was lost from both sale and property
                Property[product] -= ForSale[product];
                ForSale.Remove(product);
            }
            
            return result;
        }
        
        /// <summary>
        /// Requests a product from the pop and gives either the requested amount, or the
        /// amount available, whichever is higher.
        /// </summary>
        /// <param name="product">The product requested</param>
        /// <param name="amount">The amount requisted.</param>
        /// <returns></returns>
        public Dictionary<IProduct, decimal> GetRequestedProduct(IProduct product, decimal amount)
        {
            var result = new Dictionary<IProduct, decimal>();
            
            if (Property.ContainsKey(product) && ForSale.ContainsKey(product))
            { // if the pop owns the product requested and it's for sale
                // get the minimum between amount requested and amount for sale
                var val = Math.Min(amount, ForSale[product]);
                // get whatever is for sale
                result.Add(product, val);
                // and remove what was lost from both sale and property
                Property[product] -= val;
                ForSale[product] -= val;
                if (ForSale[product] == 0)
                    ForSale.Remove(product);
            }
            
            return result;
        }
        
        /// <summary>
        /// Takes product from the pop up to the maximum they have.
        /// </summary>
        /// <param name="product">The product being taken.</param>
        /// <param name="amount">The amount being taken</param>
        /// <returns></returns>
        public Dictionary<IProduct, decimal> TakeProduct(Product product, decimal amount)
        {
            var result = new Dictionary<IProduct, decimal>();
            
            if (Property.ContainsKey(product))
            { // if the pop owns the product requested
                // get the minimum between amount requested and amount available
                var val = Math.Min(amount, Property[product]);
                // get whatever is available
                result.Add(product, val);
                // and remove what was lost from both sale and property
                Property[product] -= val;
                if (ForSale.ContainsKey(product))
                { // also remove it from sale
                    ForSale[product] -= amount;
                    if (ForSale[product] <= 0) // if no sale is left, remove it entirely.
                        ForSale.Remove(product);
                }
            }
            
            return result;
        }

        // these functions may lead to 
        #region UnsafeExchange

        /// <summary>
        /// Gives the pop a specified amount of product.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="amount"></param>
        /// <remarks>May lead to spontaneous product generation.</remarks>
        public void AcquireProduct(Product product, decimal amount)
        {
            if (Property.ContainsKey(product))
                Property[product] += amount;
            else
                Property[product] = amount;
        }

        #endregion

        #endregion
        
        #region ReserveItems

        /// <summary>
        /// Reserves items before anything happens during the day.
        /// Prioritization logic is TBD, so currently it reserves 2 hours of time.
        /// </summary>
        public async Task ReserveItems()
        {
            var dc = DataContext.Instance;
            // preemptively, if Shopping target is set to 0, then 
            if (ShoppingTarget == 0)
                ShoppingTarget = Needs.Count + Wants.Count;
            
            // get wants and need locally just to be sure that it's not getting them
            // over and over again 
            var wants = Wants;
            // var needs = Needs; Not needed here, but may be in future iteration.

            // if rest is included as a level 0 value, set the target value.
            if (wants.Any(x => x.Want == dc.Rest && x.Tier == 0))
                RestTarget = wants.Single(x => x.Want == dc.Rest).Amount;
            
            // calculate estimated time for shopping.
            // if shopping time is too low, add some relative to total desires
            if (ShoppingTarget <= 0)
                ShoppingTarget = Wants.Count + Needs.Count;
            else
            { // if a time has been set, do a rough calculation on how much time
                var used = ShoppingResults / ShoppingTarget;

                if (used >= 1) // if maxed, more time is likely needed. Be generous for now.
                    ShoppingResults += 5;
                // If between 0.9 and 1, don't change anything.
                else if (used < 0.9m) // for every 10 points below 0.9 reduce by 1.
                {
                    var downCorrection = Math.Floor((1 - used) * 10);
                    ShoppingResults -= downCorrection;
                }
            }
            
            // rest time and shopping time has been reserved,
            // add it to the 'for sale' block so firms can 'buy' it.
            var timeReserved = ShoppingTarget / 10 + RestTarget / 2;
            ForSale.Add(dc.Time, GetTotalHours() - timeReserved);
        }

        /// <summary>
        /// An estimate of how much time the population is giving to
        /// shopping. (defaults to the number of desires the pop has).
        /// </summary>
        private decimal ShoppingTarget;

        /// <summary>
        /// How much of the prior shopping target was used the previous day.
        /// If all of it was used, then the target should be increased, if
        /// it's within a margin of error, don't change, if it's below
        /// the margin of error, reduce the target. (allow for 10% error for now)
        /// </summary>
        private decimal ShoppingResults;

        /// <summary>
        /// The estimate of how much value the pop gives to rest per unit.
        /// </summary>
        private decimal RestValue;

        /// <summary>
        /// The target amount of rest the pop is hoping to get.
        /// They will not accept less than half of this value,
        /// they will use their rest value to try and 'buy' more.
        /// they will accept up to 1.5s more than this, but will
        /// find alternatives beyond that point.
        /// If -1 then it will ignore this value entirely,
        /// treating it like any other product with value.
        /// </summary>
        private decimal RestTarget;

        #endregion

        IDictionary<IProduct, decimal> ICanSell.ForSale { get; }
    }
}
