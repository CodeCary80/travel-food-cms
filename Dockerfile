FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY travelfoodcms.csproj .
RUN dotnet restore travelfoodcms.csproj
COPY . .
RUN dotnet build travelfoodcms.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish travelfoodcms.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "travelfoodcms.dll"]