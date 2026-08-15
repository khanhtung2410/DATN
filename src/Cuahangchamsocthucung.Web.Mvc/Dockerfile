FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/Cuahangchamsocthucung.Web.Mvc/Cuahangchamsocthucung.Web.Mvc.csproj", "src/Cuahangchamsocthucung.Web.Mvc/"]
COPY ["src/Cuahangchamsocthucung.Web.Core/Cuahangchamsocthucung.Web.Core.csproj", "src/Cuahangchamsocthucung.Web.Core/"]
COPY ["src/Cuahangchamsocthucung.Application/Cuahangchamsocthucung.Application.csproj", "src/Cuahangchamsocthucung.Application/"]
COPY ["src/Cuahangchamsocthucung.Core/Cuahangchamsocthucung.Core.csproj", "src/Cuahangchamsocthucung.Core/"]
COPY ["src/Cuahangchamsocthucung.EntityFrameworkCore/Cuahangchamsocthucung.EntityFrameworkCore.csproj", "src/Cuahangchamsocthucung.EntityFrameworkCore/"]
WORKDIR "/src/src/Cuahangchamsocthucung.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/Cuahangchamsocthucung.Web.Mvc", "src/Cuahangchamsocthucung.Web.Mvc"]
COPY ["src/Cuahangchamsocthucung.Web.Core", "src/Cuahangchamsocthucung.Web.Core"]
COPY ["src/Cuahangchamsocthucung.Application", "src/Cuahangchamsocthucung.Application"]
COPY ["src/Cuahangchamsocthucung.Core", "src/Cuahangchamsocthucung.Core"]
COPY ["src/Cuahangchamsocthucung.EntityFrameworkCore", "src/Cuahangchamsocthucung.EntityFrameworkCore"]
WORKDIR "/src/src/Cuahangchamsocthucung.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "Cuahangchamsocthucung.Web.Mvc.dll"]
