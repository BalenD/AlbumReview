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
using AlbumReview.Services.Data.DtoModels;
using AlbumReview.Services.Web;
using AlbumReview.Data.Models;
using AlbumReview.Data;
using AlbumReview.Web.Helpers;
using AlbumReview.Web.Api.services;

namespace AlbumReview.Web
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
            services.AddDbContext<AlbumReviewContext>(x => x.UseSqlServer(connectionString, y => y.MigrationsAssembly("AlbumReview.Data")));

            services.AddScoped<IPaginationUrlHelper, PaginationUrlHelper>();

            //  consider  later between scoped or transcient
            //  and change to multiple repositorys
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IAlbumRepository, AlbumRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory => 
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

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
