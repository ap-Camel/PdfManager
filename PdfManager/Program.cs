using PdfManager.Services.Implmentations;
using PdfManager.Services.Interfaces;


public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        //builder.Services
        //                .AddApiVersioning(x =>
        //                {
        //                    x.AssumeDefaultVersionWhenUnspecified = true;
        //                    x.DefaultApiVersion = new ApiVersion(1, 1);
        //                    x.ReportApiVersions = true;
        //                })
        //                .AddVersionedApiExplorer(x =>
        //                {
        //                    x.GroupNameFormat = "'v'VVV";
        //                    x.SubstituteApiVersionInUrl = true;
        //                })
        //                .AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen();

        // dependency injction
        builder.Services.AddScoped<IPdfService, PdfService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();
        //}

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseExceptionHandler("/error");

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}