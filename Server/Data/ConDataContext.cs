using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AmosFriendsPhotoAlbum.Server.Models.ConData;

namespace AmosFriendsPhotoAlbum.Server.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto>()
              .HasOne(i => i.Friend)
              .WithMany(i => i.FriendPhotos)
              .HasForeignKey(i => i.FriendID)
              .HasPrincipalKey(i => i.FriendID);

            builder.Entity<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>()
              .HasOne(i => i.Gender)
              .WithMany(i => i.Friends)
              .HasForeignKey(i => i.GenderID)
              .HasPrincipalKey(i => i.GenderID);
            this.OnModelBuilding(builder);
        }

        public DbSet<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> FriendPhotos { get; set; }

        public DbSet<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> Friends { get; set; }

        public DbSet<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> Genders { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    }
}