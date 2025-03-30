using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (context.MethodInfo.GetParameters().Any(p => p.ParameterType == typeof(List<IFormFile>)))
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["images"] = new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    }
                                },
                                ["caption"] = new OpenApiSchema { Type = "string" },
                                ["categoryId"] = new OpenApiSchema { Type = "integer" },
                                ["content"] = new OpenApiSchema { Type = "string" }
                            },
                            Required = new HashSet<string> { "caption", "categoryId", "content" }
                        }
                    }
                }
            };
        }
    }
}