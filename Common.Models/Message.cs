using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.Models
{

    [Index(nameof(SessionId))]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonInclude]
        public int? Id { get; private set; }
        [JsonInclude]
        public long SessionId { get; private set; }
        [JsonInclude]
        public DateTime? Ms1Timestamp { get; private set; }
        [JsonInclude]
        public DateTime? Ms2Timestamp { get; private set; }
        [JsonInclude]
        public DateTime? Ms3Timestamp { get; private set; }
        [JsonInclude]
        public DateTime? EndTimestamp { get; private set; }


        public Message()
        {
            Ms1Timestamp = DateTime.Now;
        }

        public void SetSessionId(long sessionId)
        {
            SessionId = sessionId;
        }     

        public void SetMs2Timestamp()
        {
            Ms2Timestamp = DateTime.Now;
        }

        public void SetMs3Timestamp()
        {
            Ms3Timestamp = DateTime.Now;
        }

        public void CommitMessage()
        {
            EndTimestamp = DateTime.Now;
        }

    }
}
