using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using AmosFriendsPhotoAlbum.Server.Data;

namespace AmosFriendsPhotoAlbum.Server
{
    public partial class ConDataService
    {
        ConDataContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly ConDataContext context;
        private readonly NavigationManager navigationManager;

        public ConDataService(ConDataContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportFriendPhotosToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friendphotos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friendphotos/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFriendPhotosToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friendphotos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friendphotos/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFriendPhotosRead(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> items);

        public async Task<IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto>> GetFriendPhotos(Query query = null)
        {
            var items = Context.FriendPhotos.AsQueryable();

            items = items.Include(i => i.Friend);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnFriendPhotosRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFriendPhotoGet(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);
        partial void OnGetFriendPhotoByPhotoId(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> items);


        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> GetFriendPhotoByPhotoId(int photoid)
        {
            var items = Context.FriendPhotos
                              .AsNoTracking()
                              .Where(i => i.PhotoID == photoid);

            items = items.Include(i => i.Friend);
 
            OnGetFriendPhotoByPhotoId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFriendPhotoGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFriendPhotoCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);
        partial void OnAfterFriendPhotoCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> CreateFriendPhoto(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto friendphoto)
        {
            OnFriendPhotoCreated(friendphoto);

            var existingItem = Context.FriendPhotos
                              .Where(i => i.PhotoID == friendphoto.PhotoID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.FriendPhotos.Add(friendphoto);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(friendphoto).State = EntityState.Detached;
                throw;
            }

            OnAfterFriendPhotoCreated(friendphoto);

            return friendphoto;
        }

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> CancelFriendPhotoChanges(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFriendPhotoUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);
        partial void OnAfterFriendPhotoUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> UpdateFriendPhoto(int photoid, AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto friendphoto)
        {
            OnFriendPhotoUpdated(friendphoto);

            var itemToUpdate = Context.FriendPhotos
                              .Where(i => i.PhotoID == friendphoto.PhotoID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(friendphoto);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFriendPhotoUpdated(friendphoto);

            return friendphoto;
        }

        partial void OnFriendPhotoDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);
        partial void OnAfterFriendPhotoDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.FriendPhoto> DeleteFriendPhoto(int photoid)
        {
            var itemToDelete = Context.FriendPhotos
                              .Where(i => i.PhotoID == photoid)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFriendPhotoDeleted(itemToDelete);


            Context.FriendPhotos.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFriendPhotoDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportFriendsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friends/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friends/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportFriendsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/friends/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/friends/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnFriendsRead(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> items);

        public async Task<IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>> GetFriends(Query query = null)
        {
            var items = Context.Friends.AsQueryable();

            items = items.Include(i => i.Gender);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnFriendsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnFriendGet(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnGetFriendByFriendId(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> items);


        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> GetFriendByFriendId(int friendid)
        {
            var items = Context.Friends
                              .AsNoTracking()
                              .Where(i => i.FriendID == friendid);

            items = items.Include(i => i.Gender);
 
            OnGetFriendByFriendId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnFriendGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnFriendCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> CreateFriend(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend friend)
        {
            OnFriendCreated(friend);

            var existingItem = Context.Friends
                              .Where(i => i.FriendID == friend.FriendID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Friends.Add(friend);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(friend).State = EntityState.Detached;
                throw;
            }

            OnAfterFriendCreated(friend);

            return friend;
        }

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> CancelFriendChanges(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnFriendUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> UpdateFriend(int friendid, AmosFriendsPhotoAlbum.Server.Models.ConData.Friend friend)
        {
            OnFriendUpdated(friend);

            var itemToUpdate = Context.Friends
                              .Where(i => i.FriendID == friend.FriendID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(friend);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterFriendUpdated(friend);

            return friend;
        }

        partial void OnFriendDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> DeleteFriend(int friendid)
        {
            var itemToDelete = Context.Friends
                              .Where(i => i.FriendID == friendid)
                              .Include(i => i.FriendPhotos)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnFriendDeleted(itemToDelete);


            Context.Friends.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterFriendDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportGendersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/genders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/genders/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGendersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/condata/genders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/condata/genders/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGendersRead(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> items);

        public async Task<IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender>> GetGenders(Query query = null)
        {
            var items = Context.Genders.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnGendersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGenderGet(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnGetGenderByGenderId(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> items);


        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> GetGenderByGenderId(int genderid)
        {
            var items = Context.Genders
                              .AsNoTracking()
                              .Where(i => i.GenderID == genderid);

 
            OnGetGenderByGenderId(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGenderGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGenderCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> CreateGender(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender gender)
        {
            OnGenderCreated(gender);

            var existingItem = Context.Genders
                              .Where(i => i.GenderID == gender.GenderID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Genders.Add(gender);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(gender).State = EntityState.Detached;
                throw;
            }

            OnAfterGenderCreated(gender);

            return gender;
        }

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> CancelGenderChanges(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGenderUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> UpdateGender(int genderid, AmosFriendsPhotoAlbum.Server.Models.ConData.Gender gender)
        {
            OnGenderUpdated(gender);

            var itemToUpdate = Context.Genders
                              .Where(i => i.GenderID == gender.GenderID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(gender);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGenderUpdated(gender);

            return gender;
        }

        partial void OnGenderDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        public async Task<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> DeleteGender(int genderid)
        {
            var itemToDelete = Context.Genders
                              .Where(i => i.GenderID == genderid)
                              .Include(i => i.Friends)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGenderDeleted(itemToDelete);


            Context.Genders.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGenderDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}