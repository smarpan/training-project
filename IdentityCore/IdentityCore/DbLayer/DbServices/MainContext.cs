using IdentityCore.DbLayer.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityCore.DbLayer.DbServices
{
    public class MainContext:IdentityDbContext<UserEntity>
    {
        public DbSet<PostEntity> posts { get; set; } = null!;
        public DbSet<LikePostEntity> likePostsAndUsers { get; set; } = null!;
        public DbSet<ContactEntity> chatContactsOfUser { get; set; } = null!;
        public DbSet<TempMessageStoreEntity> tempMessages { get; set; } = null!;
        public DbSet<MessageStoreEntity> messageStore { get; set; } = null!;
        public MainContext(DbContextOptions<MainContext> options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Contacts
            modelBuilder.Entity<ContactEntity>()
                        .HasOne(x => x.ContactBelongToUser)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContactEntity>()
                        .HasOne(x => x.ContactUser)
                        .WithMany()
                        .OnDelete(DeleteBehavior.NoAction);
            #endregion

            #region Temporary Messages
            modelBuilder.Entity<TempMessageStoreEntity>()
                       .HasOne(x => x.Sender)
                       .WithMany()
                       .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TempMessageStoreEntity>()
                        .HasOne(x => x.Receiver)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region MessageStore
            modelBuilder.Entity<MessageStoreEntity>()
                        .HasOne(x => x.BelongsToUser)
                        .WithMany()
                        .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MessageStoreEntity>()
                       .HasOne(x => x.Sender)
                       .WithMany()
                       .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MessageStoreEntity>()
                        .HasOne(x => x.Receiver)
                        .WithMany()
                        .OnDelete(DeleteBehavior.NoAction);
            #endregion
        }
    }
}
