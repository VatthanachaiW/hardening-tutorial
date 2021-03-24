ARG VERSION=5.0-alpine
FROM mcr.microsoft.com/dotnet/sdk:$VERSION AS builder
WORKDIR /src

COPY ["src/Hardening.API/","Hardening.API/"]
COPY ["src/Hardening.Models/","Hardening.Models/"]

COPY ["scripts/harden.sh","./harden.sh"]
COPY ["scripts/post-install.sh","./post-install.sh"]

RUN dotnet publish "Hardening.API/Hardening.API.csproj" \
	-c Release \
	-o /app/publish \
	-r alpine-x64 \
	--self-contained true \
	/p:PublishReadyToRun=true \
	/p:PublishReadyToRunShowWarnings=true \
	/p:PublishSingleFile=true \
	/p:TargetLatestRuntimePatch=true

RUN chmod u+x,o+x /app/publish/Hardening.API

FROM mcr.microsoft.com/dotnet/aspnet:$VERSION AS final
ENV APP_USER=app \
	APP_DIR="/$APP_USER" \
	DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
	DOTNET_RUNNING_IN_CONTAINER=true \
	ASPNETCORE_URLS=http://+:8080
		
RUN apk add --no-cache icu-libs --upgrade bash && \
	addgroup -S $APP_USER && \
	adduser -S $APP_USER -G $APP_USER

WORKDIR $APP_USER

COPY --from=builder /src/harden.sh .
RUN chmod +x harden.sh
CMD	"/harden.sh"
RUN	rm harden.sh

COPY --from=builder --chown=$APP_USER:$APP_USER /app/publish .
COPY --from=builder /src/post-install.sh .

RUN chmod +x post-install.sh
CMD	"/post-install.sh Hardening.API"
RUN	rm post-install.sh

RUN chmod +x Hardening.API && \	
	mkdir AccessToken && \
	chmod u+x,o+x AccessToken

RUN ls -la
USER $APP_USER
EXPOSE 8080
ENTRYPOINT ["./Hardening.API"]