FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY Microservices.sln ./
COPY DataLibrary/DataLibrary.csproj ./DataLibrary/
COPY PlatformService/PlatformService.csproj ./PlatformService/
RUN dotnet restore

COPY . ./
RUN dotnet publish PlatformService/PlatformService.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "PlatformService.dll"]




# build the image
# docker build -t platformservice:latest .

## run container
# docker run -p 8080:80 -d platformservice:latest

## running container
# docker ps

## run containet != start container
# run container erstellt einen neuen Container => mehrere gleiche Container möglich
# start container startet einen existierenden Container

## docker push 
