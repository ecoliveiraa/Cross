using WebApplication1.Services;
using WebApplication1.Services.Interfaces;
using WebApplication1.Repository;
using WebApplication1.Validation;        
using WebApplication1.Configuration;     

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddSingleton<ICompanyRepository, InMemoryCompanyRepository>(); 
builder.Services.AddScoped<ILeadService, LeadService>();
builder.Services.AddSingleton<ILeadRepository, InMemoryLeadRepository>(); 
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>(); 
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddSingleton<IProposalRepository, InMemoryProposalRepository>();
builder.Services.AddSingleton<IRuleManager, RuleManager>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var ruleManager = scope.ServiceProvider.GetRequiredService<IRuleManager>();
    RuleConfiguration.ConfigureRules(ruleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();