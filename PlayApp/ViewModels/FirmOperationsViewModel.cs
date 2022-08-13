using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using AvaEditorUI.Models;
using Avalonia.Controls;
using Avalonia.OpenGL;
using EconomicSim.Enums;
using EconomicSim.Objects;
using EconomicSim.Objects.Firms;
using EconomicSim.Objects.Jobs;
using EconomicSim.Objects.Processes;
using PlayApp.Helpers;
using ReactiveUI;

namespace PlayApp.ViewModels;

public class FirmOperationsViewModel : ViewModelBase
{
    private IDataContext dc = DataContextFactory.GetDataContext;
    private Window _window;
    private Firm parent;
    private string _selectedWorkerName;
    private FirmJobModel _selectedWorker;
    private decimal _availableHours;
    private Pair<string, decimal>? _selectedAssignment;
    private string _pricingUnit;

    public FirmOperationsViewModel()
    {
        
    }

    public FirmOperationsViewModel(Firm parent, Window window)
    {
        _window = window;
        this.parent = parent;

        CapitalAndStock = new ObservableCollection<CapStockAndSum>();
        ExpectedInputs = new ObservableCollection<Pair<string, decimal>>();
        ExpectedCapitals = new ObservableCollection<Pair<string, decimal>>();
        ExpectedOutputs = new ObservableCollection<Pair<string, decimal>>();
        
        PricingUnitOptions = new ObservableCollection<string>();
        // put HQ market currency(s) first
        // operating region currencies second
        // All other currencies and moneys third
        // everything else last  TODO make this toggleable somewhere, like a config.
        foreach (var product in dc.Products.Values
                     .Where(x => !PricingUnitOptions.Contains(x.GetName()))
                     .Where(x => parent.HeadQuarters.MarketPrices.ContainsKey(x))
                     .Select(x => x.GetName()))
        {
            PricingUnitOptions.Add(product);
        }

        WorkersAndJobs = new ObservableCollection<FirmJobModel>();
        _updateWorkersAndJobs();

        AssignmentGrid = new ObservableCollection<Pair<string, decimal>>();

        UpdateAssignments = ReactiveCommand.Create(_updateAssignments);
        RefreshExpected = ReactiveCommand.Create(_updateExpectedTables);
        _updateProjections();
    }
    
    public ReactiveCommand<Unit, Unit> UpdateAssignments { get; set; }
    
    public ReactiveCommand<Unit, Unit> RefreshExpected { get; set; }

    public decimal AvailableHours
    {
        get => _availableHours;
        set => this.RaiseAndSetIfChanged(ref _availableHours, value);
    }

    public ObservableCollection<FirmJobModel> WorkersAndJobs { get; set; }

