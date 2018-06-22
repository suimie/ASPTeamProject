using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RecipeWorld.ViewModel;
using RecipeWorld.Models;

namespace RecipeWorld.Controllers
{
    public class WriteRecipeController : Controller
    {
        // GET: WriteRecipe
        public ActionResult Index()
        {
            var viewModel = new RecipeFormViewModel()
            {
                Recipe = new Recipe(),
                RecipeFiles = new List<RecipeFile>()
            };

            return View(viewModel);
        }
    }
}