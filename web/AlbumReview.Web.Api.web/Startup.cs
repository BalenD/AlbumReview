﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using AlbumReview.Services.Data;
using AlbumReview.Services.Web;
using AlbumReview.Data.Models;
using AlbumReview.Data;
using AlbumReview.Web.Api.Helpers;
using System.Collections.Generic;
using AlbumReview.Web.Api.DtoModels;
using AlbumReview.Web.Api.services;
using System.Linq;

namespace AlbumReview.Web.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction =>
            {
                setupAction.ReturnHttpNotAcceptable = true;

                var jsonOutputFormatter = setupAction.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.BD.json+hateoas");
                }
            })
            .AddJsonOptions(options => 
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            var connectionString = Configuration["ConnectionStrings:AlbumsDB"];
            services.AddDbContext<AlbumReviewContext>(x => x.UseSqlServer(connectionString, y => y.MigrationsAssembly("AlbumReview.Data")));

            services.AddScoped<IPaginationUrlHelper, PaginationUrlHelper>();
            services.AddScoped<IControllerHelper, ControllerHelper>();
            services.AddScoped<IHateoasHelper, HateoasHelper>();

            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IAlbumRepository, AlbumRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory => 
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddSingleton<IPropertyMappingService, PropertyMappingService>(implementationFactory => 
            {
                var pms = new PropertyMappingService();
                pms.AddPropertyMapping<ArtistDto, Artist>(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Id", new List<string>() { "Id" }},
                    { "StageName", new List<string>() { "StageName" }},
                    { "Age", new List<string>() { "DateOfBirth" }},
                    { "Name", new List<string>() { "FirstName", "LastName" }}
                });

                pms.AddPropertyMapping<ReviewDto, Review>(new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Id", new List<string>() { "Id" }},
                    { "SubmittedReview", new List<string>() { "SubmittedReview" }},
                    { "Rating", new List<string>() { "Rating" }},
                    { "Submitted", new List<string>() { "Submitted" }}

                });

                return pms;
            });

            services.AddTransient<ITypeHelperService, TypeHelperService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseStatusCodePages();
            }

            AutoMapper.Mapper.Initialize(config =>
            {

                config.CreateMap<ArtistForCreationDto, Artist>();
                config.CreateMap<ArtistForUpdateDto, Artist>();

                

                config.CreateMap<Artist, ArtistDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

                config.CreateMap<Artist, LinkedArtistDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));

                config.CreateMap<AlbumForCreationDto, Album>();

                config.CreateMap<AlbumForUpdateDto, Album>();


                config.CreateMap<Album, AlbumDto>()
                .ForMember(dest => dest.Released, opt => opt.MapFrom(src => src.Released.DateTime))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Released.CalculateAge()));

            });

            app.UseHttpsRedirection();
            app.UseMvc();

        }
    }
}
