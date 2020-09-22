## Pre-requisites:
  - Docker
  - .Net Core 3.1
  - Node 12.18
  - GNU make


## Installation:


### `make install`

- installs app/node_modules
- restores dotnetexample.csproj dependencies
- stops running containers
- builds the containers

---
#### In order to use https you might need to create your self signed certificates for .NetCore.
#### For windows and macOS i do think running `make generate-local-certificate` should issue you a .pfx inside the `certificates` directory.
#### This is then used by the docker also.
---



## Running:


`docker-compose up -d`
#### OR

`make start`
***

- Payment Endpoint: https://localhost:5001/payment
- Payment Swagger: https://localhost:5001/swagger
- Frontend (Merchant): http://localhost:8080

---


## Tests:

`make run-tests`



Have Fun ;)