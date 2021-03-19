using EconModels;
using EconModels.Enums;
using EconModels.ProductModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleInterface
{
    class Program
    {
        private static CMDInputManager inputManager;
        static void Main(string[] args)
        {
            inputManager = new CMDInputManager();
            // Initial Loading
            Console.WriteLine("Loading Values ----");
            using (var context = new EconSimContext())
            {
                Console.WriteLine("Product Count: " + context.Products.Count());
                Console.Write("Product Name >>");
                string name = Console.ReadLine();
                context.Products.Add(new Product
                {
                    Name = name,
                    VariantName = "",
                    UnitName = "unit",
                    Quality = 1,
                    DefaultPrice = 1.10M,
                    Bulk = 1,
                    ProductType = ProductTypes.Good,
                    Maintainable = false,
                    Fractional = false,
                    MeanTimeToFailure = 100,
                });

                var products = context.Products.ToList();
                var failProducts = from m in context.Products
                                   where m.FailsInto != null
                                   select m;

                Console.WriteLine(products.ToString());

                context.SaveChanges();

                Console.WriteLine("NewProductCount = " + context.Products.Count());

                Console.ReadLine();
            }
        }
    }
}
