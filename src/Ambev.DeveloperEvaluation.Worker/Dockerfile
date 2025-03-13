# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

# Expor portas caso necessário (o Worker pode não precisar, mas se usa SignalR ou outros serviços, pode ser útil)
EXPOSE 8082

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiando os arquivos do projeto Worker e suas dependências
COPY ["src/Ambev.DeveloperEvaluation.Worker/Ambev.DeveloperEvaluation.Worker.csproj", "src/Ambev.DeveloperEvaluation.Worker/"]
COPY ["src/Ambev.DeveloperEvaluation.IoC/Ambev.DeveloperEvaluation.IoC.csproj", "src/Ambev.DeveloperEvaluation.IoC/"]
COPY ["src/Ambev.DeveloperEvaluation.Domain/Ambev.DeveloperEvaluation.Domain.csproj", "src/Ambev.DeveloperEvaluation.Domain/"]
COPY ["src/Ambev.DeveloperEvaluation.Common/Ambev.DeveloperEvaluation.Common.csproj", "src/Ambev.DeveloperEvaluation.Common/"]
COPY ["src/Ambev.DeveloperEvaluation.Application/Ambev.DeveloperEvaluation.Application.csproj", "src/Ambev.DeveloperEvaluation.Application/"]
COPY ["src/Ambev.DeveloperEvaluation.ORM/Ambev.DeveloperEvaluation.ORM.csproj", "src/Ambev.DeveloperEvaluation.ORM/"]
COPY ["src/Ambev.DeveloperEvaluation.Persistence/Ambev.DeveloperEvaluation.Persistence.csproj", "src/Ambev.DeveloperEvaluation.Persistence/"]

# Restaurar pacotes
RUN dotnet restore "src/Ambev.DeveloperEvaluation.Worker/Ambev.DeveloperEvaluation.Worker.csproj"

# Copiar tudo e compilar
COPY src/ .
WORKDIR "/src/Ambev.DeveloperEvaluation.Worker"
RUN dotnet build "Ambev.DeveloperEvaluation.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa de publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Ambev.DeveloperEvaluation.Worker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa final (runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Definir ponto de entrada
ENTRYPOINT ["dotnet", "Ambev.DeveloperEvaluation.Worker.dll"]
