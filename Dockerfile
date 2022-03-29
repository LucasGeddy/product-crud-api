FROM mcr.microsoft.com/dotnet/sdk:6.0
COPY . /app
WORKDIR /app

RUN dotnet test product-crud-api.sln

RUN dotnet publish "product-crud-api\product-crud-api.csproj" -c Release -o /dist
WORKDIR "/dist"
ENTRYPOINT ["dotnet", "product-crud-api.dll"]
EXPOSE 80
EXPOSE 443

#WORKDIR /app
#ENV PATH="$PATH:/root/.dotnet/tools"
#RUN dotnet tool install --global dotnet-ef 
#RUN dotnet ef database update -p "product-crud-api\product-crud-api.csproj" --msbuildprojectextensionspath -- --environment Tests