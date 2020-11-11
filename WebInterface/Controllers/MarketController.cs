using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebInterface.Controllers
{
    public class MarketController : Controller
    {
        // GET: Market
        public string Index()
        {
            return "This is my <b>default</b> action...";
        }

        // GET: Market/Welcome
        public string Welcome(string name, int ID = 1)
        {
            return HttpUtility.HtmlEncode("Hello " + name + ", ID: " + ID);
        }
    }
}