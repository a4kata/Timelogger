using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Timelogger.Entities
{
    public class Project
    {
        [Key]
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("clientName")]
        public string ClientName { get; set; }
        [JsonPropertyName("deadline")]
        public DateTime Deadline { get; set; }
        public List<TimeLog> Logs { get; set; }
        [JsonPropertyName("hourRate")]
        public int HourRate { get; set; }
        [JsonPropertyName("time")]
        public double TotalTime
        {
            get
            {
                return Minutes.TotalMinutes / 60;

            }
        }
        public double Cost
        {
            get
            {
                return (Minutes.TotalMinutes / 60.0) * HourRate;
            }
        }
        public bool IsCompleted { get; set; }
        public TimeSpan Minutes { get; set; }
    }
}
