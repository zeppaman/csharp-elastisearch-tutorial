This repo contains the source code to demonstrate how to integrate C# applications with elastic searcg using NEST and custom queries.


## Execute the sample

```sh

dotnet ElasticSearchTest.dll create -f divina_commedia.txt -h http://localhost:9300
> Index created in 30338ms with 14006 element.

dotnet ElasticSearchTest.dll search -i divinacommediatxt -h http://localhost:9300 -q "dante AND virgi*"
> Searching for $dante AND virgi*
> "Dante, perché Virgilio se ne vada,
> "Dante, perché Virgilio se ne vada,

```