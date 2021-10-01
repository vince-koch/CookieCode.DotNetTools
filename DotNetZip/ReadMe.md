# dotnet-zip

* Zips a project or solution folder
* Includes and excludes files and folders as specified in the root ```.gitignore``` file
* If no ```.gitignore``` file is found, [default rules](#default-rules) values will be applied
* ```.git/``` is always ignored, but this can be overriden by adding ```-r !.git/```

### To install
```
dotnet tool install dotnet-zip
```

###To use
```
dotnet zip -d [directory] -z [zipOutputPath] -r [rule] [rule] [rule]

Arguments:
    -d/--directory   The directory to begin zipping at
                     Defaults to the current working directory
    -z/--zip         Path to the output zip file
                     Defaults to create a zip file as a sibling to the source directory
    -r/--rules       One or more gitignore rules;  These will be applied AFTER rules contained in the .gitignore file
```

### Default Rules
```
[Bb]in/
[Oo]bj/
packages/
node_modules/
```