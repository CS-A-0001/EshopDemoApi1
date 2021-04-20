# EshopDemoApi 
## Base description

This case study is sample WebApi application with using .Net Core 3.1 (LTS). As data layer is used EntityFramework Core 5. For API description at /swagger is used Swashbuckle.

### Stored object structure

Product - id,name,imgUri,price,description

It's seeded by mocked data and has 3 endpoints. 
 - GET item by id
 - PUT item by id (only for change description)
 - GET items

API has two versions. For versioning is used URL pattern. Version 2.0 extends GET items endpoint by paging.
Solution consists of two projects WEB API and unit test. Logging is not included. On first run seed of database is performed.

### Prequisities for running
Installed Docker desktop and running, Visual studio 2019

### Steps how to run application
Open solution in Visual Studio 2019 and run with default docker-compose lunch settings. It can tak sometime while dependency docker image mssql is downloaded and containers are prepared. In default API description - URL /swagger is opened browser.

### How to run unit tests
Open solution in Visual Studio 2019 and make action "Run Tests" on solution.
