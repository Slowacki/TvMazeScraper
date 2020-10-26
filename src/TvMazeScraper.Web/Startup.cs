using System;
using System.Net;
using System.Net.Http;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Polly;
using Polly.Extensions.Http;
using TvMazeScraper.Data.Repositories;
using TvMazeScraper.Models;
using TvMazeScraper.Repositories;
using TvMazeScraper.Services;
using TvMazeScraper.Web.Configuration;
using TvMazeScraper.Web.Configuration.AutoMapper;
using TvMazeScraper.Web.Services;

namespace TvMazeScraper.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient(Configuration.GetSection("HttpClientNames").GetValue<string>("Shows"))
                .AddPolicyHandler(GetRetryPolicy());
            services.AddHttpClient(Configuration.GetSection("HttpClientNames").GetValue<string>("Cast"))
                .AddPolicyHandler(GetRetryPolicy());
            services.AddMvc()
                .AddJsonOptions(opt => opt.SerializerSettings.DateFormatString = "yyyy-MM-dd")
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ClientNamesOptions>(Configuration.GetSection("HttpClientNames"));
            services.Configure<UrlsOptions>(Configuration.GetSection("Urls"));

            services.AddSwaggerDocument(settings =>
            {
                settings.Title = "TvMazeScraper";
            });

            services.AddAutoMapper(typeof(ShowsProfile));

            RegisterServices(services);
            InitializeMongoDb(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IShowsService, ShowsService>();
            services.AddScoped<IShowsRepository, MongoDbShowsRepository>();
        }

        private void InitializeMongoDb(IServiceCollection services)
        {
            var mongoClient = new MongoClient(new MongoUrl(Configuration.GetSection("MongoDb").GetValue<string>("Url")));
            var db = mongoClient.GetDatabase(Configuration.GetSection("MongoDb").GetValue<string>("DatabaseName"));
            var collection = db.GetCollection<Show>(Configuration.GetSection("MongoDb").GetValue<string>("CollectionName"));
            var keys = Builders<Show>.IndexKeys.Ascending(f => f.CreatedAt);
            var model = new CreateIndexModel<Show>(keys, new CreateIndexOptions() { ExpireAfter = new TimeSpan(12, 0, 0) });

            collection.Indexes.CreateOne(model);
            services.AddSingleton(i => collection);
        }

        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1));
        }
    }
}