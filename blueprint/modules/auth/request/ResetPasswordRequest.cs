﻿using blueprint.core.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace blueprint.modules.auth.request
{
    public class ResetPasswordRequest
    {
        [Required]
        [DefaultValue("")]
        public string code { get; set; }
        [Password]
        [Required]
        [DefaultValue("")]
        public string newPassword { get; set; }
    }
}
