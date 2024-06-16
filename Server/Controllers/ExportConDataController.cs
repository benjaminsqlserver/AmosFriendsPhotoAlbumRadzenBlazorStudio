using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using AmosFriendsPhotoAlbum.Server.Data;

namespace AmosFriendsPhotoAlbum.Server.Controllers
{
    public partial class ExportConDataController : ExportController
    {
        private readonly ConDataContext context;
        private readonly ConDataService service;

        public ExportConDataController(ConDataContext context, ConDataService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/ConData/friendphotos/csv")]
        [HttpGet("/export/ConData/friendphotos/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendPhotosToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFriendPhotos(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/friendphotos/excel")]
        [HttpGet("/export/ConData/friendphotos/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendPhotosToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFriendPhotos(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/friends/csv")]
        [HttpGet("/export/ConData/friends/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetFriends(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/friends/excel")]
        [HttpGet("/export/ConData/friends/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportFriendsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetFriends(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/genders/csv")]
        [HttpGet("/export/ConData/genders/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGendersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGenders(), Request.Query, false), fileName);
        }

        [HttpGet("/export/ConData/genders/excel")]
        [HttpGet("/export/ConData/genders/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGendersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGenders(), Request.Query, false), fileName);
        }
    }
}
