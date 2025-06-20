using WebApplication1.Services;
using WebApplication1.Services.Interfaces;
using WebApplication1.Repository;
using WebApplication1.Validation;
using WebApplication1.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// CONFIGURAÇÃO SWAGGER COMPLETA
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Company Management API", 
        Version = "v1",
        Description = "API for managing Companies, Leads, Products and Proposals with dynamic validation rules"
    });
    
    // Incluir XML comments para documentação detalhada
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// REGISTAR TODOS OS SERVIÇOS
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

// CONFIGURAR AS REGRAS DE VALIDAÇÃO DINÂMICA APÓS BUILD
using (var scope = app.Services.CreateScope())
{
    var ruleManager = scope.ServiceProvider.GetRequiredService<IRuleManager>();
    RuleConfiguration.ConfigureRules(ruleManager);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Company Management API v1");
        c.RoutePrefix = string.Empty; // Faz com que Swagger apareça na raiz (https://localhost:7016/)
        c.DocumentTitle = "Company Management API";
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();