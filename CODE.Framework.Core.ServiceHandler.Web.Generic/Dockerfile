FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-obj/Docker/publish} .
#ENTRYPOINT ["dotnet", "CODE.Framework.Core.ServiceHandler.Web.dll"]
CMD ["/bin/sh", "-c", "dotnet 'CODE.Framework.Core.ServiceHandler.Web.dll'"]
