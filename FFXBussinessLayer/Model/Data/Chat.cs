using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.Data
{
    [Table("ChatSessions")]
    public class ChatSessionData
    {
        [Key]
        public Int64 ChatSessionId { get; set; }
        
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyData Company { get; set; }
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

        [Required]
        public bool IsActive { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [MaxLength(200)]
        public string ChatListenerUsers { get; set; }
        public Int64? LastChatEntry { get; set; }
    }
    [Table("ChatEntries")]
    public class ChatEntrieData
    {
        [Key]
        public Int64 ChatEntryId { get; set; }

        public Int64 ChatSessionId { get; set; }
        [ForeignKey("ChatSessionId")]
        public virtual ChatSessionData ChatSession { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [MaxLength(4000)]
        public string Text { get; set; }
        [Required]
        public DateTime Time { get; set; }
        
        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        public Consts.eChatEntryType EntryType { get; set; }
        [MaxLength(4000)]
        public string ObjectData { get; set; }
    }
}
