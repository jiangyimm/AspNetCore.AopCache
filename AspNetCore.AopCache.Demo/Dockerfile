FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["../AspNetCore.AopCache.Demo/AspNetCore.AopCache.Demo.csproj", "../AspNetCore.AopCache.Demo/"]
RUN dotnet restore "../AspNetCore.AopCache.Demo/AspNetCore.AopCache.Demo.csproj"
COPY . .
WORKDIR "/src/../AspNetCore.AopCache.Demo"
RUN dotnet build "AspNetCore.AopCache.Demo.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "AspNetCore.AopCache.Demo.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AspNetCore.AopCache.Demo.dll"]