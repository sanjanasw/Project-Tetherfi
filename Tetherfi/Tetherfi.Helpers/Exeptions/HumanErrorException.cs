using System;
using System.Net;

namespace Tetherfi.Helpers.Exeptions;

public class HumanErrorException : Exception
{
    public HumanErrorException(HttpStatusCode status, object details)
    {
        Details = details;
        Status = status;
    }

    public object Details { get; set; }
    public HttpStatusCode Status { get; set; }
}

