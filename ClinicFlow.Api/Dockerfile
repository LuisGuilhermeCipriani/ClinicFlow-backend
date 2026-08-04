FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["ClinicFlow.Api/ClinicFlow.Api.csproj", "ClinicFlow.Api/"]
COPY ["ClinicFlow.Application/ClinicFlow.Application.csproj", "ClinicFlow.Application/"]
COPY ["ClinicFlow.Domain/ClinicFlow.Domain.csproj", "ClinicFlow.Domain/"]
COPY ["ClinicFlow.Infrastructure/ClinicFlow.Infrastructure.csproj", "ClinicFlow.Infrastructure/"]

RUN dotnet restore "ClinicFlow.Api/ClinicFlow.Api.csproj"

COPY . .
RUN dotnet publish "ClinicFlow.Api/ClinicFlow.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ClinicFlow.Api.dll"]
