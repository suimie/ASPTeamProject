using RecipeWorld.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeWorld.ViewModels
{
    public class ReadRecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public List<RecipeFile> RecipeFiles { get; set; }
        public RecipeComment RecipeComment { get; set; }
        public int RecipeCommentsCount { get; set; }
    }
}