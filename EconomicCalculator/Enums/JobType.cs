using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Enums
{
    /// <summary>
    /// The general types of job types.
    /// </summary>
    public enum JobType
    {
        LandLord, // Land Owners (Not Used)
        Capitalist, // Investors (not used)
        Artisan, // Singular Crafters 
        Bureaucrat, // Government Employees (not used)
        Clergymen, // Priests
        Clerk, // Managers
        Officer, // Military Officers
        Craftsmen, // Group Crafters
        Farmer, // Crop Workers
        Laborer, // Mine Workers and catch-all worker, doesn't really craft or produce.
        Slave, // super cheap laborers (not used)
        Soldier // Professional soldiers (Not used)
    }
}
