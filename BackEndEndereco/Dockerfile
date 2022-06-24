#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BackEndEndereco/BackEndEndereco.csproj", "BackEndEndereco/"]
RUN dotnet restore "BackEndEndereco/BackEndEndereco.csproj"
COPY . .
WORKDIR "/src/BackEndEndereco"
RUN dotnet build "BackEndEndereco.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackEndEndereco.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackEndEndereco.dll"]