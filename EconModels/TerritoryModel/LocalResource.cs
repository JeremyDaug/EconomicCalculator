using EconModels.ProductModel;

namespace EconModels.TerritoryModel
{
    public class LocalResource
    {
        /// <summary>
        /// The Id of the Resource
        /// </summary>
        public int ResourceId { get; set; }

        /// <summary>
        /// The Resource In question.
        /// </summary>
        public virtual Product Resource { get; set; }

        /// <summary>
        /// The Id of the Territory.
        /// </summary>
        public int TerritoryId { get; set; }

        /// <summary>
        /// The Territory it is in.
        /// </summary>
        public virtual Territory Territory { get; set; }

        /// <summary>
        /// The Ammount of the Resource available in the territory.
        /// </summary>
        public decimal Available { get; set; }
    }
}