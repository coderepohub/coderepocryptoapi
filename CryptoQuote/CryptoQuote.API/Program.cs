
using AutoMapper;
using CryptoQuote.Agent;
using CryptoQuote.Agent.Mappers;
using CryptoQuote.API.Middlewares;
using CryptoQuote.Contracts;
using CryptoQuote.Contracts.HttpConnector;
using CryptoQuote.HttpConnector;
using CryptoQuote.HttpConnector.Helpers;
using CryptoQuote.Models;

namespace CryptoQuote.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddOptions<CryptoApiOptions>().Bind(builder.Configuration.GetSection(CryptoApiOptions.Name));
            builder.Services.AddCryptoHttpClient();
            builder.Services.AddScoped<ICryptoQuoteAgent, CryptoQuoteAgent>();
            builder.Services.AddScoped<IRestClient, RestClient>();
            builder.Services.AddScoped<IHttpClientProvider, HttpClientProvider>();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new CryptoCurrencyCodeMapperProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<GlobalExceptionHandler>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}