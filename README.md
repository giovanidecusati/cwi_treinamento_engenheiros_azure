# WebApi
## Packages
1. Install-Package Swashbuckle.AspNetCore
2. Install-Package Swashbuckle.AspNetCore.SwaggerGen
3. Install-Package Swashbuckle.AspNetCore.SwaggerUI
4. Install-Package Microsoft.EntityFrameworkCore.SqlServer
5. Install-Package Microsoft.EntityFrameworkCore.Tools
6. Install-Package Microsoft.VisualStudio.Web.CodeGeneration.Design (Scaffolding)
7. Install-Package Polly
8. Install-Package FluentValidator -Version 1.0.5
9. Install- Microsoft.ApplicationInsights.AspNetCor -Version 2.2.0-beta2
9. Add-Migration -OutputDir .\Infrastructure\Data\Migrations -Name InitialDB

## Docker
1. docker build -t "giovanidecusati/meetup" .
2. docker run -it --rm -p 8000:80 --name meetup_sample giovanidecusati/meetup
3. docker push giovanidecusati/meetup