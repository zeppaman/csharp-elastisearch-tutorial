using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElasticSearchTest
{
 
        [Verb("create", HelpText = "Create the index")]
        public class CreateOptions
        {
            [Option('i', "index", Required = false, HelpText = "the name of the index you want to create, if not provided is computed by filename")]
            public string IndexName { get; set; }

            [Option('f', "filename", Required = true, HelpText = "the filename")]
            public string Filename { get; set; }

            [Option('h', "host", Required = true, HelpText = "the host uri")]
            public string Host { get; set; }

        }

        [Verb("search", HelpText = "search the index")]
        public class SearchOptions
        {
            [Option('i', "index", Required = true, HelpText = "the name of the index you want to search")]            
            public string IndexName { get; set; }

            [Option('q', "query", Required = true, HelpText = "the query you want to search")]
            public string Query { get; set; }

            [Option('h', "host", Required = true, HelpText = "the host uri")]
            public string Host { get; set; }
        }


       
    
}
