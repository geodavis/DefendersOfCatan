using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DefendersOfCatan.DAL;
using System.Text;

namespace DefendersOfCatan.Controllers
{
    public class HomeController : Controller
    {

        //private GameContext db = new GameContext();

        public HomeController() { }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //public ActionResult Game()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}

        [HttpPost]
        public JsonResult TestPost(Test data)
        {
            var testData = data;
            //var test = db.Player;
            //var players = db.GetSet<Player>().Select(p => p.Name).ToList();
            //var player = db.GetSet<Players>().FirstOrDefault();
            //player.Gold = data.gold;
            //player.PositionX = data.positionX;
            //player.PositionY = data.positionY;
            //db.SaveChanges();

            return ReturnJsonResult("Successfully saved!");
        }

        public JsonResult ReturnJsonResult(object result)
        {
            return Json(result, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }


    }

    public class Test
    {
        public string name { get; set; }
        public int gold { get; set; }
        public int positionX { get; set; }
        public int positionY { get; set; }
    }

    public class ItemModel<TItem>
    {
        public ItemModel() { }

        public string Error { get; set; }
        public bool HasError { get; set; }
        public TItem Item { get; set; }

        public void AddError(Exception error) { }
    }
}