﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace exSales.DTO.User
{
    public class ChangePasswordParam
    {
        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
    }
}
