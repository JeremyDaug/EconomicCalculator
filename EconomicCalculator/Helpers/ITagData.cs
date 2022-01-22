﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EconomicCalculator.Helpers
{
    /// <summary>
    /// Generic holder for Tags and any attached Data.
    /// </summary>
    /// <typeparam name="T">An Enum of some kind.</typeparam>
    public interface ITagData<T> where T : Enum
    {
        /// <summary>
        /// The attached tag.
        /// </summary>
        T Tag { get; }

        /// <summary>
        /// Indexor for Parameters
        /// </summary>
        /// <param name="i">Index to retrieve.</param>
        /// <returns></returns>
        object this[int i] { get; }

        /// <summary>
        /// Count Function
        /// </summary>
        /// <returns></returns>
        int Count();
    }
}