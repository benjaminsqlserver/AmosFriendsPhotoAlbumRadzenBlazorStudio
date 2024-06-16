using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmosFriendsPhotoAlbum.Server.Models.ConData
{
    [Table("Friends", Schema = "dbo")]
    public partial class Friend
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
        public int FriendID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string FirstName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string LastName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime DateOfBirth { get; set; }

        [ConcurrencyCheck]
        public int? GenderID { get; set; }

        public Gender Gender { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string EmailAddress { get; set; }

        [ConcurrencyCheck]
        public string PhoneNumber { get; set; }

        public ICollection<FriendPhoto> FriendPhotos { get; set; }
    }
}