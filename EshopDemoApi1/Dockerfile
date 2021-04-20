#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["EshopDemoApi1/EshopDemoApi1.csproj", "EshopDemoApi1/"]
RUN dotnet restore "EshopDemoApi1/EshopDemoApi1.csproj"
COPY . .
WORKDIR "/src/EshopDemoApi1"
RUN dotnet build "EshopDemoApi1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EshopDemoApi1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EshopDemoApi1.dll"]