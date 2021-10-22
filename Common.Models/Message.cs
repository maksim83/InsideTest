using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{

    [Index(nameof(SessionId))]
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        public long SessionId { get; private set; }
        public DateTime Ms1Timestamp { get; private set; }
        public DateTime Ms2Timestamp { get; private set; }
        public DateTime Ms3Timestamp { get; private set; }
        public DateTime EndTimestamp { get; private set; }


        public Message()
        {
            SetSessionId();
        }

        private void SetSessionId()
        {
            SessionId = DateTime.Now.Ticks / 10 % 1000000000;
            Ms1Timestamp = DateTime.Now;
        }

        public void SetMs1Timestamp()
        {
            Ms1Timestamp = DateTime.Now;
        }

        public void SetMs2Timestamp()
        {
            Ms2Timestamp = DateTime.Now;
        }

        public void CommitMessage()
        {
            EndTimestamp = DateTime.Now;
        }

    }
}
