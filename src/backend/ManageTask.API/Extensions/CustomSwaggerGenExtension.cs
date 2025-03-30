﻿using ManageTask.Application.Services;
using Microsoft.OpenApi.Models;

namespace ManageTask.API.Extensions
{
    public static class CustomSwaggerGenExtension
    {
        public static void AddCustomSwaggerGen(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("AccessToken", new OpenApiSecurityScheme
                {
                    Description = "Access token using the Bearer scheme.\r\n" +
                                  "Enter only your access token in the text input below.\r\n" +
                                  "Example: '12345abcdef'",
                    Name = AuthService.AuthorizationHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityDefinition("RefreshToken", new OpenApiSecurityScheme
                {
                    Description = "Refresh token.\r\n" +
                                  "Enter only your refresh token in the text input below.\r\n" +
                                  "Example: '12345abcdef'",
                    Name = AuthService.RefreshTokenHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKey"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "AccessToken"
                            },
                            Scheme = "Bearer",
                            Name = "AccessToken",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    },
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "RefreshToken"
                            },
                            Name = "RefreshToken",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
