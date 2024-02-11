using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tetherfi.Common.Middlewares.Model;
using Tetherfi.Helpers.Exeptions;

namespace Tetherfi.Common.Middlewares;

public class ApiResponseWrapper
{
    private readonly RequestDelegate _next;

    public ApiResponseWrapper(RequestDelegate next)
    {
        _next = next;
    }


    private async Task<string> ReadResponseBodyStreamAsync(Stream bodyStream)
    {
        bodyStream.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(bodyStream).ReadToEndAsync();
        bodyStream.Seek(0, SeekOrigin.Begin);
        return responseBody;
    }

    public bool IsValidJson(string text)
    {
        text = text.Trim();
        if ((text.StartsWith("{") && text.EndsWith("}")) || //For object
            (text.StartsWith("[") && text.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Value!.Contains("api"))
        {
            var response = new ApiResponse();
            var originalResponseBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();

            try
            {
                context.Response.Body = memoryStream;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await _next.Invoke(context);
                var bodyAsText = await ReadResponseBodyStreamAsync(memoryStream);
                context.Response.Body = originalResponseBodyStream;

                dynamic bodyContent;
                if (IsValidJson(bodyAsText))
                {
                    bodyContent = JsonConvert.DeserializeObject<dynamic>(bodyAsText);
                }
                else
                {
                    bodyContent = bodyAsText;
                }

                response = new ApiResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Result = bodyContent
                };

                if (context.Response.StatusCode == 200)
                    response.Message = "Success";
            }
            catch (Exception ex)
            {
                response = HandleError(context, ex);
                context.Response.Body = originalResponseBodyStream;
            }
            finally
            {
                var text = JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
                await context.Response.WriteAsync(text);
            }
        }
        else
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await context.Response.WriteAsync(JsonConvert.SerializeObject(HandleError(context, ex)));
            }
        }
    }

    public ApiResponse HandleError(HttpContext context, Exception error)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;

        switch (error)
        {
            case KeyNotFoundException e:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;
            case HumanErrorException e:
                context.Response.StatusCode = (int)e.Status;
                return new ApiResponse
                {
                    StatusCode = (int)e.Status,
                    Result = e.Details,
                };
            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        return new ApiResponse
        {
            StatusCode = context.Response.StatusCode,
            Message = error.Message
        };

    }
}


public static class ResponseWrapper
{
    public static IApplicationBuilder UseResponseWrapper(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ApiResponseWrapper>();
    }
}

