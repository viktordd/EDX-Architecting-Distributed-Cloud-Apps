FROM microsoft/aspnetcore:2.0
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
COPY ./publish .
ENTRYPOINT ["dotnet", "TemperatureHistorianService.dll"]