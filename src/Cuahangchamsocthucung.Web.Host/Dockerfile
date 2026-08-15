FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/Cuahangchamsocthucung.Web.Host/Cuahangchamsocthucung.Web.Host.csproj", "src/Cuahangchamsocthucung.Web.Host/"]
COPY ["src/Cuahangchamsocthucung.Web.Core/Cuahangchamsocthucung.Web.Core.csproj", "src/Cuahangchamsocthucung.Web.Core/"]
COPY ["src/Cuahangchamsocthucung.Application/Cuahangchamsocthucung.Application.csproj", "src/Cuahangchamsocthucung.Application/"]
COPY ["src/Cuahangchamsocthucung.Core/Cuahangchamsocthucung.Core.csproj", "src/Cuahangchamsocthucung.Core/"]
COPY ["src/Cuahangchamsocthucung.EntityFrameworkCore/Cuahangchamsocthucung.EntityFrameworkCore.csproj", "src/Cuahangchamsocthucung.EntityFrameworkCore/"]
WORKDIR "/src/src/Cuahangchamsocthucung.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Cuahangchamsocthucung.Web.Host", "src/Cuahangchamsocthucung.Web.Host"]
COPY ["src/Cuahangchamsocthucung.Web.Core", "src/Cuahangchamsocthucung.Web.Core"]
COPY ["src/Cuahangchamsocthucung.Application", "src/Cuahangchamsocthucung.Application"]
COPY ["src/Cuahangchamsocthucung.Core", "src/Cuahangchamsocthucung.Core"]
COPY ["src/Cuahangchamsocthucung.EntityFrameworkCore", "src/Cuahangchamsocthucung.EntityFrameworkCore"]
WORKDIR "/src/src/Cuahangchamsocthucung.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Cuahangchamsocthucung.Web.Host.dll"]
