﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EconomicCalculator.Enums;

namespace EconomicCalculator.Storage.Processes.ProcessTags
{
    public class AttachedProcessTag : IAttachedProcessTag
    {
        public AttachedProcessTag()
        {
            parameters = new List<object>();
        }

        private IList<object> parameters;

        /// <summary>
        /// Indexor
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i]
        {
            get
            {
                return parameters[i];
            }
            set
            {
                parameters[i] = value;
            }
        }

        public void Add(object obj)
        {
            parameters.Add(obj);
        }

        public ProcessTag Tag { get; set; }

        public IList<ParameterType> TagParameterTypes { get; set; }
    }
}