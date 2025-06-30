using Bidding.Extensions;
using Bidding.Application;
using Bidding.Infrastructure;
using Bidding.Connector;
using Bidding.Application.IApplication;
using Bidding.Infrastructure.IInfrastructure;
using Bidding.Application.Application;
using Bidding.Infrastructure.Infrastructure;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddJWTTokenServices(builder.Configuration);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHostedService<BackgroundJobs>();
//builder.Services.AddHostedService<ActiveBackgroundJob>();
builder.Services.AddTransient<IAccountApplication, AccountApplication>();
builder.Services.AddTransient<IAccountInfrastructure, AccountInfrastructure>();
builder.Services.AddTransient<IApplicationUserApplication, ApplicationUserApplication>();
builder.Services.AddTransient<IApplicationUserInfrastructure, ApplicationUserInfrastructure>();
builder.Services.AddTransient<ICustomerApplication, CustomerApplication>();
builder.Services.AddTransient<ICustomerInfrastructure, CustomerInfrastructure>();
builder.Services.AddTransient<IEmailApplication, EmailApplication>();
builder.Services.AddTransient<IEmailInfrasutructure, EmailInfrasutructure>();
builder.Services.AddTransient<IServiceConnector, ServiceConnector>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowedOrigins",
        policy =>
        {
            policy.WithOrigins("https://localhost:8081") // note the port is included 
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("MyAllowedOrigins");

app.MapControllers();

app.Run();
