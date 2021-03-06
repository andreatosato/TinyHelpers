﻿using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TinyHelpers.EntityFrameworkCore.Extensions;

namespace TinyHelpers.EntityFrameworkCore.Sample
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            using var dataContext = new DataContext();

            //var post = new Post
            //{
            //    Title = "TinyHelpers",
            //    Content = "A collection of helper methods and classes that I use everyday. I have packed them in a single library to avoid code duplication.",
            //    Authors = new string[] { "Marco", "Bot" },
            //    Date = DateTime.UtcNow,
            //    Reviews = new List<Review>
            //    {
            //        new Review { User = "Pippo", Date = DateTime.UtcNow, Score = 5 },
            //        new Review { User = "Pluto", Date = DateTime.UtcNow, Score = 4 }
            //    }
            //};

            //dataContext.Posts.Add(post);

            //await dataContext.SaveChangesAsync();

            await dataContext.ExecuteTransactionAsync(async () =>
            {
                var posts = await dataContext.Posts.ToListAsync();
                var post = posts.First();
                post.Reviews.First().User = "Topolino";
                await dataContext.SaveChangesAsync();
            });

            // Exception
            //var posts = await dataContext.Posts.Where(p => p.Reviews.Any())
            //    .ToListAsync();
        }
    }
}
