using CommandLine;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ElasticSearchTest
{
    internal class Program
    {
        public class LogDocument
        {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Body { get; set; }
        }

        private static void Main(string[] args)
        {
            object x = CommandLine.Parser.Default.ParseArguments<CreateOptions, SearchOptions>(args)
              .MapResult(
                (CreateOptions opts) => DoCreate(opts),
                (SearchOptions opts) => DoSearch(opts),
                errs => 1);

            ////
        }

        private static object DoSearch(SearchOptions opts)
        {
            string indexName = opts.IndexName;

            ElasticClient client = GetConnection(opts.Host, indexName);

            SearchString(client, opts.Query);

            return 0;
        }

        private static object DoCreate(CreateOptions opts)
        {            
            string indexName = opts.IndexName ?? GetIndexNameFromPath(opts.Filename);

            ElasticClient client = GetConnection(opts.Host, indexName);
            CreateIndex(client, "divina_commedia.txt", indexName);

            return 0;
        }

        private static void SearchString(ElasticClient client, string searchStr)
        {
            Console.WriteLine($"Searching for ${searchStr}");
            ISearchResponse<LogDocument> searchResponse = client.Search<LogDocument>(s => s
                        .Size(10)
                        .Query(q => q.QueryString(

                            qs => qs.Query(searchStr)
                            .AllowLeadingWildcard(true)
                            )

                        )
                    );

            IReadOnlyCollection<LogDocument> docs = searchResponse.Documents;

            foreach (LogDocument doc in docs)
            {
                Console.WriteLine(doc.Body);
            }
        }

        private static ElasticClient GetConnection(string connectionUri, string indexname)
        {
            Uri node = new Uri(connectionUri);
            ConnectionSettings settings = new ConnectionSettings(node)
           .DefaultMappingFor<LogDocument>(m => m
               .IndexName(indexname)
           );

            return new ElasticClient(settings);
        }

        private static void CreateIndex(ElasticClient client, string filepath, string indexName)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();

            CreateIndexResponse createIndexResponse = client.Indices.Create(indexName, c => c
                .Map<LogDocument>(m => m
                    .AutoMap<LogDocument>()
                )
            );

            

            string[] lines = File.ReadAllLines(filepath);

            int items = 0;
            Parallel.ForEach(lines, (line) =>
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    client.CreateDocument<LogDocument>(new LogDocument()
                    {
                        Body = line.Trim()
                    });
                    items++;
                }
            });

            timer.Stop();
            Console.WriteLine($"Index created in {timer.ElapsedMilliseconds}ms with {items} element.");
        }

        private static string GetIndexNameFromPath(string filepath)
        {
            string basepath = Path.GetFileNameWithoutExtension(filepath).ToLower();
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            return rgx.Replace(filepath, "");
        }
    }
}