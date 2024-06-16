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
    [Route("odata/ConData/Genders")]
    public partial class GendersController : ODataController
    {
        private AmosFriendsPhotoAlbum.Server.Data.ConDataContext context;

        public GendersController(AmosFriendsPhotoAlbum.Server.Data.ConDataContext context)
        {
            this.context = context;
        }

    
        [HttpGet]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IEnumerable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> GetGenders()
        {
            var items = this.context.Genders.AsQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender>();
            this.OnGendersRead(ref items);

            return items;
        }

        partial void OnGendersRead(ref IQueryable<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> items);

        partial void OnGenderGet(ref SingleResult<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> item);

        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        [HttpGet("/odata/ConData/Genders(GenderID={GenderID})")]
        public SingleResult<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> GetGender(int key)
        {
            var items = this.context.Genders.Where(i => i.GenderID == key);
            var result = SingleResult.Create(items);

            OnGenderGet(ref result);

            return result;
        }
        partial void OnGenderDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderDeleted(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        [HttpDelete("/odata/ConData/Genders(GenderID={GenderID})")]
        public IActionResult DeleteGender(int key)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }


                var items = this.context.Genders
                    .Where(i => i.GenderID == key)
                    .Include(i => i.Friends)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnGenderDeleted(item);
                this.context.Genders.Remove(item);
                this.context.SaveChanges();
                this.OnAfterGenderDeleted(item);

                return new NoContentResult();

            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnGenderUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderUpdated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        [HttpPut("/odata/ConData/Genders(GenderID={GenderID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PutGender(int key, [FromBody]AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Genders
                    .Where(i => i.GenderID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender>(Request, items);

                var firstItem = items.FirstOrDefault();

                if (firstItem == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                this.OnGenderUpdated(item);
                this.context.Genders.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Genders.Where(i => i.GenderID == key);
                
                this.OnAfterGenderUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        [HttpPatch("/odata/ConData/Genders(GenderID={GenderID})")]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult PatchGender(int key, [FromBody]Delta<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender> patch)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var items = this.context.Genders
                    .Where(i => i.GenderID == key)
                    .AsQueryable();

                items = Data.EntityPatch.ApplyTo<AmosFriendsPhotoAlbum.Server.Models.ConData.Gender>(Request, items);

                var item = items.FirstOrDefault();

                if (item == null)
                {
                    return StatusCode((int)HttpStatusCode.PreconditionFailed);
                }
                patch.Patch(item);

                this.OnGenderUpdated(item);
                this.context.Genders.Update(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Genders.Where(i => i.GenderID == key);
                
                this.OnAfterGenderUpdated(item);
                return new ObjectResult(SingleResult.Create(itemToReturn));
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return BadRequest(ModelState);
            }
        }

        partial void OnGenderCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);
        partial void OnAfterGenderCreated(AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item);

        [HttpPost]
        [EnableQuery(MaxExpansionDepth=10,MaxAnyAllExpressionDepth=10,MaxNodeCount=1000)]
        public IActionResult Post([FromBody] AmosFriendsPhotoAlbum.Server.Models.ConData.Gender item)
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

                this.OnGenderCreated(item);
                this.context.Genders.Add(item);
                this.context.SaveChanges();

                var itemToReturn = this.context.Genders.Where(i => i.GenderID == item.GenderID);

                

                this.OnAfterGenderCreated(item);

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
