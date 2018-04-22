FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# copy everything and build the project
COPY . ./
#RUN dotnet restore *.csproj
#RUN dotnet publish *.csproj -c Release -o out
RUN dotnet build
