Sistema de Gestão - Company Management API
Sistema em C# para gestão de Companies, Leads, Proposals e Products com validação dinâmica de regras de negócio.
Regras de Negócio Implementadas
Validações Dinâmicas (Aplicadas em Runtime)

NIF obrigatório para empresas portuguesas: Company.Country == "Portugal" → NIF required
BusinessType obrigatório para leads ativos: Lead.Status == "Active" → BusinessType required
ProductionCost obrigatório para propostas ativas: Proposal.Status != "Draft" → ProductionCost required
Produtos obrigatórios para finalizar: Proposal.Status == "Finalized" → ProductIDs not empty
MonthlyProducedProducts para propostas finalizadas: Proposal.Status == "Finalized" → MonthlyProducedProducts required

Relações de Dados (Sem Duplicação)

Lead.Country: Não armazenado - usa Company.Country por referência
Proposal.Country: Não armazenado - usa Lead.Company.Country por referência
Product Dependencies: Produtos dependentes devem ter o mesmo ProductType

Como Executar
Pré-requisitos

.NET 6.0 ou superior

Comandos
bash# Clonar e navegar
git clone [url-do-repositorio]
cd Cross

# Restaurar, compilar e executar
dotnet restore
dotnet build
dotnet run
Acessar API

Aplicação: https://localhost:7016
Swagger: https://localhost:7016/swagger

Como Testar as Regras
Testar NIF para Portugal
jsonPOST /api/company
{
    "country": "Portugal",
    "nif": "",  // Deve retornar erro: "NIF is required for Portuguese companies"
    "address": "Lisboa"
}
Testar BusinessType para Lead Ativo
jsonPOST /api/lead
{
    "companyId": "{company-id}",
    "businessType": "",  // Deve retornar erro: "BusinessType is required when lead is Active"
    "status": "Active"
}
Sequência de Teste Completa

Criar Company (com NIF se Portugal)
Criar Lead (com BusinessType se Active)
Criar Products
Criar Proposal e adicionar produtos
Finalizar Proposal (todos os campos obrigatórios preenchidos)

Executar Testes Unitários
bashdotnet test
Características Principais

Validação Dinâmica: Campos obrigatórios baseados em condições (não hardcoded)
Sem Duplicação: Country herdado via referências (Lead → Company, Proposal → Lead)
Dados InMemory: Persistem durante execução da aplicação
API REST: Documentada com Swagger

Desenvolvido para Programming Challenge 
