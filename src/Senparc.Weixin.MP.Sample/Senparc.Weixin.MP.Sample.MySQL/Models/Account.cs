using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Senparc.Weixin.MP.Sample.MySQL.Models
{
    public class Account
    {
        public long Id { get; set; }
        
        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(50)]
        [Required]
        public string Password { get; set; }

        [MaxLength(50)]
        [Required]
        public string PasswordSalt { get; set; }

        [MaxLength(50)]
        public string NickName { get; set; }

        [MaxLength(50)]
        public string RealName { get; set; }

        [MaxLength(50)]
        public string Tel { get; set; }

        [MaxLength(150)]
        public string Email { get; set; }
        
        public bool EmailChecked { get; set; }

        [MaxLength(100)]
        public string WeixinOpenId { get; set; }

        [MaxLength(200)]
        public string PicUrl { get; set; }

        [MaxLength(200)]
        public string HeadUrl { get; set; }

        public Sex Sex { get; set; }

        [MaxLength(50)]
        public string QQ { get; set; }

        [MaxLength(20)]
        public string Country { get; set; }

        [MaxLength(20)]
        public string City { get; set; }
        
        [MaxLength(20)]
        public string District { get; set; }

        [MaxLength(250)]
        public string Address { get; set; }
        
        public string Note { get; set; }

        public int Type { get; set; }

        public DateTime ThisLoginTime { get; set; }

        [MaxLength(50)]
        public string ThisLoginIp { get; set; }

        public DateTime LastLoginTime { get; set; }

        [MaxLength(50)]
        public string LastLoginIp { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime AddTime { get; set; }

        public DateTime? LastWeixinSignInTime { get; set; }
    }
}
