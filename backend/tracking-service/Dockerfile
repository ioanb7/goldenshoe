FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

#RUN MKDIR /app/nuget/
COPY --from=messages /app/nuget/ioanb7.Messages.1.0.1.nupkg /app/nuget/

# copy everything and build the project
COPY . ./
RUN dotnet restore *.csproj
RUN dotnet publish *.csproj -c Release -o out

# build runtime image
FROM microsoft/aspnetcore-build:2.0
WORKDIR /app
COPY --from=build-env /app/out ./
COPY --from=build-env /app/appsettings.json ./

EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "tracking-service.dll"]
