coverage:
	cd ./tests && dotnet test //p:CollectCoverage=true /p:Exclude=\"[*],[xunit*]*,[*]civica_service-tests*\" //p:ExcludeByAttribute="ExcludeFromCodeCoverage" //p:CoverletOutputFormat=lcov

setup:
	./setup.sh