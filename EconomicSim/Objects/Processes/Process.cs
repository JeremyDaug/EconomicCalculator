using System.Text.Json.Serialization;
using EconomicSim.Objects.Pops;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;
using Ninject.Infrastructure.Language;

namespace EconomicSim.Objects.Processes
{
    /// <summary>
    /// Process Data Class
    /// </summary>
    [JsonConverter(typeof(ProcessJsonConverter))]
    public class Process : IProcess
    {
        public Process()
        {
            ProcessTags = new Dictionary<ProcessTag, Dictionary<string, object>?>();
            ProcessProducts = new List<ProcessProduct>();
            ProcessWants = new List<ProcessWant>();
        }
        
        /// <summary>
        /// The Process Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Process Name, must be a unique combo with Variant Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Process Variant Name, must be unique combo with Name.
        /// </summary>
        public string VariantName { get; set; }

        /// <summary>
        /// The Minimum time the Process Takes.
        /// </summary>
        public decimal MinimumTime { get; set; }

        #region InCapOut

        /// <summary>
        /// All the products that go into or come out of the process.
        /// </summary>
        public List<ProcessProduct> ProcessProducts { get; set; }
        IReadOnlyList<IProcessProduct> IProcess.ProcessProducts => ProcessProducts;

        /// <summary>
        /// All the wants that go into or come out of the process.
        /// </summary>
        public List<ProcessWant> ProcessWants { get; set; }
        IReadOnlyList<IProcessWant> IProcess.ProcessWants => ProcessWants;

        /// <summary>
        /// Input Products, consumed by the process.
        /// </summary>
        public List<ProcessProduct> InputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Input)
                .ToList();
        IReadOnlyList<IProcessProduct> IProcess.InputProducts
            => InputProducts;

