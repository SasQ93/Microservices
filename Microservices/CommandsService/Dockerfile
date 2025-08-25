FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "CommandsService.dll" ]



# build the image
# docker build -t commandsservice:latest .

## run container
# docker run -p 8080:8080 -d commandsservice:latest // nur 80 funktioniert nicht 

## running container
# docker ps

## run containet != start container
# run container erstellt einen neuen Container => mehrere gleiche Container möglich
# start container startet einen existierenden Container

## docker push