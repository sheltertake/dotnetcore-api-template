using FriendsApi.Model.Types;
using Microsoft.EntityFrameworkCore;

namespace FootballContacts.Contexts
{
    public class FriendContext : DbContext
    {
        public FriendContext(DbContextOptions<FriendContext> options) : base(options)
        {
        }

        public virtual DbSet<Friend> Friends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Friend>(entity =>
            {
               

            });
        }

    }
}
