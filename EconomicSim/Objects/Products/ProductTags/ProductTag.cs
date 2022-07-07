namespace EconomicSim.Objects.Products.ProductTags
{
    /// <summary>
    /// The existant product tags in our system.
    /// </summary>
    public enum ProductTag
    {
        /// <summary>
        /// Luxury(D;S), the good becomes more efficient at satisfying
        /// desires as its' price rises above the average and falls below it.
        /// D is the Multiplier of the effect. S is the name of the want or desire.
        /// S may be a Want, self, for the specific product, or all for everything the
        /// product gives.
        /// </summary>
        Luxury,
        /// <summary>
        /// Bargain(D;S), the product becomes more efficient at satisfying
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
        /// Claim(S).
        /// The product is a claim of ownership on some other product or thing, such as
        /// a deed to land or ownership of a home for renting. This allows ownership of
        /// a <see cref="Fixed"/> good to leave a territory, without actually moving it.
        /// S is for what it claims, Product is the only valid options currently.
        /// </summary>
        Claim,
        /// <summary>
        /// Share(S).
        /// The product is a Share of ownership on a firm. This gives them a portion of the
        /// profits / dividends of the company and can allow them to vote in board decisions.
        /// S is for what it claims, Firm is the only valid options currently.
        /// </summary>
        Share,
        /// <summary>
        /// The product is exclusively a consumeable, using it for itself or it's wants
        /// consumes the product.
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
        /// Atomic(P;N), the product is an elemental product. As such, it can be reduced into
        /// protons and neutrons. Allowing transmutation and the like to exist.
        /// </summary>
        Atomic,
        /// <summary>
        /// Energy(J), the product is or contains some form of energy. J is the amount of energy
        /// a unit of the product contains. This energy is released as heat when it fails.
        /// </summary>
        Energy,
        /// <summary>
        /// Storage(type,Volume,Mass)
        /// This product can store other products. It can store goods of 
        /// the defined type (Basic, Liquid, Gas, Energy), as well as 
        /// a defined volume and mass. Setting either to -1 gives it infinite
        /// storage (IE magical Storage).
        /// </summary>
        Storage,
        /// <summary>
        /// Nondegrading
        /// The product does not degrade, any 'failure' roll is skipped for it.
        /// </summary>
        Nondegrading
    }

    public static class ProductTagMethods
    {
        /// <summary>
        /// Gets the parameter Names for a Product Tag
        /// </summary>
        /// <param name="tag">The tag we are getting parameters for.</param>
        /// <returns>A list of all parameter names.</returns>
        public static List<string> GetParameterNames(this ProductTag tag)
        {
            var result = new List<string>();

            switch (tag)
            {
                case ProductTag.Atomic:
                    result.Add("Protons");
                    result.Add("Neutrons");
                    break;
                case ProductTag.Energy:
                    result.Add("Joules");
                    break;
                case ProductTag.Storage:
                    result.Add("Type");
                    result.Add("Volume");
                    result.Add("Mass");
                    break;
                case ProductTag.Luxury:
                    result.Add("Multiplier");
                    result.Add("Want");
                    break;
                case ProductTag.Bargain:
                    result.Add("Multiplier");
                    result.Add("Want");
                    break;
                case ProductTag.Claim:
                    result.Add("Product");
                    break;
                case ProductTag.Share:
                    result.Add("Firm");
                    break;
                default: 
                    break;
            }
            
            return result;
        }

        public static Dictionary<string, object>? ProcessParameters(this ProductTag tag,
            Dictionary<string, object>? parameters)
        {
            // if parameters list is null, just drop back.
            var result = new Dictionary<string, object>();

            switch (tag)
            {
                case ProductTag.Abstract:
                case ProductTag.Liquid:
                case ProductTag.Gas:
                case ProductTag.Consumable:
                case ProductTag.ConsumerGood:
                case ProductTag.Animal:
                case ProductTag.Livestock:
                case ProductTag.Pet:
                case ProductTag.MilitaryGood:
                case ProductTag.Fixed:
                case ProductTag.Currency:
                case ProductTag.Service:
                case ProductTag.Invariant:
                case ProductTag.Nondegrading:
                case ProductTag.Public:
                    if (parameters == null)
                        return null;
                    if (parameters.Any())
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    return null; // if you somehow got here (because Params were null) return null.
                case ProductTag.Atomic:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 2 )
                        throw new ArgumentException("Atomic should have 2 parameters.");
                    try
                    {
                        result.Add("Protons", int.Parse(parameters["Protons"].ToString()));
                        result.Add("Neutrons", int.Parse(parameters["Neutrons"].ToString()));
                        if ((int) result["Protons"] < 1 || (int) result["Neutrons"] < 1)
                            throw new ArgumentException("Protons and Neutrons must be Integer Numbers greater than 0.");
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException("Protons and Neutrons must be Integer Numbers greater than 0.", e);
                    }
                    break;
                case ProductTag.Energy:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 1)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    try
                    {
                        result.Add("Joules", decimal.Parse(parameters["Joules"].ToString()));
                        if ((decimal)result["Joules"] <= 0)
                            throw new ArgumentException("Joules must be a decimal greater than 0.");
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException("Joules must be a decimal greater than 0.", e);
                    }
                    break;
                case ProductTag.Storage:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 3)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    try
                    {
                        result.Add("Type", (StorageType) Enum.Parse(typeof(StorageType), parameters["Type"].ToString()));
                        result.Add("Volume", decimal.Parse(parameters["Volume"].ToString()));
                        result.Add("Mass", decimal.Parse(parameters["Mass"].ToString()));
                        if (((decimal)result["Volume"] < 0 && (decimal)result["Volume"] != -1) ||
                            ((decimal)result["Mass"] < 0 && (decimal)result["Mass"] != -1))
                            throw new ArgumentException("Volume and Mass must be either greater than 0 or equal to -1");
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException("Volume and Mass must be either greater than 0 or equal to -1", e);
                    }
                    break;
                case ProductTag.Luxury:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 2)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    try
                    {
                        result.Add("Multiplier", decimal.Parse(parameters["Multiplier"].ToString()));
                        result.Add("Want", DataContext.Instance.Wants[parameters["Want"].ToString()]);
                        if ((decimal) result["Multiplier"] <= 0)
                            throw new ArgumentException("Multiplier must be greater than 0.");
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException("Multiplier must be a decimal greater than 0.", e);
                    }
                    break;
                case ProductTag.Bargain:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 2)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    try
                    {
                        result.Add("Multiplier", decimal.Parse(parameters["Multiplier"].ToString()));
                        result.Add("Want", DataContext.Instance.Wants[parameters["Want"].ToString()]);
                        if ((decimal) result["Multiplier"] <= 0)
                            throw new ArgumentException("Multiplier must be greater than 0.");
                    }
                    catch (FormatException e)
                    {
                        throw new ArgumentException("Multiplier must be a decimal greater than 0.", e);
                    }
                    break;
                case ProductTag.Claim:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 1)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    result.Add("Product", DataContext.Instance.Products[parameters["Product"].ToString()]);
                    break;
                case ProductTag.Share:
                    if (parameters == null)
                        throw new ArgumentException($"Parameters for {tag} cannot be null.");
                    if (parameters.Count() != 1)
                        throw new ArgumentException("Invalid Parameters in Tag.");
                    result.Add("Firm", DataContext.Instance.Firms[parameters["Firm"].ToString()]);
                    break;
            }

            return result;
        }
    }
}
