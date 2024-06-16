using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AmosFriendsPhotoAlbum.Server.Models.ConData
{
    [Table("Genders", Schema = "dbo")]
    public partial class Gender
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
        public int GenderID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string GenderName { get; set; }

        public ICollection<Friend> Friends { get; set; }
    }
}