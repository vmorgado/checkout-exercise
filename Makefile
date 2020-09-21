clean:
	rm -rf obj/
	rm -rf bin/
	rm -rf Tests/UnitTests/obj
	rm -rf Tests/UnitTests/bin

run-tests:
	# make clean
	dotnet test "./Tests/UnitTests/UnitTests.csproj"