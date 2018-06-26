using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RecipeWorld.Models;

namespace RecipeWorld.ViewModels
{
    public class RecipeViewModel
    {
        public Recipe Recipe { get; set; }
        public RecipeFile RecipeFile { get; set; }
        public int RecipeFilesCount { get; set; }
        public int RecipeCommentsCount { get; set; }
    }
}