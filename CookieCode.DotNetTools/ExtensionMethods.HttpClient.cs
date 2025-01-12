using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CookieCode.DotNetTools
{
    public static partial class ExtensionMethods
    {
        public static async Task DownloadFileAsync(this HttpClient client, string fileUrl, string outputFilePath)
        {
            using HttpResponseMessage response = await client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            using (Stream responseStream = await response.Content.ReadAsStreamAsync())
            using (Stream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await responseStream.CopyToAsync(fileStream);
            }
        }

        public static void DownloadFile(this HttpClient client, string fileUrl, string outputFilePath)
        {
            using HttpResponseMessage response = client.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead).Result;
            response.EnsureSuccessStatusCode();

            using (Stream responseStream = response.Content.ReadAsStream())
            using (Stream fileStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                responseStream.CopyTo(fileStream);
            }
        }
    }
}
