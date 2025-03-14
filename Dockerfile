# https://mcr.microsoft.com/en-us/artifact/mar/dotnet/sdk/tags
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env

WORKDIR /DMR2Mongo

COPY ["DMR2Mongo/", "DMR2Mongo/"]

RUN dotnet restore "DMR2Mongo/DMR2Mongo.csproj"

RUN dotnet publish "DMR2Mongo/DMR2Mongo.csproj" -p:PublishSingleFile=true -r linux-musl-x64 --self-contained -c Release -o /publish


FROM alpine:latest

RUN apk upgrade --no-cache && apk add --no-cache icu-libs

RUN mkdir "/DmrDatabase"

WORKDIR /src

COPY --from=build-env /publish /src

ENTRYPOINT ["./DMR2Mongo"]