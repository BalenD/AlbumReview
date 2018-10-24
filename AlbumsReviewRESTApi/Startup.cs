using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlbumsReviewRESTApi.context;
using AlbumsReviewRESTApi.Entities;
using AlbumsReviewRESTApi.Helpers;
using AlbumsReviewRESTApi.Models;
using AlbumsReviewRESTApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

namespace AlbumsReviewRESTApi
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
                setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());

                setupAction.InputFormatters.Add(new XmlSerializerInputFormatter(setupAction));


            })
            .AddJsonOptions(options => 
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            var connectionString = Configuration["ConnectionStrings:AlbumsDB"];
            services.AddDbContext<AlbumsReviewContext>(x => x.UseSqlServer(connectionString));
            //  consider  later between scoped or transcient
            //  and change to multiple repositorys
            services.AddScoped<IAlbumsReviewRepository, AlbumsReviewRepository>();



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
            }

            AutoMapper.Mapper.Initialize(config =>
            {

                config.CreateMap<ArtistForCreationDto, Artist>();
                config.CreateMap<ArtistForUpdateDto, Artist>();

                

                config.CreateMap<Artist, ArtistDto>()
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
