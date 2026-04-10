using ExcelSatýsVerileri.Context;
using ExcelSatýsVerileri.Service;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<ApiContext>();
builder.Services.AddScoped<SatislarService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

/*using (var scope = app.Services.CreateScope())
{
    var excelService = scope.ServiceProvider.GetRequiredService<SatislarService>();
    excelService.ImportExcel(@"C:\Users\AYSU YÝÐÝT\Desktop\SatisVerileri.xlsx");
}*/

app.Run();