        /// <summary>
        /// Capital Products, used, but not consumed by the process.
        /// </summary>
        public List<ProcessProduct> CapitalProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Capital)
                .ToList();

        IReadOnlyList<IProcessProduct> IProcess.CapitalProducts
            => CapitalProducts;

        /// <summary>
        /// Output Products, resulting products from the process.
        /// </summary>
        public List<ProcessProduct> OutputProducts
            => ProcessProducts.Where(x => x.Part == ProcessPartTag.Output)
                .ToList();

        IReadOnlyList<IProcessProduct> IProcess.OutputProducts
            => OutputProducts;

        /// <summary>
        /// Input Wants, consumed by the process.
        /// </summary>
        public List<ProcessWant> InputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Input)
                .ToList();

        IReadOnlyList<IProcessWant> IProcess.InputWants
            => InputWants;

        /// <summary>
        /// Capital Wants, used by the process.
        /// </summary>
        public List<ProcessWant> CapitalWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Capital)
                .ToList();
        IReadOnlyList<IProcessWant> IProcess.CapitalWants => CapitalWants;

        /// <summary>
        /// Output Wants, created by the process.
        /// </summary>
        public List<ProcessWant> OutputWants
            => ProcessWants.Where(x => x.Part == ProcessPartTag.Output)
            .ToList();
        IReadOnlyList<IProcessWant> IProcess.OutputWants => OutputWants;

        #endregion InCapOut

        /// <summary>
        /// The Process's Tags.
        /// </summary>
        public Dictionary<ProcessTag, Dictionary<string, object>?> ProcessTags { get; set; }
        Dictionary<ProcessTag, Dictionary<string, object>?> IProcess.ProcessTags => ProcessTags;

        /// <summary>
        /// The Skill the process Uses.
        /// </summary>
        public ISkill? Skill { get; set; }

        /// <summary>
        /// The minimum level of the skill.
        /// </summary>
        public decimal SkillMinimum { get; set; }

        /// <summary>
        /// The Maximum level of a skill for the process.
        /// </summary>
        public decimal SkillMaximum { get; set; }

        /// <summary>
        /// Whether the process is functional or not.
        /// </summary>
        public bool Fractional
        {
            get
            {
                return
                    !InputProducts.Any(x => !x.Product.Fractional) ||
                    !OutputProducts.Any(x => !x.Product.Fractional);
            }
        }

        /// <summary>
        /// A quick description of the process.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Location of the Icon representing the process.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The tech required see/do this process.
        /// </summary>
        public ITechnology? TechRequirement { get; set; }

        public IProcessNode? Node { get; set; }
        ITechnology? IProcess.TechRequirement => TechRequirement;
        
        /// <summary>
        /// Retrieve the name of the process.
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            if (string.IsNullOrWhiteSpace(VariantName))
                return Name;
            return Name + "(" + VariantName + ")";
        }
        
        /// <summary>
        /// Do the process's work. Takes in inputs and capitals,
        /// outputs the product made/consumed, capital used, and wants made or consumed.
        /// </summary>
        /// <param name="iterations">How many iterations to try and complete.</param>
        /// <param name="progress">The progress being brought in (will round up iterations sought).</param>
        /// <param name="products">The products available to be consumed/used.</param>
        /// <param name="wants">The wants available to be used directly.</param>
        /// <returns>
        /// Successes is how many iterations were completed.
        /// Progress is how much overflow remains.
        /// productChange is the product that has been created/consumed (+ for created, - for destroyed).
        /// productUsed is the products that have been used as capital (positive values).
        /// wantsChange is the change in wants, cap wants are still consumed.
        /// </returns>
        public (int successes, decimal progress,
                Dictionary<IProduct, decimal> productChange,
                Dictionary<IProduct, decimal> productUsed,
                Dictionary<IWant, decimal> wantsChange)
            DoProcess(decimal iterations, decimal progress,
                Dictionary<IProduct, decimal> products,
                Dictionary<IWant, decimal> wants)
        {
            // get the target iterations desired and no more.
            var target = iterations;
            if (progress != 0)
                target += 1 - progress;
            
            // get optional items
            // TODO work on optional inclusions later. Will likely need small rework.
            // Optional items only matter if there is a fixed item elsewhere in the process.
            /*var optionalProdInputs = InputProducts
                .Where(x => x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            var optionalWantInputs = InputWants
                .Where(x => x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target);
            var optionalProdCapitals = CapitalProducts
                .Where(x => x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            var optionalWantCapitals = CapitalWants
                .Where(x => x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target); */

            // get the max targets for inputs and capitals
            var inputProds = InputProducts
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            /*var inputWants = InputWants
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target);*/
            var capitalProds = CapitalProducts
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            /*var capitalWants = CapitalWants
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target);*/
            // TODO update to deal with Want based stuff later
            // input wants can be outputs from previous processes or default come from consumption processes.
            // capital wants can only come from use processes.
            
            // find how much is available in given resources
            var keys = inputProds.Select(x => x.Key).ToList();
            keys.AddRange(capitalProds.Keys);
            // get all the keys for input and captial products.
            var keyFin = keys.Distinct();
            decimal reduction = 1;
            foreach (var resource in keyFin)
            {
                decimal input = 0, capital = 0, available = 0;
                inputProds.TryGetValue(resource, out input);     // input desired
                capitalProds.TryGetValue(resource, out capital); // capital desired
                products.TryGetValue(resource, out available);     // product available

                // get the lower between the available product and the desired product.
                var current = Math.Min(input + capital, available);
                // if current / the desired is less than the current reduction, reduce further. 
                reduction = Math.Min(reduction, current / (input + capital));
            }
            
            // if reduction is 0 return with empty values, no iterations could be done.
            if (reduction == 0)
                return (0, 0,
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IWant, decimal>());
            
            // with the reduction found (and it not being zero) we find out how
            // much of everything we use/consume and output
            var endIter = target * reduction + progress;

            var prodChange = new Dictionary<IProduct, decimal>();
            var used = new Dictionary<IProduct, decimal>();
            var wantChange = new Dictionary<IWant, decimal>();
            
            // get the product consumed
            foreach (var input in InputProducts)
                prodChange[input.Product] = -input.Amount * endIter;
            // get product output
            foreach (var output in OutputProducts)
            {
                if (prodChange.ContainsKey(output.Product))
                    prodChange[output.Product] += output.Amount * endIter;
                else
                    prodChange[output.Product] = output.Amount * endIter;
            }
            // get product used
            foreach (var cap in CapitalProducts)
                used[cap.Product] = cap.Amount * endIter;
            
            // get want consumed by inputs
            foreach (var input in InputWants)
                wantChange[input.Want] = 0; // TODO update these to actually use wants.
            // and by capitals
            foreach (var input in CapitalWants)
                wantChange[input.Want] = 0; // TODO update these to actually use wants.
            // and add want outputs
            foreach (var input in OutputWants)
                wantChange[input.Want] = 0; // TODO update these to actually use wants.

            return ((int)Math.Floor(endIter),
                    endIter % 1, 
                    prodChange, used, wantChange);
        }

        #region BaseOverrides

        public override string ToString()
        {
            return GetName();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Process);
        }

        public bool Equals(Process? obj)
        {
            if (obj == null)
                return false;
            return string.Equals(GetName(), obj.GetName());
        }

        public override int GetHashCode()
        {
            return GetName().GetHashCode();
        }

        #endregion

        public IReadOnlyList<IProcessProduct> GetProductsByName(string name)
        {
            return ProcessProducts.Where(x => x.Product.GetName() == name).ToList();
        }

        public IReadOnlyList<IProcessProduct> GetProductsByName(string name, ProcessPartTag part)
        {
            return ProcessProducts
                .Where(x => x.Part == part)
                .Where(x => x.Product.GetName() == name)
                .ToList();
        }

        /// <summary>
        /// Gets the amount of a product part, taking chance into account.
        /// </summary>
        /// <param name="part">The part we are calculating.</param>
        /// <returns>the projected amount.</returns>
        public decimal ProjectedPartAmount(IProcessProduct part)
        {
            if (!ProcessProducts.Contains(part))
                throw new ArgumentException("Process Product does not belong to this process.");
            var result = part.Amount;
            
            if (part.Part == ProcessPartTag.Input)
                result *= -1; // if the part is an input, negate it.
            else if (part.Part == ProcessPartTag.Capital)
                result = 0; // if it's a capital, remove it (not consumed or produced).
            else if (part.Part == ProcessPartTag.Output)
            { // if it's an output check that it's a chance
                if (part.TagData.Any(x => x.tag == ProductionTag.Chance))
                { // if it's a chance modify the output amount by the probability of it happening.
                    // TODO This is most likely broken, will need to update/change process tags to store chance properly on creation.
                    decimal chance = (int) part.TagData
                        .Single(x => x.tag == ProductionTag.Chance)
                        .parameters["Weight"] / (int) ProcessTags[ProcessTag.Chance]["Total"];
                    result = chance * result;
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get's the projected amount of product produced and/or consumed by the
        /// process
        /// </summary>
        /// <param name="product">The product in question.</param>
        /// <returns>The total amount, subtracts inputs from outputs.</returns>
        public decimal ProjectedProductAmount(IProduct product)
        {
            var parts = ProcessProducts
                .Where(x => Equals(x.Product, product));

            decimal result = 0;

            foreach (var part in parts)
            {
                result += ProjectedPartAmount(part);
            }

            return result;
        }
        
        /// <summary>
        /// Gets the projected amount of product involved in a part of the process.
        /// </summary>
        /// <param name="product">The product in question.</param>
        /// <param name="part">Which part (input, capital, output) to return.</param>
        /// <returns>The total amount, inputs are negative.</returns>
        public decimal ProjectedProductAmount(IProduct product, ProcessPartTag part)
        {
            var sections = ProcessProducts
                .Where(x => Equals(x.Product, product) && x.Part == part);

            decimal result = 0;

            foreach (var section in sections)
            {
                if (part == ProcessPartTag.Output) // outputs might be chances
                    result += ProjectedPartAmount(section);
                else if (part == ProcessPartTag.Input) // inputs are always negative
                    result -= section.Amount;
                else // capital is positive.
                    result += section.Amount;
            }

            return result;
        }
    }
}
