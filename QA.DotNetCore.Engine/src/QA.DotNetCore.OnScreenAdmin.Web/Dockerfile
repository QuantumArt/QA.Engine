FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

RUN apt-get update && \
    apt-get install unzip && \
    apt-get install -y wget && \
    apt-get install -y gnupg2 && \
    wget -qO- https://deb.nodesource.com/setup_8.x | bash - && \
    apt-get install -y build-essential nodejs

COPY .nuget .
COPY projectstructure.zip .

RUN unzip projectstructure.zip && dotnet restore --configfile NuGet.config --verbosity n -r linux-x64

WORKDIR /app/QA.DotNetCore.OnScreenAdmin.Web
COPY QA.DotNetCore.OnScreenAdmin.Web/package.json ./
RUN npm install
WORKDIR /app
COPY . ./

RUN dotnet publish QA.DotNetCore.OnScreenAdmin.Web/QA.DotNetCore.OnScreenAdmin.Web.csproj -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "QA.DotNetCore.OnScreenAdmin.Web.dll"]
