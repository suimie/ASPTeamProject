namespace RecipeWorld.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateInitalUser : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'064b43a1-f438-4ae4-83be-bb7eb2a38835', N'guest@recipeworld.com', 0, N'AEevoQ5EOKfNiaLrhCCWZdW/hS71t7jJNk5nnPHZIAS12HNcg4zryQu0vpOdGnjt2A==', N'e105ebd3-9f49-4444-9a0b-beaabe9b6e19', NULL, 0, 0, NULL, 1, 0, N'guest@recipeworld.com')
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'952d0e85-177e-4dfc-84d2-d8d80a248026', N'admin@recipeworld.com', 0, N'AEzYTBXJNK2MsATjtI/1MPNXKmKy2l5419fhSmMuMBalP1hjkqeO9eaCU+D9SwrhLg==', N'1a6b288e-a88d-4624-9c30-7ba555e04e5e', NULL, 0, 0, NULL, 1, 0, N'admin@recipeworld.com')");

            Sql(@"INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'fc636de6-050d-408f-b14a-04dabdc7b8c8', N'CanEditRecipe')");

            Sql(@"INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'952d0e85-177e-4dfc-84d2-d8d80a248026', N'fc636de6-050d-408f-b14a-04dabdc7b8c8')
");
        }
        
        public override void Down()
        {
        }
    }
}
