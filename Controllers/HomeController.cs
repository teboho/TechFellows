using Newtonsoft.Json;
using Sensitive_Data_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sensitive_Data_Application.Controllers
{

    public class HomeController : Controller
    {
        // Get the connection string from ConnectionStrings.config
        private String connection = System.Configuration.
                                    ConfigurationManager.
                                    ConnectionStrings["AzureSQL"].ConnectionString;
        FellowDataClassesDataContext db;
        
        // Make a constructor
        public HomeController()
        {
            // Make a FellowDataContext object
            db = new FellowDataClassesDataContext(connection);
        }
        
        // GET /Home/Index = /Home = / 
        public ActionResult Index()
        {
            var fellows = (from f in db.Fellows select f);

            ViewBag.Fellows = fellows;
            return View();
        }

        /// <summary>
        /// Adding a new fellow <br />
        /// POST /Home = / {... : ..., ... : ...}
        /// </summary>
        /// <param name="fellow"></param>
        /// <returns></returns>
        public ActionResult PostFellow(Fellow fellow)
        {
            // Make a fellow object
            db.Fellows.InsertOnSubmit(fellow);

            string message = "";

            try
            {
                db.SubmitChanges();
            }
            catch(Exception e)
            {
                message = "Your fellow could not inserted the database! Why?";
            }
            // JsonConvert.SerializeObject(Fellow, Formatting.Indented)
            message = String.Format("Congratulations. The fellow, {0} {1}, has been added into our database.", fellow.Name, fellow.Surname);

            ViewBag.Message = message;
            ViewBag.ChangeType = "Adding New Fellow";

            return View();
        }

        [HttpGet]
        public ActionResult DeleteFellow(int Id)
        {
            var fellow = (from f in db.Fellows
                         where f.Id == Id
                         select f).FirstOrDefault();

            //Fellow fellow = new Fellow() { Id = Id, Name = Fellow.Name, };

            string message = "";

            // Prepare the Delete transaction
            db.Fellows.DeleteOnSubmit(fellow);

            try
            {
                db.SubmitChanges();
            }
            catch(Exception ex)
            {
                message = String.Format("Could not delete {0} {1}!", fellow.Name, fellow.Surname);
            }



            message = String.Format("Successfully deleted {0} {1}", fellow.Name, fellow.Surname);

            ViewBag.Message = message;
            ViewBag.ChangeType = "Deleting A Fellow";

            return View();
        }

        // Home/EditFellow/4
        [HttpGet]
        public ActionResult EditFellow(int Id)
        {
            // Needs a view with prefilled current content
            // get the fellow
            Fellow fellow = ( from f in db.Fellows
                           where f.Id == Id
                            select f
                        ).FirstOrDefault();

            // Pass the ID
            ViewBag.Id = Id;

            // Pass the model to the view
            return View(fellow);
        }


        public ActionResult PostEditedFellow(Fellow fellow, int Id)
        {
            // select
            var f = (from u in db.Fellows
                          where Id == u.Id
                          select u).FirstOrDefault();

            bool changed = false;
            string message = "";

            if (!f.Name.Equals(fellow.Name))
            {
                f.Name = fellow.Name;
                changed = true;
            }
            if (!f.Surname.Equals(fellow.Surname))
            {
                f.Surname = fellow.Surname;
                if (!changed) changed = true;
            }
            if (!f.Year.Equals(fellow.Year))
            {
                f.Year = fellow.Year;
                if (!changed) changed = true;
            }

            if (f.CompanyFound == null || !f.CompanyFound.Equals(fellow.CompanyFound))
            {
                f.CompanyFound = fellow.CompanyFound;
                if (!changed) changed=true;
            }

            if (!changed)
                message = string.Format("Did not update {0} {1}!", f.Name, f.Surname);

            try
            {
                db.SubmitChanges();
            } catch(Exception e)
            {
                message = e.Message;
            }


            message = string.Format("Changed fellow information to {0} {1}, founded {2}.", f.Name, f.Surname, f.CompanyFound);

            ViewBag.Message = message;
            ViewBag.ChangeType = "Editing A Fellow";

            return View();
        }
    }
}