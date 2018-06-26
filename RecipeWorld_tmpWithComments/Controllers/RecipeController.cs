using RecipeWorld.Models;
using RecipeWorld.ViewModels;

using RecipeWorld.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Security;

namespace RecipeWorld.Controllers
{
    public class RecipeController : Controller
    {
        private ApplicationDbContext _context;
        public static int NumberOfRecipePerPage = 8;
        public RecipeController()
        {
            _context = new ApplicationDbContext();
        }

        // GET: Recipe
        public ActionResult Index(string SearchString, int Page=1)
        {
            List<RecipeViewModel> viewModels = new List<RecipeViewModel>();

            var recipes = _context.Recipes.ToList();

            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                var tmp = recipes.AsQueryable();
                // in LINQ
                tmp = (from r in tmp where r.Title.Contains(SearchString) || r.Contents.Contains(SearchString) || r.Ingredients.Contains(SearchString) select r);

                recipes = tmp.ToList();

                ViewBag.search = SearchString;
            }
            else
            {
                ViewBag.search = "";
            }

            int NumberOfRecipe = NumberOfRecipePerPage * Page;
            if (recipes.Count() > NumberOfRecipe)
                ViewBag.Page = Page;
            else
                ViewBag.Page = Page - 1;

            recipes = recipes.Take(NumberOfRecipe).ToList();

            foreach (var recipe in recipes)
            {
                int recipeFilesCount = _context.RecipeFiles.Where(rf => rf.RecipeId == recipe.Id).ToList().Count;
                int recipeCommentsCount = _context.Comments.Where(c => c.RecipeId == recipe.Id).ToList().Count;
                RecipeFile firstImage = _context.RecipeFiles.Where(rf => rf.RecipeId == recipe.Id).ToList().First();

                RecipeViewModel viewModel = new RecipeViewModel()
                {
                    Recipe = recipe,
                    RecipeFile = firstImage,    // to display in list
                    RecipeFilesCount = recipeFilesCount,
                    RecipeCommentsCount = recipeCommentsCount
                };
                viewModels.Add(viewModel);
                
            }


            return View(viewModels);
        }

        // GET: WriteRecipe
        [Authorize]
        public ActionResult New()
        {
            var viewModel = new RecipeViewModel()
            {
                Recipe = new Recipe()
            };
            viewModel.Recipe.Email = Membership.GetUser().UserName;
            viewModel.Recipe.CreateDate = DateTime.Today;
            viewModel.Recipe.ModifiedDate = DateTime.Today;

            return View("Edit", viewModel);
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            List<RecipeViewModel> viewModels = new List<RecipeViewModel>();

            var recipe = _context.Recipes.Where(r => r.Id == id).SingleOrDefault();


            var recipeFiles = _context.RecipeFiles.Where(rf => rf.RecipeId == recipe.Id).ToList();

            RecipeViewModel viewModel = new RecipeViewModel()
            {
                Recipe = recipe,
                RecipeFilesCount = recipeFiles.Count
            };

            viewModels.Add(viewModel);

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Save(RecipeFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // The form is not valid --> return same form to the user

                return View("Edit", viewModel);
            }

            // 1. Save Recipe
            Recipe recipe = viewModel.Recipe;

            if (recipe.Id == 0)
            {
                _context.Recipes.Add(recipe);
            }
            else
            {
                var recipeInDB = _context.Recipes.Single(r => r.Id == viewModel.Recipe.Id);

                /*
                 * This method has a security flow.
                TryUpdateModel(customerInDB);
                */

                // Manually update the fields I want.
                recipeInDB.Title = viewModel.Recipe.Title;
                recipeInDB.Ingredients = viewModel.Recipe.Ingredients;
                recipeInDB.Contents = viewModel.Recipe.Contents;
                recipeInDB.ModifiedDate = DateTime.Today;
            }


            _context.SaveChanges();

            // 2. Save Recipe Files
            if(viewModel.RecipeFiles.Count > 0)
            {
                List<HttpPostedFileBase> ojbImage = viewModel.RecipeFiles;
                foreach (var file in ojbImage)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        RecipeFile recipeFile = new RecipeFile();
                        recipeFile.ImgFile = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        recipeFile.RecipeId = recipe.Id;

                        file.SaveAs(Path.Combine(Server.MapPath("~/RecipeImageFiles"), recipeFile.ImgFile));

                        _context.RecipeFiles.Add(recipeFile);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "Recipe");
        }


