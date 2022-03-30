using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IcreCreamRatingAPI_WithEntitySQL
{
    public class IceCreamRating
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string productId { get; set; }
        public DateTime timestamp { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
    }

    public class IceCreamRatingContext : DbContext
    {
        public IceCreamRatingContext()
        {
        }

        public IceCreamRatingContext(DbContextOptions<IceCreamRatingContext> options) : base(options) { }

     
        public DbSet<IceCreamRating> Rating
        {
            get;
            set;
        }

    }

}
