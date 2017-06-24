# JsonClassGen
Command line wrapper around the fantastic json2csharp library.  

Adds batch support via config file.  Nearly 100% fleshed out, but feel free to contribute as it's pretty close but is doing all the bits I need for now.  

Once I started doing the batch processing I don't tend to use the Single file any more, however it's still provided.

## Usage 
### Batch processing
Generates all the code based on the configuration file.
```
JsonClassGen.exe batch -f configfile.json 
```
### Single file
Generates the code based for a single json file.  The below example generates nested classes in a single file using Pascal case, with the main class for the json file "MainClassName" with the awesome namespace.

```
JsonClassGen.exe generate -f myjson.json -n Awesome.Namespaces.Are.Awesome -m MainClassName -p -s --pascal --nested

 -f, --filename         Required. Input JSON file to read.
 -t, --targetfolder     Target output folder.
 -n, --namespace        Required. Target Namespace.
 -p, --useproperties    (Default: true) Use Properties.
 -i, --internal         (Default: false) Internal visibility only.
 -x, --explicit         (Default: false) Explicit deserialization.
 -h, --nohelp           (Default: false) No helper class.
 -m, --mainclass        Main class.
 --pascal               (Default: false) Pascal Case.
 --nested               (Default: false) Nested classes.
 -o, --obfuscation      (Default: false) Apply obfuscation attributes.
 -s, --singlefile       (Default: false) Single file.
 --nullable             (Default: false) Always use nullable values.
 --examples             (Default: false) Examples in documentation.
 -v, --verbose          Print details during execution.
 --help                 Display this help screen.
 --version              Display version information.
```
