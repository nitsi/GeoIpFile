FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Kubernetes1/Kubernetes1.csproj", "Kubernetes1/"]

RUN dotnet restore "Kubernetes1/Kubernetes1.csproj"
COPY . .
WORKDIR "/src/Kubernetes1"
RUN dotnet build "Kubernetes1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kubernetes1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kubernetes1.dll"]