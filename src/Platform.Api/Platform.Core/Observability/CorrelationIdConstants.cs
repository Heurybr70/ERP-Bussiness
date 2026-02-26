using System;
using System.Collections.Generic;
using System.Text;

namespace Platform.Core.Observability
{
    public static class CorrelationIdConstants
    {
        public const string HeaderName = "X-Correlation-ID";
        public const string LogPropertyName = "CorrelationId";
    }
}
