# ClinicFlow API

API principal do ClinicFlow, responsável pela autenticação, regras de negócio e persistência dos dados clínicos do sistema.

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-5C2D91?logo=dotnet)
![Oracle](https://img.shields.io/badge/Oracle-Database-F80000?logo=oracle)
![Docker](https://img.shields.io/badge/Docker-Containerized-2496ED?logo=docker)

## Resumo Executivo

O `ClinicFlow API` centraliza a lógica do sistema clínico e entrega ao frontend os dados necessários para autenticação, cadastro, consulta e manutenção operacional. O foco é manter a camada de serviço organizada, previsível e pronta para evolução.

## O que esta API resolve

- autenticação de usuários e perfis;
- controle de permissões por área de acesso;
- cadastro e consulta de pacientes;
- cadastro e consulta de médicos;
- gestão de especialidades;
- agenda e consultas;
- prontuário e indicadores operacionais;
- tratamento padronizado de erros e respostas.

## Diferenciais técnicos

- arquitetura em camadas para separar domínio, aplicação, infraestrutura e entrada HTTP;
- uso de Oracle como banco relacional principal;
- integração pronta para Docker;
- contratos e serviços organizados para facilitar testes e manutenção;
- endpoints pensados para consumo por frontend desacoplado.

## Arquitetura

O projeto segue uma separação em camadas:

- `ClinicFlow.Api` - controllers, filtros, configuração e entrada HTTP;
- `ClinicFlow.Application` - casos de uso, validações e serviços;
- `ClinicFlow.Domain` - entidades, contratos e regras de negócio;
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
- preparo para execução em contêiner;
- documentação pensada para facilitar onboarding e revisão técnica.

## Status do Projeto

- backend funcional e integrado ao frontend;
- pronto para execução local ou via Docker;
- evoluindo com foco em experiência profissional e apresentação de portfólio.

## Relacionado ao Frontend

O frontend da solução está em um repositório separado:

- [LuisGuilhermeCipriani/ClinicFlow-frontend](https://github.com/LuisGuilhermeCipriani/ClinicFlow-frontend)

## Licença

Projeto de uso pessoal, acadêmico e demonstrativo.
