using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmosFriendsPhotoAlbum.Server.Models.ConData
{
    [Table("FriendPhotos", Schema = "dbo")]
    public partial class FriendPhoto
    {

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("@odata.etag")]
        public string ETag
        {
            get;
            set;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PhotoID { get; set; }

        [ConcurrencyCheck]
        public int? FriendID { get; set; }

        public Friend Friend { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string PhotoURL { get; set; }

        [ConcurrencyCheck]
        public string PhotoDescription { get; set; }

        [ConcurrencyCheck]
        public DateTime? DateTaken { get; set; }
    }
}