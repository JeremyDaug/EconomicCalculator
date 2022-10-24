using System.Text.Json.Serialization;
using EconomicSim.Helpers;
using EconomicSim.Objects.Processes.ProcessTags;
using EconomicSim.Objects.Processes.ProductionTags;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Skills;
using EconomicSim.Objects.Technology;
using EconomicSim.Objects.Wants;

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
        /// Checks if two processes are compatible with each other and
        /// can be added together.
        /// Currently, if either process has ANY tags, they are invalid for addition.
        /// </summary>
        /// <returns></returns>
        public bool ValidProcessTagAddition(IProcess other)
        {
            if (ProcessTags.Any() || other.ProcessTags.Any())
                return false;
            return true;
        }

        #region ProcessAdditions

        /// <summary>
        /// Adds another process to this process and outputs the resulting fusion.
        /// </summary>
        /// <param name="other">The process we are adding to this one</param>
        /// <returns>The resulting sum between the two.</returns>
        /// <exception cref="ProcessTagMismatchException">
        /// If the processes being added have incompatible tags.
        /// </exception>
        public IProcess AddProcess(IProcess other)
        {
            var result = new Process();
            // check that it's process Tags do not clash
            if (!ValidProcessTagAddition(other))
                throw new ProcessTagMismatchException("Processes with tags cannot be added together."); 
            // set it's easy data.
            result.Name = $"{GetName()} + {other.GetName()}";
            result.MinimumTime = MinimumTime + other.MinimumTime;
            result.Skill = Skill;
            result.SkillMinimum = Math.Min(SkillMinimum, other.SkillMinimum);
            result.SkillMaximum = Math.Max(SkillMaximum, other.SkillMaximum);
            result.Icon = Icon;
            result.TechRequirement = TechRequirement;
            // do the addition of the components.
            foreach (var input in InputProducts)
            {
                result.ProcessProducts.Add(
                    new ProcessProduct
                    {
                        Product = input.Product,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Input,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in InputWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Input,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in CapitalProducts)
            {
                result.ProcessProducts.Add(
                    new ProcessProduct
                    {
                        Product = input.Product,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Capital,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in CapitalWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Capital,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in OutputProducts)
            {
                result.ProcessProducts.Add(
                    new ProcessProduct
                    {
                        Product = input.Product,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Output,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in OutputWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Output,
                        TagData = input.TagData.ToList()
                    });
            }
            // add the other process parts,
            // TODO, I'm a lazy fuck, so these are just tacked onto the end,
            // not properly added to existing ones. Should be tho
            foreach (var input in other.InputProducts)
            {
                result.ProcessProducts.Add(
                new ProcessProduct
                {
                    Product = input.Product,
                    Amount = input.Amount,
                    Part = ProcessPartTag.Input,
                    TagData = input.TagData.ToList()
                });
            }
            foreach (var input in other.InputWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Input,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in other.CapitalProducts)
            {
                result.ProcessProducts.Add(
                    new ProcessProduct
                    {
                        Product = input.Product,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Capital,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in other.CapitalWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Capital,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in OutputProducts)
            {
                result.ProcessProducts.Add(
                    new ProcessProduct
                    {
                        Product = input.Product,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Output,
                        TagData = input.TagData.ToList()
                    });
            }
            foreach (var input in OutputWants)
            {
                result.ProcessWants.Add(
                    new ProcessWant
                    {
                        Want = input.Want,
                        Amount = input.Amount,
                        Part = ProcessPartTag.Output,
                        TagData = input.TagData.ToList()
                    });
            }

            return result;
        }

        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <returns>The combined process resulting from the combination.</returns>
        /// <exception cref="ProcessTagMismatchException"></exception>
        /// <exception cref="ProcessInterfaceMismatchException"></exception>
        /// <exception cref="ProcessInterfaceTagMismatchException"></exception>
        public IProcess InputProcess(IProcess other)
        {
            if (!ValidProcessTagAddition(other))
                throw new ProcessTagMismatchException($"{other.GetName()} and {GetName()} have incompatible tags.");

            // get the outputs which exist in inputs
            var matchedProducts = other
                .OutputProducts
                .Select(x => x.Product)
                .Intersect(InputProducts.Select(x => x.Product))
                .ToList();
            var matchedWants = other
                .OutputWants
                .Select(x => x.Want)
                .Intersect(InputWants.Select(x => x.Want))
                .ToList();
            
            if (!(matchedProducts.Any() || matchedWants.Any()))
                throw new ProcessInterfaceMismatchException($"Process {other.GetName()} must output an input of {GetName()}");
            
            // check that these matches don't have Tags
            if (other.OutputProducts
                    .Where(x => matchedProducts.Contains(x.Product))
                    .Any(x => x.TagData.Any()) ||
                InputProducts
                    .Where(x => matchedProducts.Contains(x.Product))
                    .Any(x => x.TagData.Any()))
                throw new ProcessInterfaceTagMismatchException("Inputs being fed in cannot have any tags.");
            if (other.OutputWants
                    .Where(x => matchedWants.Contains(x.Want))
                    .Any(x => x.TagData.Any()) ||
                InputWants
                    .Where(x => matchedWants.Contains(x.Want))
                    .Any(x => x.TagData.Any()))
                throw new ProcessInterfaceTagMismatchException("Outputs being fed cannot have any tags.");

            throw new NotImplementedException();
        }

        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <param name="target">
        /// What product we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The combined process resulting from the combination.</returns>
        public IProcess InputProcess(IProcess other, IProduct target)
        {
            if (!ValidProcessTagAddition(other))
                throw new ProcessTagMismatchException($"{other.GetName()} and {GetName()} have incompatible tags.");

            // get the outputs which exist in inputs
            var matchedProducts = other
                .OutputProducts
                .Select(x => x.Product)
                .Intersect(InputProducts.Select(x => x.Product))
                .ToList();
            var matchedWants = other
                .OutputWants
                .Select(x => x.Want)
                .Intersect(InputWants.Select(x => x.Want))
                .ToList();
            
            if (!(matchedProducts.Any() || matchedWants.Any()))
                throw new ProcessInterfaceMismatchException($"Process {other.GetName()} must output an input of {GetName()}");
            
            // check that these matches don't have Tags
            if (other.OutputProducts
                    .Where(x => matchedProducts.Contains(x.Product))
                    .Any(x => x.TagData.Any()) ||
                InputProducts
                    .Where(x => matchedProducts.Contains(x.Product))
                    .Any(x => x.TagData.Any()))
                throw new ProcessInterfaceTagMismatchException("Inputs being fed in cannot have any tags.");
            if (other.OutputWants
                    .Where(x => matchedWants.Contains(x.Want))
                    .Any(x => x.TagData.Any()) ||
                InputWants
                    .Where(x => matchedWants.Contains(x.Want))
                    .Any(x => x.TagData.Any()))
                throw new ProcessInterfaceTagMismatchException("Outputs being fed cannot have any tags.");

            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Feeds the other process into this process. Any outputs of other which are
        /// inputs for this cancel, otherwise inputs, capital, and outputs are added
        /// together.
        /// </summary>
        /// <param name="other">The process to bring in as an input to this one.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The combined process resulting from the combination.</returns>
        public IProcess InputProcess(IProcess other, IWant target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <returns>The resulting combined process.</returns>
        public IProcess OutputToProcess(IProcess other)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <param name="target">
        /// What product we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The resulting combined process.</returns>
        public IProcess OutputToProcess(IProcess other, IProduct target)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Feeds this process's outputs into the <see cref="other"/> process as
        /// inputs. If this process's outputs are inputs to other, they cancel out,
        /// otherwise they add together.
        /// </summary>
        /// <param name="other">The process we are moving our outputs to.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns>The resulting combined process.</returns>
        public IProcess OutputToProcess(IProcess other, IWant target)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Feeds the <see cref="other"/> process into this process as a source
        /// for capital wants.
        /// </summary>
        /// <remarks>
        /// This does not remove capital good product requirements.
        /// </remarks>
        /// <param name="other">The process we are taking in for capital goods.</param>
        /// <returns></returns>
        public IProcess TakeAsCapital(IProcess other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Feeds the <see cref="other"/> process into this process as a source
        /// for capital wants.
        /// </summary>
        /// <remarks>
        /// This does not remove capital good product requirements.
        /// </remarks>
        /// <param name="other">The process we are taking in for capital goods.</param>
        /// <param name="target">
        /// What want we are trying to satisfy.
        /// The other process will be multiplied to satisfy this.
        /// </param>
        /// <returns></returns>
        public IProcess TakeAsCapital(IProcess other, IWant target)
        {
            throw new NotImplementedException();
        }

        #endregion

        public decimal ProjectedWantAmount(IWant want, ProcessPartTag part)
        {
            var sections = ProcessWants
                .Where(x => x.Part == part && Equals(x.Want, want));

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

        /// <summary>
        /// Given products desired for input and capital, as well as the available products
        /// to use, return how much we need to reduce our inputs and capitals to not overdraw
        /// from the available products.
        /// 1 is no reduction, 0 is unable to do it.
        /// </summary>
        /// <param name="inputProds">Products for input use.</param>
        /// <param name="capitalProds">Products for capital use.</param>
        /// <param name="availableProducts">The products available</param>
        /// <returns>The reduction to not overdraw from products.</returns>
        private decimal ProcessReductionBasedOnProducts(
            Dictionary<IProduct, decimal> inputProds,
            Dictionary<IProduct, decimal> capitalProds,
            Dictionary<IProduct, decimal> availableProducts)
        {
            // find how much is available in given resources
            var keys = inputProds.Select(x => x.Key).ToList();
            keys.AddRange(capitalProds.Keys);
            // get all the keys for input and captial products.
            var keyFin = keys.Distinct();
            decimal reduction = 1;
            foreach (var resource in keyFin)
            {
                decimal input = 0, capital = 0, available = 0;
                inputProds.TryGetValue(resource, out input); // input desired
                capitalProds.TryGetValue(resource, out capital); // capital desired
                availableProducts.TryGetValue(resource, out available); // product available

                // get the lower between the available product and the desired product.
                var current = Math.Min(input + capital, available);
                // if current / the desired is less than the current reduction, reduce further. 
                reduction = Math.Min(reduction, current / (input + capital));
                if (reduction == 0) return reduction; // if reduction reached 0, breakout and return.
            }

            return reduction;
        }

        /// <summary>
        /// Given the total wants desired and the wants available how much do we need to
        /// reduce our desires to not overdraw.
        /// 1 means no reduction, 0 means incapable of meeting at least one of them.
        /// </summary>
        /// <param name="inputWants">The amount of wants desired.</param>
        /// <param name="availableWants">The wants available.</param>
        /// <returns></returns>
        private decimal ProcessReductionBasedOnWants(
            Dictionary<IWant, decimal> inputWants,
            Dictionary<IWant, decimal> availableWants)
        {
            decimal reduction = 1;
            // do what we just did, but for wants
            foreach (var want in inputWants.Keys)
            {
                var input = inputWants[want];
                availableWants.TryGetValue(want, out var available); // wants available
                
                // get the lower between the two
                var current = Math.Min(input, available);
                // reduce further if needed
                reduction = Math.Min(reduction, current / input);
                if (reduction == 0) break; // if reduced to 0 breakout
            }
            
            // if reduction is 0 return with empty values, no iterations could be done.
            return reduction;
        }

        /// <summary>
        /// Checks if this process is valid to be run or not.
        /// If this process has a 
        /// </summary>
        /// <returns></returns>
        public bool ProcessValid()
        {
            if (CapitalWants.Any())
                return false;
            return true;
        }
        
        /// <summary>
        /// Do the process's work. Takes in inputs and capitals,
        /// outputs the product made/consumed, capital used, and wants made or consumed.
        /// Does not remove items from inputs, leaves that to the caller.
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
            // TODO also work on Skill bonus. It would go around here as well.
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
            // TODO does not take into account items which may have multiples in the same category.
            var inputProds = InputProducts
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            var inputWants = InputWants
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target);
            var capitalProds = CapitalProducts
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Product,
                    x => x.Amount * target);
            if (CapitalWants.Any())
                throw new NotImplementedException("DoProcess");
            /*var capitalWants = CapitalWants
                .Where(x => !x.ContainsTag(ProductionTag.Optional))
                .ToDictionary(x => x.Want,
                    x => x.Amount * target);*/
            // TODO update to deal with Want based stuff later
            // TODO Reevaluate how Capital Wants should be dealt with
                // Capital wants 'must' come from Use Processes, but unless we find a
                // product to use here, we'll never know if a want came from a use process or not. 
                // Best bet is to force processes with Capital wants to input a use process first
                // before allowing it to continue.
            // input wants can be outputs from previous processes or default come from consumption processes.
            // capital wants can only come from use processes.
            
            // find how much is available in given resources
            var reduction = ProcessReductionBasedOnProducts(inputProds, capitalProds, products);
            
            // if reduction is 0 return with empty values, no iterations could be done.
            if (reduction == 0)
                return (0, 0,
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IWant, decimal>());
            
            // do what we just did, but for wants
            var wantReduction = ProcessReductionBasedOnWants(inputWants, wants);
            // if reduction is 0 return with empty values, no iterations could be done.
            if (wantReduction == 0)
                return (0, 0,
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IProduct, decimal>(),
                    new Dictionary<IWant, decimal>());

            // get the smaller between product and want reduction.
            reduction = Math.Min(reduction, wantReduction);
            
            // with the reduction found (and it not being zero) we find out how
            // much of everything we use/consume and output
            // Don't add Progress back in as we allow fractional products in inventory.
            var endIter = target * reduction;

            var prodChange = new Dictionary<IProduct, decimal>();
            var used = new Dictionary<IProduct, decimal>();
            var wantChange = new Dictionary<IWant, decimal>();
            
            // TODO update this to possibly handle multiples of the same item.
            // get the product consumed
            foreach (var input in InputProducts)
                prodChange.AddOrInclude(input.Product, -input.Amount * endIter);
            // get the wants consumed
            foreach (var input in InputWants)
                wantChange.AddOrInclude(input.Want, -input.Amount * endIter);
            // get product output
            foreach (var output in OutputProducts)
                prodChange.AddOrInclude(output.Product, output.Amount * endIter);
            // get wants outputted
            foreach (var output in OutputWants)
                wantChange.AddOrInclude(output.Want, output.Amount * endIter);
            // get product used
            foreach (var cap in CapitalProducts)
                used[cap.Product] = cap.Amount * endIter;
            // we don't actually allow Capital wants, and thus can't 'use' them.

            return ((int)Math.Floor(endIter),
                    endIter % 1, 
                    prodChange, used, wantChange);
        }

        private const double UnskilledDivisor = 0.5;
        private const double SkilledBonus = 1.2;
        private const decimal OverskilledBonus = 0.1m;

        public decimal SkillThroughputModifier(decimal skillLevel)
        {
            if (Skill == null)
                return 1; // if process has no skill, default to 1.
            
            if (skillLevel < SkillMinimum)
            {
                return (decimal) Math.Pow(UnskilledDivisor, (double)(SkillMinimum - skillLevel));
            }

            if (skillLevel >= SkillMinimum && skillLevel <= SkillMaximum)
                return (decimal) Math.Pow(SkilledBonus, (double) (skillLevel - SkillMinimum));

            var max = SkillThroughputModifier(SkillMaximum);
            var step = max * OverskilledBonus;
            return max + step * (skillLevel - SkillMaximum);
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
        /// Gets the amount of a product part, taking chance into account.
        /// </summary>
        /// <param name="part">The part we are calculating.</param>
        /// <returns>the projected amount.</returns>
        public decimal ProjectedPartAmount(IProcessWant part)
        {
            if (!ProcessWants.Contains(part))
                throw new ArgumentException("Process Want does not belong to this process.");
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
