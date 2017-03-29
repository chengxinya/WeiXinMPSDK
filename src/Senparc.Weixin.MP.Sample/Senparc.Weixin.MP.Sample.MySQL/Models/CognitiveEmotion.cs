using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Senparc.Weixin.MP.Sample.MySQL.Models
{
    public class CognitiveEmotion
    {
        public long Id { get; set; }

        public long AccountId { get; set; }
    
        public virtual Account Account { get; set; }

        [MaxLength(250)]
        public string PicUrl { get; set; }

        public string ResultJson { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime AddTime { get; set; }
    }
}
