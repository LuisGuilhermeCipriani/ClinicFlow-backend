# ClinicFlow API

API principal do ClinicFlow, responsável pela autenticação, regras de negócio e persistência dos dados clínicos do sistema.

## Visão Geral

O `ClinicFlow API` fornece os serviços consumidos pelo frontend e centraliza a operação da clínica em um backend organizado, com foco em:

- autenticação de usuários;
- controle de perfis e permissões;
- cadastro e consulta de pacientes;
- cadastro e consulta de médicos;
- gestão de especialidades;
- agenda e consultas;
- prontuário e indicadores operacionais;
- padronização de respostas e tratamento de erros.

## Arquitetura

O projeto segue uma separação em camadas para facilitar manutenção, testes e evolução:

- `ClinicFlow.Api` - camada de entrada HTTP, controllers, filtros e configuração da aplicação;
- `ClinicFlow.Application` - casos de uso, validações e serviços de aplicação;
- `ClinicFlow.Domain` - entidades, contratos e regras de domínio;
- `ClinicFlow.Infrastructure` - acesso a dados, repositórios e integrações.

## Tecnologias

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- Oracle Database
- Docker
- OpenAPI / Swagger

## Estrutura

```text
ClinicFlow-backend/
├─ ClinicFlow.Api/
├─ ClinicFlow.Application/
├─ ClinicFlow.Domain/
├─ ClinicFlow.Infrastructure/
└─ README.md
```

## Requisitos

- .NET SDK compatível com o projeto;
- Oracle Database disponível localmente ou via Docker;
- variáveis de ambiente configuradas conforme o ambiente de execução.

## Configuração

Os principais ajustes ficam no `appsettings.json` e no `appsettings.Development.json`.

### Principais chaves

- `ConnectionStrings:OracleDatabase`
- `Authentication:SigningKey`
- `Cors:AllowedOrigins`

## Contas de Demonstração

Estas contas são usadas no ambiente de desenvolvimento e demonstração:

- `admin` / `admin123`
- `recepcao` / `recepcao123`
- `medico` / `medico123`

## Como Executar

### Localmente

```powershell
dotnet run --project .\ClinicFlow.Api\ClinicFlow.Api.csproj
```

### Com Docker

```powershell
docker build -t clinicflow-api .
```

Se o projeto estiver integrado em um ambiente com `docker compose`, a API pode ser iniciada junto com o banco e o frontend pelo arquivo de orquestração da solução.

## Endpoints Principais

Os endpoints podem variar conforme a evolução da solução, mas a API normalmente expõe recursos como:

- `POST /api/auth/login`
- `GET /api/patients`
- `POST /api/patients`
- `PUT /api/patients/{id}`
- `GET /api/doctors`
- `GET /api/specialties`
- `GET /api/schedules`
- `GET /api/appointments`
- `GET /api/clinical-records`
- `GET /api/dashboard`

## Boas Práticas Adotadas

- mensagens e respostas padronizadas;
- tratamento centralizado de erros;
- autenticação baseada em sessão/token conforme o fluxo da aplicação;
- organização para reduzir acoplamento entre interface e persistência;
- preparo para execução em contêiner.

## Relacionado ao Frontend

O frontend da solução está em um repositório separado:

- [LuisGuilhermeCipriani/ClinicFlow-frontend](https://github.com/LuisGuilhermeCipriani/ClinicFlow-frontend)

## Licença

Projeto de uso pessoal, acadêmico e demonstrativo.