        public ActionResult Read(int id)
        {
            List<ReadRecipeViewModel> viewModels = new List<ReadRecipeViewModel>();

            var recipe = _context.Recipes.Where(r => r.Id == id).SingleOrDefault();

            recipe.ViewCount++;
            _context.SaveChanges();

            var recipeFiles = _context.RecipeFiles.Where(rf => rf.RecipeId == recipe.Id).ToList();
            int recipeCommentsCount = _context.RecipeFiles.Where(rf => rf.RecipeId == recipe.Id).ToList().Count;

            ReadRecipeViewModel viewModel = new ReadRecipeViewModel()
            {
                Recipe = recipe,
                RecipeFiles = recipeFiles,
                RecipeComment = new RecipeComment(),
                RecipeCommentsCount = recipeCommentsCount
            };
            viewModel.RecipeComment.CreateDate = DateTime.Today;
            viewModel.RecipeComment.ModifiedDate = DateTime.Today;
            //viewModel.RecipeComment.Email = Membership.GetUser().UserName;
            viewModel.RecipeComment.RecipeId = recipe.Id;
            viewModel.RecipeComment.ParentArticleId = 0;

            viewModels.Add(viewModel);

            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var recipe = _context.Recipes.Where(r => r.Id == id).SingleOrDefault();
            if (recipe == null)
                return HttpNotFound();


            _context.Recipes.Remove(recipe);
            _context.SaveChanges();

            return RedirectToAction("Index", "Recipe");
        }

        public ActionResult ShowImages(int id)
        {
            var recipeFiles = _context.RecipeFiles.Where(rf => rf.RecipeId == id).ToList();

            return PartialView("_ShowImages", recipeFiles);
        }

        public ActionResult DeleteImage(int RecipeId, int ImageId)
        {
            var imageInDb = _context.RecipeFiles.Find(ImageId);

            if (imageInDb != null)
            {
                //Response.WriteFile(Server.MapPath("~/RecipeImageFiles/" + imageInDb.ImgFile));

                //Response.Flush();

                if (System.IO.File.Exists(Server.MapPath("~/RecipeImageFiles/" + imageInDb.ImgFile)))
                    System.IO.File.Delete(Server.MapPath("~/RecipeImageFiles/" + imageInDb.ImgFile));

                _context.RecipeFiles.Remove(imageInDb);
            }

            _context.SaveChanges();

            var recipeFiles = _context.RecipeFiles.Where(rf => rf.RecipeId == RecipeId).ToList();

            return PartialView("_ShowImages", recipeFiles);
        }

        public ActionResult CommentsIndex(int id)
        {
            var comments = _context.Comments.Where(c => c.RecipeId == id).ToList();

            return PartialView("_CommentsIndex", comments);

        }

        public ActionResult NewComments(int RecipeId, int ParentArticleId)
        {
            RecipeComment comments = new RecipeComment();
            comments.Email = "guest@recipeworld.com";//Membership.GetUser().UserName;
            comments.CreateDate = DateTime.Today;
            comments.ModifiedDate = DateTime.Today;
            comments.ParentArticleId = ParentArticleId;
            comments.RecipeId = RecipeId;

            return PartialView("_NewComments", comments);
        }

        public ActionResult SaveComments(RecipeComment comments)
        {
            if (!ModelState.IsValid)
            {
                // The form is not valid --> return same form to the user

                return PartialView("_NewComments", comments);
            }

            // 1. Save Recipe

            if (comments.Id == 0)
            {
                _context.Comments.Add(comments);
            }
            else
            {
                return HttpNotFound();
            }


            _context.SaveChanges();

            comments = new RecipeComment();

            return PartialView("_NewComments", comments);

        }
    }
}