    public FirmJobModel SelectedWorker
    {
        get => _selectedWorker;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedWorker, value);
            _updateAssignmentBreakdown();
        }
    }
    
    public ObservableCollection<CapStockAndSum> CapitalAndStock { get; set; }

    public ObservableCollection<string> PricingUnitOptions { get; set; }

    public string PricingUnit
    {
        get => _pricingUnit;
        set
        {
            _pricingUnit = value;
            _updatePrices();
        }
    }

    public string SelectedWorkerName
    {
        get => _selectedWorkerName;
        set => this.RaiseAndSetIfChanged(ref _selectedWorkerName, value);
    }
    
    public ObservableCollection<Pair<string, decimal>> AssignmentGrid { get; set; }

    public Pair<string, decimal>? SelectedAssignment
    {
        get => _selectedAssignment;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedAssignment, value);
            _updateExpectedTables();
        }
    }

    public ObservableCollection<Pair<string, decimal>> ExpectedInputs { get; set; }
    public ObservableCollection<Pair<string, decimal>> ExpectedCapitals { get; set; }
    public ObservableCollection<Pair<string, decimal>> ExpectedOutputs { get; set; }

    private void _updatePrices()
    {
        var pricer = dc.Products[PricingUnit];
        foreach (var row in CapitalAndStock)
        { // go through each row.
            if (dc.Products.ContainsKey(row.Item))
            { // if the item in the row is a product.
                var prod = dc.Products[row.Item];
                if (parent.HeadQuarters.MarketPrices.ContainsKey(prod))
                { // and the product has a price, then update the price to the market price.
                    row.Price = parent.HeadQuarters.MarketPrices[prod] / parent.HeadQuarters.MarketPrices[pricer];
                }
            }
        }
    }
    
    private void _updateAssignments()
    {
        if (SelectedWorker == null)
            return;
        var jobSelection = parent.Jobs.Single(x => x.Job.GetName() == SelectedWorker.Job &&
                                                   EnumExtension.ToName(x.WageType) == SelectedWorker.WageType);
        foreach (var assignment in AssignmentGrid)
        {
            // update the job in the parent
            jobSelection.Assignments[dc.Processes[assignment.Primary]].Iterations = assignment.Secondary;
        }
        // update Projections
        _updateProjections();
        // update Worker text
        //_updateWorkersAndJobs();
    }
    
    private void _updateExpectedTables()
    {
        if (SelectedAssignment == null)
            return;
        var process = dc.Processes[SelectedAssignment.Primary];
        ExpectedInputs.Clear();
        foreach (var input in process.InputProducts)
        {
            ExpectedInputs.Add(new Pair<string, decimal>(input.Product.GetName(), 
                input.Amount * SelectedAssignment.Secondary));
        }
        foreach (var input in process.InputWants)
        {
            ExpectedInputs.Add(new Pair<string, decimal>(input.Want.Name, 
                input.Amount * SelectedAssignment.Secondary));
        }
        
        ExpectedCapitals.Clear();
        foreach (var cap in process.CapitalProducts)
        {
            ExpectedCapitals.Add(new Pair<string, decimal>(cap.Product.GetName(), 
                cap.Amount * SelectedAssignment.Secondary));
        }
        foreach (var cap in process.CapitalWants)
        {
            ExpectedCapitals.Add(new Pair<string, decimal>(cap.Want.Name, 
                cap.Amount * SelectedAssignment.Secondary));
        }
        
        ExpectedOutputs.Clear();
        foreach (var output in process.OutputProducts)
        {
            ExpectedOutputs.Add(new Pair<string, decimal>(output.Product.GetName(), 
                output.Amount * SelectedAssignment.Secondary));
        }
        foreach (var output in process.OutputWants)
        {
            ExpectedOutputs.Add(new Pair<string, decimal>(output.Want.Name, 
                output.Amount * SelectedAssignment.Secondary));
        }
    }

    private void _updateProjections()
    {
        // update the parent's assignments
        foreach (var assignment in AssignmentGrid)
        {
            parent.Jobs.Single(x => x.Job.GetName() == SelectedWorker.Job)
                .Assignments[dc.Processes[assignment.Primary]].Iterations = assignment.Secondary;
        }

        // Update Workers and Jobs
        if (SelectedWorker != null)
        {
            var selJob = parent.Jobs.Single(x => x.Job.GetName() == SelectedWorker.Job);
            string assignments = "";
            foreach (var assignment in selJob.Assignments)
            {
                assignments += $"{assignment.Key.GetName()} : {assignment.Value.Iterations}\n";
            }
            // sum up the time of the worker and their assignments
            var hours = selJob.Pop.GetTotalHours();

            foreach (var assigment in selJob.Assignments)
            {
                if (assigment.Value.Iterations > 0 &&
                    assigment.Key.InputProducts.Any(x => x.Product.GetName() == "Time"))
                {
                    hours -= assigment.Value.Iterations * assigment.Key.GetProductsByName("Time", ProcessPartTag.Input)
                        .Sum(x => x.Amount);
                }
            }
            assignments += $"Idle Time: {hours}";

            SelectedWorker.Assignments = assignments.Trim();
        }

        // update stock and capital
        CapitalAndStock.Clear();
        foreach (var stock in parent.Resources)
        {
            // go through all existing resources, add them, and project the changes expected.
            var item = new CapStockAndSum(stock.Key.GetName(), stock: stock.Value);
            if (parent.HeadQuarters.MarketPrices.ContainsKey(stock.Key))
                item.Price = parent.HeadQuarters.MarketPrices[stock.Key];
            CapitalAndStock.Add(item);
        }

        // add missed products that are projected to change but not already in stock and capital
        foreach (var job in parent.Jobs)
        {
            // for each assignment in each job
            foreach (var assn in job.Assignments)
            {
                // if the process has no weight, skip it
                if (assn.Value.Iterations == 0)
                    continue;
                // foreach product in the process
                foreach (var prod in assn.Key.ProcessProducts)
                {
                    // if it's already in the list, update it
                    if (CapitalAndStock.Any(x => x.Item == prod.Product.GetName()))
                    {
                        var stock = CapitalAndStock.Single(x => x.Item == prod.Product.GetName());
                        if (prod.Part == ProcessPartTag.Input) // inputs to expenditures
                            stock.Expenditures += prod.Amount * assn.Value.Iterations;
                        else if (prod.Part == ProcessPartTag.Capital) // capital to used
                            stock.Used += prod.Amount * assn.Value.Iterations;
                        else if (prod.Part == ProcessPartTag.Output) // outputs are gained
                            stock.Gains += prod.Amount * assn.Value.Iterations;
                        // optional taken somewhere?
                    }
                    else
                    {
                        var stock = new CapStockAndSum(prod.Product.GetName());
                        if (prod.Part == ProcessPartTag.Input) // inputs to expenditures
                            stock.Expenditures += prod.Amount * assn.Value.Iterations;
                        else if (prod.Part == ProcessPartTag.Capital) // capital to used
                            stock.Used += prod.Amount * assn.Value.Iterations;
                        else if (prod.Part == ProcessPartTag.Output) // outputs are gained
                            stock.Gains += prod.Amount * assn.Value.Iterations;
                        CapitalAndStock.Add(stock);
                    }
                }

                // foreach want in the process
                foreach (var want in assn.Key.ProcessWants)
                {
                    // if it's already in the list, update it
                    if (CapitalAndStock.Any(x => x.Item == want.Want.Name))
                    {
                        var stock = CapitalAndStock.Single(x => x.Item == want.Want.Name);
                        if (want.Part == ProcessPartTag.Input) // inputs to expenditures
                            stock.Expenditures += want.Amount * assn.Value.Iterations;
                        else if (want.Part == ProcessPartTag.Capital) // capital to used
                            stock.Used += want.Amount * assn.Value.Iterations;
                        else if (want.Part == ProcessPartTag.Output) // outputs are gained
                            stock.Gains += want.Amount * assn.Value.Iterations;
                        // optional taken somewhere?
                    }
                    else
                    {
                        var stock = new CapStockAndSum(want.Want.Name);
                        if (want.Part == ProcessPartTag.Input) // inputs to expenditures
                            stock.Expenditures += want.Amount * assn.Value.Iterations;
                        else if (want.Part == ProcessPartTag.Capital) // capital to used
                            stock.Used += want.Amount * assn.Value.Iterations;
                        else if (want.Part == ProcessPartTag.Output) // outputs are gained
                            stock.Gains += want.Amount * assn.Value.Iterations;
                        CapitalAndStock.Add(stock);
                    }
                }
            }
        }
        
        _updateExpectedTables();
        
        // update time available.
    }
    
    private void _updateWorkersAndJobs()
    {
        foreach (var job in parent.Jobs)
        {
            string assignments = "";
            foreach (var assignment in job.Assignments)
            {
                assignments += $"{assignment.Key.GetName()} : {assignment.Value.Iterations}\n";
            }
            // sum up the time of the worker and their assignments
            var hours = job.Pop.GetTotalHours();

            foreach (var assigment in job.Assignments)
            {
                if (assigment.Value.Iterations > 0 &&
                    assigment.Key.InputProducts.Any(x => x.Product.GetName() == "Time"))
                {
                    hours -= assigment.Value.Iterations * assigment.Key.GetProductsByName("Time", ProcessPartTag.Input)
                        .Sum(x => x.Amount);
                }
            }
            assignments += $"Idle Time: {hours}";

            WorkersAndJobs.Add(new FirmJobModel
            {
                Job = job.Job.GetName(),
                Wage = job.Wage,
                WageType = EnumExtension.ToName(job.WageType),
                Assignments = assignments
            });
        }
    }
    
    private void _updateAssignmentBreakdown()
    {
        // clear out old assignments
        AssignmentGrid.Clear();
        // get the worker selected
        var jobSelection = parent.Jobs.Single(x => x.Job.GetName() == SelectedWorker.Job &&
                                EnumExtension.ToName(x.WageType) == SelectedWorker.WageType);
        // update to the new selected worker.
        foreach (var process in jobSelection.Assignments)
        {
            AssignmentGrid.Add(new Pair<string, decimal>(process.Key.GetName(), process.Value.Iterations));
        }
    }
}