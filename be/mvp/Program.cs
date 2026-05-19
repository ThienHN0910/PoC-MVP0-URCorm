using mvp.Interfaces;
using mvp.Options;
using mvp.Models;
using mvp.Services;
using mvp.Repos;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MongoDbOptions>(builder.Configuration.GetSection(MongoDbOptions.SectionName));
builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection(GeminiOptions.SectionName));

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAnyCors", policy =>
		policy.AllowAnyOrigin()
			.AllowAnyHeader()
			.AllowAnyMethod());
});

// Add services to the container.
builder.Services.AddHttpClient<IGeminiClient, GeminiClient>();
builder.Services.AddSingleton<IReviewRepository, MongoReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddHostedService<MongoSeedService>();


builder.Services.AddControllers();
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
app.UseCors("AllowAnyCors");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
