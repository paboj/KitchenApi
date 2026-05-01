using Kitchen.Core.Repositories;
using Kitchen.Application.Services;
using Kitchen.Infrastructure.DAL.Repositories;
using Kitchen.Infrastructure.DAL;
using Microsoft.EntityFrameworkCore;
using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCore();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();



app.Run();