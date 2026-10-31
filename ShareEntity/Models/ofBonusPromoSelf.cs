using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareEntity.Models
{
    [Table("ofBonusPromoSelf")]
    public class ofBonusPromoSelf
    {
        [Key]
        public long Id { get; set; }
        public string Currency { get; set; }
        public string Account { get; set; }
        public decimal Amount { get; set; }
        public decimal Reward { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remarks { get; set; }
        public int Level { get; set; }
        public int Bet { get; set; }
        public string CreatePerson { get; set; }
        public string ReviewPerson { get; set; }
        public DateTime ReviewTime { get; set; }
        public int GameType { get; set; }
        public string Referrer { get; set; }
        public string ReferrerReg { get; set; }
        public string ReferenceGuid { get; set; }
        public int Status { get; set; }
        public int Visible { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
        public int Alert { get; set; }
        public string Reason { get; set; }
        public int GamePercentType { get; set; }
        public int BonusPromoType { get; set; }
        public int BonusPromoCode { get; set; }
        public int AutoPromoType { get; set; }
        public int PlatformType { get; set; }
        public int AccountType { get; set; }
        public string Saler { get; set; }
        public string AaffAccount { get; set; }
        public int DeviceId { get; set; }
        public int IsAutoPromo { get; set; }
        public DateTime? ExpiredTime { get; set; }
        public DateTime? VerifyTime { get; set; }
    }

}
