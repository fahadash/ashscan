FROM microsoft/dotnet-framework-build:4.6.2 AS build-env


 WORKDIR /app
 COPY . .

 RUN msbuild.exe /t:Build /p:Configuration=Release /p:OutputPath=out
 