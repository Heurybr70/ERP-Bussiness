using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Platform.Core.Configuration
{
    public sealed class DgiiOptions
    {
        public string Environment { get; init; } = "testecf";
        [Required]
        public string AuthBaseUrl { get; init; } = "";
        [Required]
        public string RecepcionBaseUrl { get; init; } = "";
        [Required]
        public string AnulacionBaseUrl { get; init; } = "";
        public int TimeoutSeconds { get; init; } = 60;
    }
}
