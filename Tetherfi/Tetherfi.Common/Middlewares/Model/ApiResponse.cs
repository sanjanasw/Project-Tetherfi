namespace Tetherfi.Common.Middlewares.Model;

public class ApiResponse
{
    public string Version { get; set; }

    public int StatusCode { get; set; }

    public string Message { get; set; }

    public object ResponseException { get; set; }

    public object Result { get; set; }

}

