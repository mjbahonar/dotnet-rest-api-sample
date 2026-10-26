FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/TaskApi/TaskApi.csproj src/TaskApi/
RUN dotnet restore src/TaskApi/TaskApi.csproj
COPY src/TaskApi/ src/TaskApi/
RUN dotnet publish src/TaskApi/TaskApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__Default="Data Source=/data/todos.db"
VOLUME /data
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskApi.dll"]
