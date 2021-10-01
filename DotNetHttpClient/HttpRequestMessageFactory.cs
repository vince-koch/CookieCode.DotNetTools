using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace DotNetHttpClient
{
    public class HttpRequestMessageFactory
    {
        public HttpRequestMessage LoadFile(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var request = Parse(stream);
                return request;
            }
        }

        public HttpRequestMessage Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream, leaveOpen: true))
            {
                var request = ParseFirstLine(reader);
                var headers = ParseHeaders(reader, request);
                var content = ParseContent(reader, request, headers);

                AddRequestHeaders(request, headers);
                AddContentHeaders(request, headers);

                return request;
            }
        }

        private HttpRequestMessage ParseFirstLine(StreamReader reader)
        {
            var firstLine = reader.ReadLine().Split(' ');

            var request = new HttpRequestMessage();
            request.Method = new HttpMethod(firstLine[0]);
            request.RequestUri = new Uri(firstLine[1]);
            request.Version = Version.Parse(firstLine[2].Split('/')[1]);

            return request;
        }

        private Dictionary<string, string[]> ParseHeaders(StreamReader reader, HttpRequestMessage request)
        {
            var dictionary = new Dictionary<string, string[]>();

            while (true)
            {
                if (reader.EndOfStream)
                {
                    break;
                }

                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                var key = line.Split(':').Take(1).Single();
                var values = string.Join(":", line.Split(':').Skip(1)).TrimStart().Split(";");
                dictionary.Add(key, values);
            }

            return dictionary;
        }

        private HttpContent ParseContent(StreamReader reader, HttpRequestMessage request, Dictionary<string, string[]> headers)
        {
            if (reader.EndOfStream)
            {
                return null;
            }

            var text = reader.ReadToEnd();

            var contentType = headers
                .Where(pair => pair.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                .Select(pair => pair.Value.Single())
                .SingleOrDefault();

            var content = contentType != null
                ? new StringContent(text, Encoding.UTF8, contentType)
                : new StringContent(text);

            request.Content = content;

            return request.Content;
        }

        private void AddRequestHeaders(HttpRequestMessage request, Dictionary<string, string[]> headers)
        {
            var requestHeaders = headers
                .Where(pair => !pair.Key.StartsWith("content-", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            AddHeaders(requestHeaders, request.Headers);
        }

        private void AddContentHeaders(HttpRequestMessage request, Dictionary<string, string[]> headers)
        {
            if (request.Content == null)
            {
                return;
            }

            // add the content headers, but not content-type or content-length (those have already been added)
            var contentHeaders = headers
                .Where(pair => pair.Key.StartsWith("content-", StringComparison.OrdinalIgnoreCase))
                .Where(pair => !pair.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase))
                .Where(pair => !pair.Key.Equals("content-length", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            AddHeaders(contentHeaders, request.Content.Headers);
        }

        private void AddHeaders(KeyValuePair<string, string[]>[] source, HttpHeaders target)
        {
            foreach (var pair in source)
            {
                if (pair.Value.Length == 1)
                {
                    target.Add(pair.Key, pair.Value.Single());
                }
                else
                {
                    target.Add(pair.Key, pair.Value);
                }
            }
        }
    }
}
