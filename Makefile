clean:
	rm -rf src/obj/
	rm -rf bin/
	rm -rf Tests/UnitTests/obj
	rm -rf Tests/UnitTests/bin

run-tests:
	dotnet test "./Tests/UnitTests/UnitTests.csproj"

generate-local-certificate:
	dotnet dev-certs https -ep ./certificates/backend.pfx -p defaultpassword
	dotnet dev-certs https --trust