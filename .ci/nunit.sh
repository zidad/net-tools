#mono --runtime=v4.0 .nuget/NuGet.exe install NUnit.Runners -Version 2.6.4 -o packages

runTest(){
   mono --runtime=v4.0 src/packages/NUnit.Runners.2.6.4/tools/nunit-console.exe -noxml -nodots -labels -stoponerror $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

runTest $1 -exclude=Performance

exit $?