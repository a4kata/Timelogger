using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Timelogger.Api.Module
{
    public class RegisterTimeRequest
    {
        [Required]
        public int ProjectID { get; set; }
        [Required]
        [Range(30, Int32.MaxValue)]
        public int Minutes { get; set; }
        [Required]
        public string Notes { get; set; }
    }
}
