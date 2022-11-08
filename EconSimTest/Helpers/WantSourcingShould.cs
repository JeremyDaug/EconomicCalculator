using EconomicSim.Helpers;
using EconomicSim.Objects.Processes;
using EconomicSim.Objects.Products;
using EconomicSim.Objects.Wants;
using NUnit.Framework;

namespace EconSimTest.Helpers;

[TestFixture]
public class WantSourcingShould
{
    private WantSourcing test;

    private IWant testWant;

    private IProduct testProduct1;
    private IProduct testProduct2;
    private IProduct testProduct3;
    private IProduct testProduct4;

    private IProcess UseProcess;
    private IProcess ConsumptionProcess;
}