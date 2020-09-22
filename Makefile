clean:
	rm -rf src/obj/
	rm -rf bin/
	rm -rf Tests/UnitTests/obj
	rm -rf Tests/UnitTests/bin
	rm -rf app/node_modules

run-tests:
	dotnet test "./Tests/UnitTests/UnitTests.csproj"

generate-local-certificate:
	dotnet dev-certs https -ep ./certificates/backend.pfx -p defaultpassword
	dotnet dev-certs https --trust

build-containers:
	make stop-containers
	docker-compose build

stop-containers:
	docker-compose down --remove-orphans

install:
	make install-node-modules
	make restore-dot-net
	make build-containers

install-node-modules:
	cd app && npm install;

restore-dot-net:
	dotnet restore "src/dotnetexample.csproj"

start:
	docker-compose up -d