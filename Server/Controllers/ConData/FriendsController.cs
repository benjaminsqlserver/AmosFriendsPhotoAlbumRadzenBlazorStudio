using System;
using System.Net;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AmosFriendsPhotoAlbum.Server.Controllers.ConData
{
    [Route("odata/ConData/Friends")]
    public partial class FriendsController : ODataController
    {
        private AmosFriendsPhotoAlbum.Server.Data.ConDataContext context;

        public FriendsController(AmosFriendsPhotoAlbum.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> GetFriends()
        {
            var items = this.context.Friends.AsQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>();
            this.OnFriendsRead(ref items);

            return items;
        }

        partial void OnFriendsRead(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> items);

        partial void OnFriendGet(ref SingleResult<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/Friends(FriendID={FriendID})")]
        public SingleResult<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> GetFriend(int key)
        {
            var items = this.context.Friends.Where(i => i.FriendID == key);
            var result = SingleResult.Create(items);

            OnFriendGet(ref result);

            return result;
        }
        partial void OnFriendDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        [HttpDelete("/odata/ConData/Friends(FriendID={FriendID})")]
        public IActionResult DeleteFriend(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Friends
                    .Where(i => i.FriendID == key)
                    .Include(i => i.FriendPhotos)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnFriendDeleted(item);
                this.context.Friends.Remove(item);
                this.context.SaveChanges();
                this.OnAfterFriendDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnFriendUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        [HttpPut("/odata/ConData/Friends(FriendID={FriendID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutFriend(int key, [FromBody]AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Friends
                    .Where(i => i.FriendID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnFriendUpdated(item);
                this.context.Friends.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Friends.Where(i => i.FriendID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Gender");
                this.OnAfterFriendUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/Friends(FriendID={FriendID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchFriend(int key, [FromBody]Delta<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Friends
                    .Where(i => i.FriendID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Friend>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnFriendUpdated(item);
                this.context.Friends.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Friends.Where(i => i.FriendID == key);
                Request.QueryString = Request.QueryString.Add("$expand", "Gender");
                this.OnAfterFriendUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnFriendCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);
        partial void OnAfterFriendCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] AmosFriendsPhotoAlbum.Server.Models.ConData.Friend item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (item == null)
                {
                    return BadRequest();
                }

                this.OnFriendCreated(item);
                this.context.Friends.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Friends.Where(i => i.FriendID == item.FriendID);

                Request.QueryString = Request.QueryString.Add("$expand", "Gender");

                this.OnAfterFriendCreated(item);

                return new ObjectResult(SingleResult.Create(itemToReturn))
                {
                    StatusCode = 201
                };
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
