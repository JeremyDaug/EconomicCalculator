using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconModels.Enums
{
    public enum JobTypes
    {
        /// <summary>
        /// This job works on time tables over long periods of time
        /// most of the labor is simply maintenance. Ex. Farming.
        /// </summary>
        LongTerm,
        /// <summary>
        /// This job produces goods from 'nothing'. There are no 
        /// required inputs, only optional or equivalent. Instead, 
        /// it connects to the territory and extracts items from
        /// there, decreasing the supply in the territory.
        /// </summary>
        Extraction, 
        /// <summary>
        /// This job produces goods in fixed increments. Often it
        /// has a specific input which it works in.
        /// </summary>
        Crafter,
        /// <summary>
        /// A Manager is a type of service worker, however he is 
        /// only valuable as part of an organization and cannot
        /// work alone.
        /// </summary>
        Manager,
        /// <summary>
        /// This job produces goods in arbitrary amounts. This is
        /// mostly used for refining and processing raw goods
        /// into finished goods like milling grain into flour.
        /// </summary>
        Processing,
        /// <summary>
        /// Retail Jobs are the local middle man, buying local goods
        /// to sell them back at a profit. His primary job is buying
        /// and maintaining goods in storage and selling them at a
        /// later date.
        /// </summary>
        Retail,
        /// <summary>
        /// Shipping jobs don't produce any goods, instead they
        /// move between territories, buying goods low and
        /// selling them high for a profit. Their costs are 
        /// primarily in living costs and 
        /// </summary>
        Shipping,
        /// <summary>
        /// Service jobs don't produce any physical goods, instead
        /// they provide a service that is desired. Maintenance
        /// workers, healthcare providers, 
        /// </summary>
        Service,
        /// <summary>
        /// Land Lords sell housing, both permanent and temporary.
        /// </summary>
        LandLord,
        /// <summary>
        /// Researchers produce research, which is collected by
        /// the nation or territory and creates technological
        /// inventions.
        /// </summary>
        Researcher,
        /// <summary>
        /// Engineers produce engineering research, which
        /// creates new things from existing research and 
        /// innovates old ideas into new and more efficient
        /// ways.
        /// </summary>
        Engineer,
        /// <summary>
        /// This unit doesn't produce anything, instead 
        /// ensuring that the state is running effectively.
        /// </summary>
        Bureaucrat
    }
}
